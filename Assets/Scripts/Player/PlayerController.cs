using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IPointerDownHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    
    public enum PlayerState {IDLE,DRAGING, FLYING, PARTIAL_ANCHOR, COMPLETE_ANCHOR};
    public event Action<PlayerState> OnPlayerStateChange;

    public float maxDragRadiousDistance;
    public float defaultLookGrade = -90;
    public float rotationSpeed;
    public float maxShootDistance;
    public float shootSpeed;
    public float rewindSpeed;
    public AnchorController anchorPrefab;
    public AnchorController.AnchorModel leftAnchorModel;
    public AnchorController.AnchorModel righttAnchorModel;

    public PlayerState State { get; private set; } 


    private bool isInitialFlyEnd;
    private float normalRigidBodyDrag;
    private float maxDragRadiousDistanceSqrMagnitude;
    private Rigidbody2D mRigidBody;
    private Vector2 previusVelocity;
    private AnchorController[] anchorsControllers;
    private Vector3 centroid;
    private Dictionary<AnchorController.State, Action> actionByAnchorState;

    private void Awake()
    {

        actionByAnchorState = new Dictionary<AnchorController.State, Action>();
        actionByAnchorState.Add(AnchorController.State.ANCHOR, OnOneAnchor);
        actionByAnchorState.Add(AnchorController.State.REWIND_END, OnRewindEnd);

        mRigidBody = GetComponent<Rigidbody2D>();
        normalRigidBodyDrag = mRigidBody.drag;
        InitAchors(leftAnchorModel, righttAnchorModel);
        isInitialFlyEnd = true;
        previusVelocity = Vector2.zero;
        maxDragRadiousDistanceSqrMagnitude = maxDragRadiousDistance * maxDragRadiousDistance;
    }

    private void Update()
    {

        if (State == PlayerState.DRAGING)
        {
            DragingState();
            ViewPoint();
            CheckMaxDragRadius();
        }

        if (State == PlayerState.FLYING)
        {
            if (!isInitialFlyEnd)
            {
                IntialFlyState();
            }
        }
        
    }

    private void OnDestroy()
    {
        foreach (AnchorController anchor in anchorsControllers)
        {
            anchor.OnStateChange -= OnAnchorsChnageState;
        }
    }

    public void Rotate(bool turnToRight)
    {
        if (State == PlayerState.DRAGING || State == PlayerState.COMPLETE_ANCHOR)
            return;
        float rotation = turnToRight ? (-1) * rotationSpeed : rotationSpeed;
        rotation *= Time.deltaTime;
        transform.Rotate(Vector3.forward, rotation);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (((State == PlayerState.FLYING) && isInitialFlyEnd) || State == PlayerState.IDLE || State == PlayerState.PARTIAL_ANCHOR)
        {
            foreach (AnchorController anchor in anchorsControllers)
            {
                anchor.Shoot();
            }
            return;
        }
    }

    // need OnDrag(PointerEventData eventData) to work
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (State == PlayerState.IDLE || State == PlayerState.COMPLETE_ANCHOR)
        {
            CalculateCentroid();
            ViewPoint();
            State = PlayerState.DRAGING;
            isInitialFlyEnd = false;
            mRigidBody.isKinematic = true;
            if (OnPlayerStateChange != null)
                OnPlayerStateChange(State);
        }
    }

    // need OnDrag(PointerEventData eventData) to work
    public void OnEndDrag(PointerEventData eventData)
    {
        if (State == PlayerState.DRAGING)
        {
            ViewPoint();
            isInitialFlyEnd = false;
            State = PlayerState.FLYING;
            mRigidBody.isKinematic = false;
            mRigidBody.drag = 0;
            if (OnPlayerStateChange != null)
                OnPlayerStateChange(State);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //is needed for OnBeginDrag and OnEndDrag can be detect
        //it is to much slow for use for drag accion
    }

    private void InitAchors(params AnchorController.AnchorModel[] anchorsModel)
    {
        anchorsControllers = new AnchorController[anchorsModel.Length];
        for (int i = 0; i < anchorsModel.Length; i++)
        {
            anchorsControllers[i] = Instantiate(anchorPrefab, anchorsModel[i].pivotShoot.position, anchorsModel[i].pivotShoot.rotation);
            anchorsControllers[i].OnStateChange += OnAnchorsChnageState;
            anchorsControllers[i].ShootSpeed = shootSpeed;
            anchorsControllers[i].MaxShootDistance = maxShootDistance;
            anchorsControllers[i].RewindSpeed = rewindSpeed;
            anchorsControllers[i].Anchor = anchorsModel[i];
            anchorsModel[i].springJoint.enabled = false;
        }
    }

    private void DragingState()
    {
        Vector3 position;
#if UNITY_EDITOR
        position = Input.mousePosition;
#else
            position = Input.touches[0].position;
#endif

        position = Camera.main.ScreenToWorldPoint(position);
        position.z = transform.position.z;
        transform.position = position;
    }

    private void IntialFlyState()
    {

        
        isInitialFlyEnd = previusVelocity.sqrMagnitude > mRigidBody.velocity.sqrMagnitude;
        previusVelocity = mRigidBody.velocity;
        if (isInitialFlyEnd)
        {
            mRigidBody.velocity = previusVelocity;
            previusVelocity = Vector2.zero;
            foreach (AnchorController anchor in anchorsControllers)
            {
                anchor.RemoveAnchor();
            }
        }
    }

    private void OnAnchorsChnageState( AnchorController.State newState)
    {
        if(actionByAnchorState.ContainsKey(newState))
        {
            Action action = actionByAnchorState[newState];
            if (action != null)
                action();
        }
    }

    private void OnOneAnchor()
    {
        State = PlayerState.PARTIAL_ANCHOR;
        if (PotencialCompleteAnchor())
        {
            State = PlayerState.COMPLETE_ANCHOR;
            mRigidBody.drag = normalRigidBodyDrag;
            if (OnPlayerStateChange != null)
                OnPlayerStateChange(State);
        }
    }

    private void OnRewindEnd()
    {
        if (PotecialRewindEnd())
        {
            State = PlayerState.FLYING;
            if (OnPlayerStateChange != null)
                OnPlayerStateChange(State);
        }
    }

    private bool PotencialCompleteAnchor()
    {
        bool isCompletAnchor = true;
        foreach (AnchorController anchor in anchorsControllers)
        {
            isCompletAnchor = isCompletAnchor && (anchor.AnchorState == AnchorController.State.ANCHOR);
            if (!isCompletAnchor)
                return isCompletAnchor;
        }

        return isCompletAnchor;
    }

    private bool PotecialRewindEnd()
    {
        bool isRewindEnd = true;
        foreach (AnchorController anchor in anchorsControllers)
        {
            isRewindEnd = isRewindEnd && (anchor.AnchorState == AnchorController.State.REWIND_END);
            if (!isRewindEnd)
                return isRewindEnd;
        }

        return isRewindEnd;
    }
    
    private void ViewPoint()
    {
        Vector3 viewPoint = centroid - transform.position;
        float angle = Mathf.Atan2(viewPoint.y, viewPoint.x) * Mathf.Rad2Deg;
        if (angle != 180 && angle != 0)
        {
            angle += defaultLookGrade;
        }
        transform.eulerAngles = angle * Vector3.forward;
        if(viewPoint.y < 0)
        {
            transform.eulerAngles += 180 * Vector3.up;
        }
    }

    private void CheckMaxDragRadius()
    {
        Vector3 viewPoint = transform.position - centroid;
        if (viewPoint.sqrMagnitude > maxDragRadiousDistance)
        {
            Vector3 position = maxShootDistance * viewPoint.normalized;
            position = position + centroid;
            transform.position = position;
        }
    }

    private void CalculateCentroid()
    {
        centroid = Vector3.zero;
        foreach (AnchorController anchor in anchorsControllers)
        {
            centroid += anchor.transform.position;

        }
        centroid = centroid / anchorsControllers.Length;
    }
}
