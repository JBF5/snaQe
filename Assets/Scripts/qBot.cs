using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class qBot
{
    System.Random rn = new System.Random();
    Dictionary<string, float[]> qTable;
    int actionSize = 3;
    private float lr;
    private float y;
    private float eps;
    private float epsDec;
    private int[] previousSight;
    private string prevSight;
    private int action = -1;

    private static qBot qb;

    private qBot()
    {
        qTable = new Dictionary<string, float[]>();
        lr = .8f;
        y = .95f;
        eps = .999f;
        epsDec = .001f;
    }

    public static qBot GetInstance()
    {
        if (qb == null)
        {
            qb = new qBot();
        }
        return qb;
    }

    public int ProcessSight(int[] sight)
    {
        previousSight = sight;
        prevSight = Stringify(sight);


        if (qTable.ContainsKey(prevSight)) //If action exists
        {
            float maxF = float.MinValue;
            for(int i = 0; i < qTable[prevSight].Length; i++)
            {
                if(qTable[prevSight][i] > maxF)
                {
                    maxF = qTable[prevSight][i];
                    action = i;
                }
            }
        }
        else  //No action exists
        {
            qTable.Add(prevSight, new float[actionSize]);
            //Takes a random action
            action = rn.Next(0,actionSize);
        }
        if(eps > rn.NextDouble())  //Takes a random action
        {
            action = rn.Next(0, actionSize);
        }

        eps = eps - epsDec;    //Slowly takes away randomness
        
        

        return action;
    }

    public void ProcessReward(int[] postSight, int reward)
    {
        string key = Stringify(postSight);
        if (!qTable.ContainsKey(key))
        {
            qTable.Add(key, new float[actionSize]);
        }

        //Finding the max value of the next sights 
        float maxPostF = float.MinValue;
        float maxPrevF = float.MinValue;

        for (int i = 0; i < qTable[key].Length; i++)
        {
            if (qTable[key][i] > maxPostF)
            {
                maxPostF = qTable[key][i];
            }
        }

        if (qTable.ContainsKey(prevSight))
        {
            //Max Q from last move
            maxPrevF = qTable[prevSight][action];

            //Setting the value for the place we moved from
            qTable[prevSight][action] = maxPrevF + lr * (reward + y * maxPostF - maxPrevF);
        }
        else
        {
            maxPrevF = 0f;
            qTable[prevSight][action] = maxPrevF + lr * (reward + y * maxPostF - maxPrevF);
        }

        Debug.Log("qscore " + qTable[prevSight][action]);
    }

    private static string Stringify(int[] sight)
    {
        string built = "";
        foreach (int i in sight)
        {
            built += i + ",";
        }
        return built;
    }
}
