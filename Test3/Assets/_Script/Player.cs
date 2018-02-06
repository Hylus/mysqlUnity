using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : MonoBehaviour {


    //public static int firstLevelExp = 100;

    public static double ExpToNextLevel (int lvl)
    {
        double neededExp = (lvl + 1) * 100;
        return neededExp;
    }


}
