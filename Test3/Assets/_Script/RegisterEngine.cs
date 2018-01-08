using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterEngine : MonoBehaviour {

    int loginID;
    AsyncOperation async;
    private IEnumerator coroutine;

    /*
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (boardGameInstance == null)
        {
            boardGameInstance = this;
        }
        else
        {
            DestroyObject(gameObject);
        }
    }
    */

    private void Start()
    {
        Mediator.Instance.Subscribe<LoggedCmd>(OnLoggedCmd);
    }

    void OnLoggedCmd(LoggedCmd cmd)
    {
        // Mediator.Instance.DeleteSubscriber<LoggedCmd>(OnLoggedCmd);
        // StopAllCoroutines();
        
        // loginID = cmd.LoginID;
        //coroutine = LoadNewScene("GameScene");
        // StartCoroutine(coroutine);
        SceneManager.LoadScene("GameScene");

    }

    IEnumerator LoadNewScene(string sceneName)
    {
        yield return new WaitForSeconds(3);
        async = SceneManager.LoadSceneAsync(sceneName); 
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            yield return null;
        }
        //Debug.Log("the Game has been loaded, press left mouse button");
        Mediator.Instance.Publish<SceneIsLoadedCmd>(null);
        
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
        DeleteSubscribes();
        
        async.allowSceneActivation = true;
    }

    void DeleteSubscribes()
    {
        //Mediator.Instance.DeleteSubscriber<LoggedCmd>(OnLoggedCmd);
    }

    [ContextMenu("ToRegisterScene()")]
    void ToRegisterScene()
    {
        SceneManager.LoadScene(0);
    }
}
