using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPlayer : MonoBehaviour {

    [SerializeField] InputField loginField;
    [SerializeField] InputField passwordField;
    [SerializeField] Text loginErrorText;

    [SerializeField] Button buttonEnter;
    [SerializeField] Button buttonRegisterNow;


    private void Start()
    {
        Mediator.Instance.Subscribe<SceneIsLoadedCmd>(OnSceneIsLoadedCmd);
        Mediator.Instance.Subscribe<LoggedCmd>(OnLoggedCmd);
    }

    private void OnDestroy()
    {
        Mediator.Instance.DeleteSubscriber<SceneIsLoadedCmd>(OnSceneIsLoadedCmd);
        Mediator.Instance.DeleteSubscriber<LoggedCmd>(OnLoggedCmd);
    }

    void OnLoggedCmd(LoggedCmd cmd)
    {
        // maybe i must delete subscribes at this moment

        buttonEnter.interactable = false;
        buttonRegisterNow.interactable = false;
    }

    void OnSceneIsLoadedCmd(SceneIsLoadedCmd cmd)
    {
        Mediator.Instance.DeleteSubscriber<SceneIsLoadedCmd>(OnSceneIsLoadedCmd);
        loginErrorText.text = "The game has been loaded, press left mouse button";
    }

    void OnShowStatementCmd(ShowStatementCmd cmd)
    {
        Mediator.Instance.DeleteSubscriber<ShowStatementCmd>(OnShowStatementCmd);
        loginErrorText.text = cmd.Statement;
    }

    public void LogIn()
    {
        Mediator.Instance.Subscribe<ShowStatementCmd>(OnShowStatementCmd);

        var cmd = new LogInCmd();
        cmd.Login = loginField.text;
        cmd.Password = passwordField.text;
        Mediator.Instance.Publish<LogInCmd>(cmd);
    }
}
