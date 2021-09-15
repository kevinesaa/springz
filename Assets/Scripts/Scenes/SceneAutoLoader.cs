using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAutoLoader : MonoBehaviour
{


    public SceneController sceneController;
    public SceneController.SceneEnum scene;
    public float timer;


    void Start()
    {


        Invoke("change", timer);
    }

    private void change()
    {
        sceneController.ChangeScena(scene);
    }
}
