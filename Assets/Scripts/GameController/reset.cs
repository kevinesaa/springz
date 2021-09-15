using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class reset : MonoBehaviour
{

    public Transform positionInitial;
    public GameObject gameObjectPlane;
    public Text lastOneText;
    public Text scoreText;
    public Text recordText;

    private ResetElement[] elements;
    private float record;

    void Start()
    {
        elements = GameObject.FindObjectsOfType<ResetElement>();
        record = PlayerPrefs.GetFloat("record", 0);
        recordText.text = record.ToString("F2");
    }

    private void OnGUI()
    {
        float distance = gameObjectPlane.transform.position.x - positionInitial.position.x;
        distance = Mathf.Abs(distance);
        scoreText.text = distance.ToString("F2");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        myReset();
    }


    public void myReset()
    {
        float distance = gameObjectPlane.transform.position.x - positionInitial.position.x;
        distance = Mathf.Abs(distance);
        lastOneText.text = distance.ToString("F2");
        if (distance > record)
        {
            record = distance;
            PlayerPrefs.SetFloat("record", record);
            recordText.text = record.ToString("F2");
        }
        gameObjectPlane.transform.position = positionInitial.position;
        foreach (ResetElement element in elements)
        {
            element.Reset();
        }
    }


}
