using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeEngine : MonoBehaviour {

    [SerializeField] GameObject papyrus;
    [SerializeField] GameObject container;
    [SerializeField] GameObject buttonHomeScrollPanelPrefab;


    public void Return()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void YourWeapons()
    {
        ClearScrollPanelList();
        List<string> weaponsName = new List<string>();
        DataBaseConnection.Instance.GetEntityWeapons(ref weaponsName);

        AddButtonsToScrollPanel(ref weaponsName);
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

    void AddButtonsToScrollPanel(ref List<string> weapons)
    {
        foreach (var item in weapons)
        {
            GameObject go = Instantiate(buttonHomeScrollPanelPrefab.gameObject, container.transform);
            go.GetComponentInChildren<Text>().text = item;
            go.GetComponent<Button>().onClick.AddListener(() => ShowText(item));
        }

    }

    void ShowText(string name)
    {
        papyrus.SetActive(true);
        papyrus.GetComponentInChildren<Text>().text = name + "\r\n";

        int weaponID = DataBaseConnection.Instance.GetWeaponID(name);
        int weaponHealth = DataBaseConnection.Instance.GetWeaponHealth(weaponID);
        int weaponPower = DataBaseConnection.Instance.GetWeaponPower(weaponID);

        papyrus.GetComponentInChildren<Text>().text += "Power: " + weaponPower.ToString() + "\r\nHealth: "+ weaponHealth.ToString();
    }

    public void YourStats()
    {
        papyrus.SetActive(true);
        papyrus.GetComponentInChildren<Text>().text = DataBaseConnection.Instance.GetEntityName() + "\r\n";
        papyrus.GetComponentInChildren<Text>().text += "Power: " + DataBaseConnection.Instance.GetEntityPower().ToString() + "\r\n";
        papyrus.GetComponentInChildren<Text>().text += "Health: " + DataBaseConnection.Instance.GetEntityHealth().ToString() + "\r\n";

        papyrus.GetComponentInChildren<Text>().text += "Weapons power: " + DataBaseConnection.Instance.GetEntityWeaponsPower() + "\r\n";
        papyrus.GetComponentInChildren<Text>().text += "Weapons health: " + DataBaseConnection.Instance.GetEntityWeaponsHealth() + "\r\n";
    }
}
