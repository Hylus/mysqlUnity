using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] bool debugMode;

    int loginID;
    int raceID;

    private void Awake()
    {
        if(debugMode)
        {
            Debug.Log("Debug mode is on, your loginID is 1");
        }
        loginID = (debugMode) ? 1 : GameObject.Find("DataBase").GetComponent<DataBaseConnection>().CurrentLoginID;
        
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
