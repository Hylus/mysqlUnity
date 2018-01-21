using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    int loginID;
    int raceID;

    private void Awake()
    {

        //loginID = (debugMode) ? 1 : DataBaseConnection.Instance.CurrentLoginID;
        
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
