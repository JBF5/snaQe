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
        sc = GetComponent<SnakeController>();
        qb = new qBot();
    }

    

    public override void PreGameStep()
    {
        //feed robot
        int[] sight = GameMaster.GetSight(sc);

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
        //Get score from moving
        int reward = sc.GetMoveScore();
        //Process reward for moving
        qb.ProcessReward(postSight, reward);
    }

    public override void GameOver()
    {
       

    }
}
