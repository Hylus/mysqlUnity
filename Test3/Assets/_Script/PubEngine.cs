using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PubEngine : MonoBehaviour {

    [SerializeField] GameObject container;
    [SerializeField] Button buttonScrollPanelPrefab;
    [SerializeField] GameObject papyrus;
    [SerializeField] Button fightButton;
    [SerializeField] GameObject resultImage;

    List<string> _monsters = new List<string>();
    int _currentAdventure;

    public void FindAdventures()
    {
        resultImage.SetActive(false);
        papyrus.SetActive(false);
        ClearScrollPanelList();
        ShowUnfinishedAdventures();
        //fightButton.interactable = true;
    }

    void ClearScrollPanelList()
    {
        Button[] buttons = new Button[container.GetComponentsInChildren<Button>().Length];
        buttons = container.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            Destroy(buttons[i].gameObject);
        }
    }

    void ShowUnfinishedAdventures()
    {
        List<string> advTitles = new List<string>();
        DataBaseConnection.Instance.GetUnfinishedAdventureTitle(ref advTitles);

        bool finishAdvClicked = false;
        AddButtonsToScrollPanel(ref advTitles, finishAdvClicked);
    }

    void ShowFinishedAdventures()
    {
        List<string> advTitles = new List<string>();
        DataBaseConnection.Instance.GetFinishedAdventureTitle(ref advTitles);
        bool finishAdvClicked = true;
        AddButtonsToScrollPanel(ref advTitles, finishAdvClicked);
    }

    void AddButtonsToScrollPanel(ref List<string> advTitles, bool finishAdvClicked)
    {
        foreach (var item in advTitles)
        {
            GameObject go = Instantiate(buttonScrollPanelPrefab.gameObject, container.transform);
            go.GetComponentInChildren<Text>().text = item;
            go.GetComponent<Button>().onClick.AddListener(()=> ShowText(item, finishAdvClicked));
        }
        
    }

    void ShowText(string title, bool finishAdvClicked)
    {
        _currentAdventure = DataBaseConnection.Instance.GetAdventureID(title);
        if (_currentAdventure != -1)
        {
            if (!papyrus.activeInHierarchy)
            {
                papyrus.SetActive(true);
            }
            Text text = papyrus.GetComponentInChildren<Text>();
            text.text = title + "\r\n";
            text.text += DataBaseConnection.Instance.GetAdventureDescripton(title);
            text.text += "\r\n\r\n";

            if (!finishAdvClicked)
            {
                ShowRequiredLevel(ref text, title);
                text.text += "\r\n";
                text.text += "Your current level: " + DataBaseConnection.Instance.GetCurrentLevel();
                fightButton.GetComponentInChildren<Text>().text = "Carry out";
                if (CheckRequiredLevel(title))
                {
                    fightButton.interactable = true;
                }
                else
                {
                    fightButton.interactable = false;
                }
            }
            else
            {
                fightButton.GetComponentInChildren<Text>().text = "You finished this quest";
            }

            text.text += "\r\n";
            ShowPrize(ref text, title);
            text.text += "\r\n";
            ShowMonsters(ref text, title);
            text.text += "\r\n";

            papyrus.SetActive(true);
        }
    }

    bool CheckRequiredLevel(string title)
    {
        int requiredLevel = DataBaseConnection.Instance.GetAdventureRequiredLevel(title);
        int currentLevel = DataBaseConnection.Instance.GetCurrentLevel();

        if(requiredLevel <= currentLevel)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void ShowRequiredLevel(ref Text text, string title)
    {
        text.text += "Required level: ";
        int level = DataBaseConnection.Instance.GetAdventureRequiredLevel(title);
        if(level == -1)
        {
            text.text += "DB error";
        }
        else
        {               
            text.text += level.ToString();
        }
    }

    void ShowPrize(ref Text text, string title)
    {
        text.text += "Prize: ";
        string prize = DataBaseConnection.Instance.GetAdventurePrize(title);
        text.text += prize;
    }

    void ShowMonsters(ref Text text, string title)
    {
        text.text += "Monsters: ";
       
        DataBaseConnection.Instance.GetAdventureMonsters(ref _monsters, title);
        foreach (var item in _monsters)
        {
            text.text += item + "\r\n";
        }
    }

    public void FinishedAdventure()
    {
        resultImage.SetActive(false);
        papyrus.SetActive(false);
        fightButton.interactable = false;
        ClearScrollPanelList();
        ShowFinishedAdventures();
    }

    public void CarryOutTask()
    {       
        papyrus.SetActive(false);
        resultImage.SetActive(true);
        resultImage.GetComponentInChildren<Text>().text = "Fighting...";
        double result = Fight();

        if (result == -1)
        {
            Debug.Log("DB error");
            resultImage.GetComponentInChildren<Text>().text = "DB interrupt";

        }
        else if( result == 0)
        {
            Debug.Log("You lose");
            resultImage.GetComponentInChildren<Text>().text = "You lose";
        }
        else
        {
            resultImage.GetComponentInChildren<Text>().text = "You win\r\nYou gain "+ result + " exp";
            if(GainExp(result))
            {
                int prizeID = DataBaseConnection.Instance.GetAdventurePrize(_currentAdventure);
                if(DataBaseConnection.Instance.AddWeapon(prizeID) == 1)
                {
                    if(DataBaseConnection.Instance.SetFinishedAdventage(_currentAdventure) == 1)
                    {
                        ClearScrollPanelList();
                    }
                }
            }
        }
    }

    bool GainExp(double result)
    {
        if(DataBaseConnection.Instance.AddExp(result) == -1)
        {
            resultImage.GetComponentInChildren<Text>().text = "DB interrupt. Progress are not saving";
            return false;
        }
        return true;
    }

    double Fight()
    {
        int monstersPower =0;
        int monstersHealth =0;
        double monstersExp = 0;

        int entityHealth = DataBaseConnection.Instance.GetEntityHealth();
        int entityPower = DataBaseConnection.Instance.GetEntityPower();

        if(entityHealth == -1 || entityPower== -1)
        {
            return -1;
        }

        foreach (var item in _monsters)
        {
            int quantity = System.Int32.Parse(item.Substring(0, item.IndexOf("x")));
            string monster = item.Substring(item.IndexOf(" ")+1);

            int health = quantity * DataBaseConnection.Instance.GetMonsterHealth(monster);
            int power = quantity * DataBaseConnection.Instance.GetMonsterPower(monster);  
            double exp = quantity * DataBaseConnection.Instance.GetMonsterExp(monster);

            monstersHealth += health;
            monstersPower += power;
            monstersExp += exp;

            if(health == -1 || power == -1 || exp == -1)
            {
                return -1;
            }
        }
        
        double entityStats = DataBaseConnection.Instance.GetEntityTotalityHealth() * DataBaseConnection.Instance.GetEntityTotalityPower();
        double monsterStats = monstersHealth * monstersPower;

        Debug.Log("entityStats " + entityStats + " vs " + "monsterStats " + monsterStats);

        if (entityStats > monsterStats)
        {
            int bonusAdventureExp = DataBaseConnection.Instance.GetAdventureExp(_currentAdventure);
            if(bonusAdventureExp == -1)
            {
                return -1;
            }
            return monstersExp + bonusAdventureExp;
        }
        else
        {
            return 0;
        }        
    }

    public void Return()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
