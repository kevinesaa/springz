using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    [System.Serializable]
    public enum SceneEnum
    {
        LOADING = 0,
        MAIN = 1,
        PLAY = 2
    }
    public Slider loadingBar;

    private const string PLAY_NAME = "GamePlay";
    private const string MAIN_NAME = "MainMenu";
    private const string LOADING_NAME = "Loading";

    private void Start()
    {
        UpdateProgress(0);
    }

    public void ChangeScena(int sceneEnum)
    {
        ChangeScena((SceneEnum)sceneEnum);
    }

    public void ChangeScena(SceneEnum scene)
    {
        string sceneName = SelectScena(scene);
        StartCoroutine(Loading(sceneName));
    }

    private string SelectScena(SceneEnum scene)
    {
        string sceneName = "";
        switch (scene)
        {
            case SceneEnum.PLAY: sceneName = PLAY_NAME; break;
            case SceneEnum.MAIN: sceneName = MAIN_NAME; break;
            case SceneEnum.LOADING: sceneName = LOADING_NAME; break;
        }
        return sceneName;
    }

    private void UpdateProgress(float progress)
    {
        if (loadingBar != null)
        {
            loadingBar.value = progress;
        }
    }

    private IEnumerator Loading(string sceneName)
    {
        AsyncOperation scenaAsyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!scenaAsyncOperation.isDone)
        {
            UpdateProgress(scenaAsyncOperation.progress);
            yield return null;
        }
        StopCoroutine(Loading(sceneName));
    }
}