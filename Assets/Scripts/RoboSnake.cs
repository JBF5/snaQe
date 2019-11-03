using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboSnake : GamePiece
{
    private SnakeController sc;
    private GameMaster gmr;
    private qBot qb;
    
    
   

    private void Start()
    {
        qb = qBot.GetInstance();
    }


    public override void GameStart()
    {
        sc = FindObjectOfType<SnakeController>();
    }

    public override void PreGameStep()
    {
        //feed robot
        int[] sight = GameMaster.GetSight(sc);
        Debug.Log("Pre " + sight[0] + ", " + sight[1] + ", " + sight[2]);

        //robot proccess
        int dirTurn = qb.ProcessSight(sight);

        //robot output
        sc.TurnTo(dirTurn);
    }

    public override void GameStep()
    {
        
    }

    public override void PostGameStep()
    {
        //Get sight after moving
        int[] postSight = GameMaster.GetSight(sc);
        Debug.Log("Post " + postSight[0] + ", " + postSight[1] + ", " + postSight[2]);
        //Get score from moving
        int reward = sc.GetMoveScore();
        //Process reward for moving
        Debug.Log("Reward " + reward);
        qb.ProcessReward(postSight, reward);
    }

    public override void GameOver()
    {
       

    }
}
