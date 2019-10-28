using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboSnake : GamePiece
{
    private SnakeController sc;
    float reward = 0f;


    private void Start()
    {
        sc = GetComponent<SnakeController>();
    }

    public override void GameStep()
    {
        //feed robot
        float[] sight = GameMaster.GetSight(sc);
        

        //robot proccess
        //int dirTurn = qBot.ProcessSight(sight, reward);

        //robot output
        sc.TurnTo(0);
    }

}
