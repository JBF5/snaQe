using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboSnake : GamePiece
{
    private SnakeController sc;

    public override void GameStep()
    {
        //feed robot
        float[] sight = GameMaster.GetSight(sc);

        //robot proccess
        //int dirTurn = qBot.ProcessSight(sight);

        //robot output
        sc.TurnTo(0);
    }
}
