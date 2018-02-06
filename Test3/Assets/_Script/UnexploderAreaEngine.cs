using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnexploderAreaEngine : MonoBehaviour {

    [SerializeField] GameObject papyrus;
    [SerializeField] Button fightButton;
    [SerializeField] Button returnButton;

    [SerializeField] int timeToUnlockButtonWhenLose = 6;
    [SerializeField] int timeToUnlockButtonWhenWin = 3;

    public void Return()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void Fight()
    {
        papyrus.SetActive(true);
        SpotEnemy();
    }

    void SpotEnemy()
    {
        int monsterID = DataBaseConnection.Instance.GetRandomEnemyID();
        if(monsterID != -1)
        {            
            int quantity = Random.Range(1, 6);
            papyrus.GetComponentInChildren<Text>().text = "You spotted " + quantity + "x " +  DataBaseConnection.Instance.GetMonsterName(monsterID).ToString() + "\r\n";

            int monsterHealth = DataBaseConnection.Instance.GetMonsterHealth(monsterID);
            int monsterPower = DataBaseConnection.Instance.GetMonsterPower(monsterID);
            int entityPower = DataBaseConnection.Instance.GetEntityTotalityPower();
            int entityHealth = DataBaseConnection.Instance.GetEntityTotalityHealth();
            double exp = DataBaseConnection.Instance.GetMonsterExp(monsterID);

            if (monsterHealth != -1 || monsterPower != -1 || entityPower!= -1 || entityHealth !=-1 || exp != -1)
            {
                double entityStats = entityPower * entityHealth;
                double enemyStats = monsterHealth * monsterPower;

                if(entityStats > enemyStats)
                {
                    papyrus.GetComponentInChildren<Text>().text += "You win!";
                    papyrus.GetComponentInChildren<Text>().text += "\r\n";
                    double totalityExp = exp * quantity;                    
                    if(DataBaseConnection.Instance.AddExp(totalityExp) == -1)
                    {
                        papyrus.GetComponentInChildren<Text>().text = "DB interrupt";
                    }
                    else
                    {
                        papyrus.GetComponentInChildren<Text>().text += "You gain " + totalityExp + " exp!";
                        BlocktButtons(3);
                    }
                }
                else
                {
                    papyrus.GetComponentInChildren<Text>().text += "You lose!";
                    BlocktButtons(5);
                }
            }
            else
            {
                papyrus.GetComponentInChildren<Text>().text = "DB interrupt";
            }
        }
        else
        {
            papyrus.GetComponentInChildren<Text>().text = "DB interrupt";
        }
    }

    void BlocktButtons(int timeToUnlock)
    {
        fightButton.interactable = false;
        returnButton.interactable = false;
        StartCoroutine(UnlockButtons(timeToUnlock));
    }

    IEnumerator UnlockButtons(int time)
    {
        while(time >=0)
        {
            yield return new WaitForSeconds(1);
            ChangeButtonsText(time);
            time--;
        }

        fightButton.GetComponentInChildren<Text>().text = "Fight";
        returnButton.GetComponentInChildren<Text>().text = "Return";

        fightButton.interactable = true;
        returnButton.interactable = true;
    }

    void ChangeButtonsText(int time)
    {
        fightButton.GetComponentInChildren<Text>().text = "Time to unlock: " + time;
        returnButton.GetComponentInChildren<Text>().text = "Time to unlock: " + time;
    }


}
