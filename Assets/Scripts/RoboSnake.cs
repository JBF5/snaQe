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

    public override void PreGameStep()
    {
        //feed robot
        float[] sight = GameMaster.GetSight(sc);
    }

    public override void GameStep()
    {
        //robot proccess
        //int dirTurn = qBot.ProcessSight(sight, reward);

        //robot output
        sc.TurnTo(0);
    }

    public override void PostGameStep()
    {
        int reward = sc.GetMoveScore();
    }

    public override void GameOver()
    {

    }
}
