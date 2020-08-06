using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Waypoints wp;
    int currentWP = 0;

    bool move = true;

    float timeElapsed = 0;
    float lerpDuration = 2;
    Vector3 startVector;
    Vector3 goalVector;
    
    void Awake()
    {
        startVector = wp.GetStartVector();
        transform.position = startVector;
        currentWP++;
        goalVector = wp.GetNextVector(currentWP);
    }



    void Update()
    {
        if(move)
        {
            if (timeElapsed < lerpDuration)
            {
                transform.position = Vector3.Lerp(startVector, goalVector, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
            }
            else
            {
                timeElapsed = 0;
                if (currentWP < wp.WPList.Count - 1)
                {
                    currentWP++;
                    NextVector();
                }
                else if (wp.cycleWP)
                {
                    currentWP = 0;
                    NextVector();
                }
                else
                {
                    move = false;
                }

            }
        }
        
    }


    void NextVector()
    {
        startVector = goalVector;
        goalVector = wp.GetNextVector(currentWP);
        
    }
}
