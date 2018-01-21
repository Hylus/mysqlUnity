using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterEngine : MonoBehaviour {

    int loginID;
    AsyncOperation async;
    private IEnumerator coroutine;

    private void Start()
    {
        Mediator.Instance.Subscribe<LoggedCmd>(OnLoggedCmd);
    }

    void OnLoggedCmd(LoggedCmd cmd)
    {
        Mediator.Instance.DestroyAllSubscribers();
        SceneManager.LoadScene("GameScene");

    }


    [ContextMenu("ToRegisterScene()")]
    void ToRegisterScene()
    {
        SceneManager.LoadScene(0);
    }
}
