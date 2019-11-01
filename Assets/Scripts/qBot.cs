using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class qBot
{
    System.Random rn = new System.Random();
    Dictionary<int[], float[]> qTable;
    int actionSize = 3;
    private float lr;
    private float y;
    private int[] previousSight;
    private int action = -1;


    public qBot()
    {
        qTable = new Dictionary<int[], float[]>();
        lr = .8f;
        y = .95f;
    }

    public int ProcessSight(int[] sight)
    {
        previousSight = sight;
        
        if (qTable.ContainsKey(sight)) //If action exists
        {
            float maxF = float.MinValue;
            for(int i = 0; i < qTable[sight].Length; i++)
            {
                if(qTable[sight][i] > maxF)
                {
                    maxF = qTable[sight][i];
                    action = i;
                }
            }
        }
        else  //No action exists
        {
            qTable.Add(sight, new float[actionSize]);
            //Takes a random action
            action = rn.Next(0,actionSize);
        }
        return action;
    }

    public void ProcessReward(int[] postSight, int reward)
    {
        //Finding the max value of the next sights 
        float maxPostF = float.MinValue;
        float maxPrevF = float.MinValue;

        for (int i = 0; i < qTable[postSight].Length; i++)
        {
            if (qTable[postSight][i] > maxPostF)
            {
                maxPostF = qTable[postSight][i];
            }
        }

        if (qTable.ContainsKey(previousSight))
        {
            //Max Q from last move
            maxPrevF = qTable[previousSight][action];

            //Setting the value for the place we moved from
            qTable[previousSight][action] = maxPrevF + lr * (reward + y * maxPostF - maxPrevF);
        }
        else
        {
            maxPrevF = 0f;
            qTable[previousSight][action] = maxPrevF + lr * (reward + y * maxPostF - maxPrevF);
        }       
    }

}
