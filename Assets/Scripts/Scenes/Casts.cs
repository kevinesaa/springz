using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Casts : MonoBehaviour
{

    public Image cast;
    //public string[] scenes;

    // Use this for initialization
    void Start()
    {
        cast.CrossFadeAlpha(0, 0.5f, false);
    }

    public void FadeOut(int s)
    {
        cast.CrossFadeAlpha(1, 0.5f, false);
        //StartCoroutine(ChangeScene(scenes[s]));
    }

    /*IEnumerator ChangeScene(string scene)
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene);
    } */
}