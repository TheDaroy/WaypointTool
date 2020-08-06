using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Waypoints : ScriptableObject
{
    
    public List<Vector3> WPList;
    public bool cycleWP = false;

    public Vector3 GetStartVector()
    {
        return WPList[0];
    }
    public Vector3 GetNextVector(int currentWP)
    {
        return WPList[currentWP];
    }


}
