using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class RotationController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerController player;
    public bool isRight;
    public Vector2 offsetSize;
    public Vector3 offsetPosition;
    

    private bool isRotating;
    private float cameraLastSize;
    private float sizeDiff;
    private Vector3 relativeDistance;

    void Start()
    {
        sizeDiff = 0;
        cameraLastSize = Camera.main.orthographicSize;
        isRotating = false;
        UpdateScale();
        updatePosition();
        relativeDistance = transform.position - player.transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        sizeDiff = cameraLastSize - Camera.main.orthographicSize;
        UpdateScale();

        transform.position = player.transform.position + relativeDistance;
        updatePosition();

        if (!isRotating)
            return;

        if (player != null)
        {
            player.Rotate(isRight);
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isRotating = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {   
        isRotating = false;
    }

    private void UpdateScale()
    {
        Vector3 top = Camera.main.ViewportToWorldPoint(Vector3.up);
        Vector3 botomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 botomRight = Camera.main.ViewportToWorldPoint(Vector3.right);
        
        float w = Mathf.Abs((botomRight.x - botomLeft.x));
        float h = Mathf.Abs(top.y - botomLeft.y);

        Vector3 scale = transform.localScale;
        scale.x = w + offsetSize.x;
        scale.y = h + offsetSize.y;
        transform.localScale = scale;
    }

    private void updatePosition()
    {
        Vector3 top = Camera.main.ViewportToWorldPoint(Vector3.up);
        Vector3 botomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 botomRight = Camera.main.ViewportToWorldPoint(Vector3.right);
        Vector3 middleY = top / 2;

        Vector3 positon = transform.position;
        positon.x = isRight ? botomRight.x : botomLeft.x;
        positon.x += sizeDiff /100;
        positon.y = middleY.y + (sizeDiff / 100);
        positon.z = (-1) * Camera.main.transform.position.z;
        transform.position = positon + offsetPosition;
    }
}
