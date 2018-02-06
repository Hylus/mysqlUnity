using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateEntity : MonoBehaviour {

    [SerializeField] Canvas canvas;
    [SerializeField] InputField nameInputField;
    [SerializeField] Dropdown dropdownProfession;
    [SerializeField] Button createButton;
    [SerializeField] Button returnButton;
    [SerializeField] Text errorText;


  
    private void Awake()
    {
        List<string> professionNameList = new List<string>(DataBaseConnection.Instance.GetProfessionsName());
        dropdownProfession.ClearOptions();
        dropdownProfession.AddOptions(professionNameList);
    }

    private void Update()
    {
        if(dropdownProfession.captionText.text != "" && nameInputField.text.Length > 3)
        {
            createButton.interactable = true;
        }
        else
        {
            createButton.interactable = false;
        }
    }

    public void Create()
    {
        int professionID = DataBaseConnection.Instance.GetProfessionID(dropdownProfession.captionText.text);
        int result = DataBaseConnection.Instance.CreateEntity(nameInputField.text, professionID);
        if(result == -1)
        {
            Debug.Log("database interrupt");
            errorText.text = "database interrupt";
        }
        else if (result == 0)
        {
            Debug.Log("login is not available");
            errorText.text = "login is not available";
        }
        else
        {
            Debug.Log("created new entity");

            Mediator.Instance.Publish<CreatedNewEntityCmd>(null);
            canvas.gameObject.SetActive(false);
        }
    }

    public void ClearAlert()
    {
        errorText.text = "";
    }

    public void Return()
    {
        canvas.gameObject.SetActive(false);
    }

    public void ShowCreateEntityCanvas()
    {
        canvas.gameObject.SetActive(true);
    }


	
}
