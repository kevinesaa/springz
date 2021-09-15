using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorController : MonoBehaviour
{
    public enum State { IDLE,NORMAL_REWINDING ,TRAVELING_SHOOT_DIRECTION, ANCHOR, REWINDING, REWIND_END };

    [System.Serializable]
    public struct AnchorModel
    {
        public SpringJoint2D springJoint;
        public Transform pivotShoot;
        public LineRenderer lineRender;
    }


    public event Action<AnchorController.State> OnStateChange;
    public State AnchorState { get; private set; }
    public float MaxTravelTime { get; set; }
    public float ShootSpeed { get; set; }
    public float RewindSpeed { get; set; }
    public AnchorModel Anchor { get; set; }

    
    private bool hitToAnchor;
    private float travelTime;

    private void Update()
    {
        
        if((AnchorState == State.IDLE) || (AnchorState == State.REWIND_END))
        {
            transform.position = Anchor.pivotShoot.position;
            transform.rotation = Anchor.pivotShoot.rotation;
        }

        if (AnchorState == State.NORMAL_REWINDING)
        {
            RewindingAnimation(1.2f);
            RewindindState();
        }

        if (AnchorState == State.TRAVELING_SHOOT_DIRECTION)
        {
            TravelingShootDirectionState();
        }

        if (AnchorState == State.REWINDING)
        {
            RewindingAnimation(1f);
            RewindindState();
        }

        if (AnchorState == State.ANCHOR)
        {
            AnchorRotation();
        }
        DrawLine();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(AnchorState == State.TRAVELING_SHOOT_DIRECTION)
        {
            hitToAnchor = true;
            Anchor.springJoint.enabled = true;
            Anchor.springJoint.connectedAnchor = transform.position;
            AnchorState = State.ANCHOR;
            if (OnStateChange != null)
                OnStateChange(AnchorState);
        }
    }

    public void Shoot()
    {
        if (AnchorState != State.ANCHOR && AnchorState != State.REWINDING && AnchorState != State.TRAVELING_SHOOT_DIRECTION)
        {
            hitToAnchor = false;
            transform.position = Anchor.pivotShoot.position;
            transform.rotation = Anchor.pivotShoot.rotation;
            AnchorState = State.TRAVELING_SHOOT_DIRECTION;
            travelTime = 0;
        }
    }

    public void RemoveAnchor()
    {
        Anchor.springJoint.enabled = false;
        AnchorState = State.NORMAL_REWINDING;
    }


    private void DrawLine()
    {
        Anchor.lineRender.SetPosition(0, Anchor.pivotShoot.position);
        Anchor.lineRender.SetPosition(1, transform.position);
    }

    private void TravelingShootDirectionState()
    {
        Vector3 position = transform.position;
        float speed = ShootSpeed * Time.deltaTime;
        position = Vector3.MoveTowards(position, position + (ShootSpeed * Anchor.pivotShoot.right), speed);
        transform.position = position;
        travelTime += Time.deltaTime;

        if (travelTime >= MaxTravelTime)
        {
            if (!hitToAnchor)
            {
                AnchorState = State.REWINDING;
                AnchorRotation();
                if (OnStateChange != null)
                    OnStateChange(AnchorState);
            }
        }
    }

    private void RewindindState()
    {
        
        float distance = Vector3.Distance(transform.position, Anchor.pivotShoot.position);
        if ( Mathf.Approximately(distance, 0) )
        {
            transform.position = Anchor.pivotShoot.position;
            transform.rotation = Anchor.pivotShoot.rotation;
            AnchorState = State.REWIND_END;
            if (OnStateChange != null)
                OnStateChange(AnchorState);
        }
          
    }

    private void AnchorRotation()
    {
        Vector3 diff = transform.position - Anchor.pivotShoot.position;
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.eulerAngles = angle * Vector3.forward;
    }
    
    private void RewindingAnimation(float modify)
    {
        if (modify <= 0)
            modify = 1;

        Vector3 position = transform.position;
        float speed = modify * RewindSpeed * Time.deltaTime;
        position = Vector3.MoveTowards(position, Anchor.pivotShoot.position, speed);
        transform.position = position;
    }

}
