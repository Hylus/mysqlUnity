using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPlayerCanvas : MonoBehaviour {

    [SerializeField] int minNumberLoginChar;
    [SerializeField] int minNumberPasswordChar;
    [Space]
    [SerializeField] InputField loginField;
    [SerializeField] InputField passwordField;
    [SerializeField] InputField passwordRepeatField;

    [SerializeField] Text loginError;
    [SerializeField] Text passwordError;
    [SerializeField] Text repeatPasswordError;

    [SerializeField] Button signInButton;

    [SerializeField] Text statementText;

    bool correctLogin = false;
    bool correctPasswords=false;

    public void ClearField()
    {
        loginField.text = "";
        passwordField.text = "";
        passwordRepeatField.text = "";
    }

    private void Update()
    {
        signInButton.interactable = (correctLogin && correctPasswords) ? true : false;
    }

    public void Register()
    {
        if(correctPasswords)
        {
            if(passwordField.text != passwordRepeatField.text)
            {
                new UnityException("passwords are difference");
            }

            Mediator.Instance.Subscribe<ShowStatementCmd>(OnShowStatementCmd);
            DataBaseConnection.Instance.Register(loginField.text.Trim(), passwordField.text);
        }
    }

    void OnShowStatementCmd(ShowStatementCmd cmd)
    {
        Mediator.Instance.DeleteSubscriber<ShowStatementCmd>(OnShowStatementCmd);
        statementText.text = cmd.Statement;
        statementText.gameObject.SetActive(true);
    }

    public void OnLoginFieldChange(string login)
    {
        statementText.gameObject.SetActive(false);
        if (loginField.text.Length < minNumberLoginChar) 
        {
            correctLogin = false;
            loginError.text = "Login must be at least " + minNumberLoginChar + " characters";
            loginError.gameObject.SetActive(true);
        }
        else
        {
            correctLogin = true;
            loginError.gameObject.SetActive(false);
        }
    }

    public void OnChangePasswordValue(string password)
    {
        password = passwordField.text;
        if(password.Length < minNumberPasswordChar)
        {
            correctPasswords = false;
            passwordError.text = "Password must be at least " + minNumberPasswordChar + " characters";
            passwordError.gameObject.SetActive(true);
        }
        else
        {
            correctPasswords = true;
            passwordError.gameObject.SetActive(false);
            repeatPasswordError.gameObject.SetActive(false);//!
        }

        if (correctPasswords)
        {
            if (password != passwordRepeatField.text)
            {
                correctPasswords = false;
                repeatPasswordError.text = "Password are difference";
                repeatPasswordError.gameObject.SetActive(true);
            }
        }
    }

    public void OnChangeRepeatPasswordValue(string password)
    {
        password = passwordRepeatField.text;
        if (password.Length < minNumberPasswordChar)
        {
            correctPasswords = false;
            repeatPasswordError.text = "Password must be at least " + minNumberPasswordChar + " characters";
            repeatPasswordError.gameObject.SetActive(true);
        }
        else
        {
            if (password == passwordField.text)
            {
                correctPasswords = true;
                repeatPasswordError.gameObject.SetActive(false);
            }
            else
            {
                correctPasswords = false;
                repeatPasswordError.text = "Password are difference";
                repeatPasswordError.gameObject.SetActive(true);
            }
        }
    }
}
