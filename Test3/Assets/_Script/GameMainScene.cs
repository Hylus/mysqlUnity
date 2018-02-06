using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMainScene : MonoBehaviour {

    public void GoToPub()
    {
        SceneManager.LoadScene("Pub");
    }
/*
    public void GoToMountain()
    {
        SceneManager.LoadScene("Mountain");
    }
    */
    public void GoToUnexploredArea()
    {
        SceneManager.LoadScene("Unexplored area");
    }

    public void GoToHome()
    {
        SceneManager.LoadScene("Home");
    }

    public void Logout()
    {
        SceneManager.LoadScene("WaitingRoom");
    }

}
