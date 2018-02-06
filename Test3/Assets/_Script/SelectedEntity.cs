using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedEntity : MonoBehaviour {

    [SerializeField] Text entityNameText;
    [SerializeField] Text statisticsText;
    [SerializeField] Text ErrorText;
    [SerializeField] Button PlayButton;
    [SerializeField] Button deleteEntityButton;

    int entityID = 0;

    private void Awake()
    {
        Mediator.Instance.Subscribe<EntityButtonClickCmd>(OnEntityButtonClickCmd);
    }

    private void Update()
    {
        if(entityID >0)
        {
            PlayButton.interactable = true;
            deleteEntityButton.interactable = true;
        }
        else
        {
            PlayButton.interactable = false;
            deleteEntityButton.interactable = false;
        }
    }

    void OnEntityButtonClickCmd(EntityButtonClickCmd cmd)
    {
        OnEntityClick(cmd.Button);
    }

    public void ToRegisterMenu()
    {
        Mediator.Instance.DeleteSubscriber<EntityButtonClickCmd>(OnEntityButtonClickCmd);
        UnityEngine.SceneManagement.SceneManager.LoadScene("RegisterScene");
    }

    public void Play()
    {
        Mediator.Instance.DeleteSubscriber<EntityButtonClickCmd>(OnEntityButtonClickCmd);
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    void OnEntityClick(Button button)
    {
        ErrorText.text = "";
        string entityName = button.GetComponentInChildren<Text>().text;
        entityNameText.text = "Selected entity: ";
        entityNameText.text += entityName;

        entityID = DataBaseConnection.Instance.GetEntityID(entityName);

        if (entityID == -1)
        {
            Debug.Log("database interrupt");
            ErrorText.text = "database interrupt";
        }
        else
        {
            List<string> entityStatistics = new List<string>();
            //DataBaseConnection.Instance.GetStatistics(entityID, ref entityStatistics);
            DataBaseConnection.Instance.GetTotalityStatistics(entityID, ref entityStatistics);
            if (entityStatistics.Count == 0)
            {
                Debug.LogError("No statistics?");
            }
            else
            {
                statisticsText.text = "Statistics:\r\n ";
                foreach (var s in entityStatistics)
                {
                    statisticsText.text += s + "\r\n";
                }

                DataBaseConnection.Instance.CurrentEntityID = entityID;
            }
        }
    }

    public void DeleteSelectedEntity()
    {
        if(DataBaseConnection.Instance.DeleteEntity(entityID) == -1)
        {
            ErrorText.text = "database interrupt";
        }
        else
        {
            entityID = -1;
            ErrorText.text = "entity deleted";
            var cmd = new RefreshListCmd();
            Mediator.Instance.Publish<RefreshListCmd>(cmd);
        }
    }

    public void DeleteAccount()
    {
        if (DataBaseConnection.Instance.DeleteAccount() != -1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            

            ErrorText.text = "account deleted. Wait to leaving waiting room...";
            StartCoroutine(AccountDeleted());
        }
        else
        {
            ErrorText.text = "DB interrupt";
        }
    }

    IEnumerator AccountDeleted()
    {
        yield return new WaitForSeconds(2);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ToRegisterMenu();
    }
}
