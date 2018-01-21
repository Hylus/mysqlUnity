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
        Mediator.Instance.Subscribe<LoggedCmd>(OnLoggedCmd);
    }

    void OnLoggedCmd(LoggedCmd cmd)
    {
        buttonEnter.interactable = false;
        buttonRegisterNow.interactable = false;
    }

    void OnShowStatementCmd(ShowStatementCmd cmd)
    {
        Mediator.Instance.DeleteSubscriber<ShowStatementCmd>(OnShowStatementCmd);
        loginErrorText.text = cmd.Statement;
    }

    public void LogIn()
    {
        Mediator.Instance.Subscribe<ShowStatementCmd>(OnShowStatementCmd);
        DataBaseConnection.Instance.LoginValidate(loginField.text, passwordField.text);

    }
}
