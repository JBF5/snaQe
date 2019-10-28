using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class qBot
{
    private List<float[]> QStates;
    private List<float[]> QActions;

    private int delcaredActions;
    private float[] initialState;
    private int initialActionIndex;
    private float[] outcomeState;
    private float outcomeActionValue;

    private float lr;
    private float y;
    private float SimInterval;

    private bool firstIteration;
    System.Random rn = new System.Random();

    public qBot(int actions)
    {
        QStates = new List<float[]>();
        QActions = new List<float[]>();
        delcaredActions = actions;

        lr = .8f;
        y = .95f;

        SimInterval = 1f;

        firstIteration = true;
    }


    public static int ProcessSight(float[] sight, float reward)
    {
       



        return 0;
    }

}