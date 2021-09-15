using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetElement : MonoBehaviour
{

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void Reset()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        gameObject.SetActive(true);
        int i = 0;
        while (i < transform.childCount)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            i++;
        }

    }
}