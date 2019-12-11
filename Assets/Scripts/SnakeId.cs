using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeId
{
    private static SnakeId snake;
    private int idplayer;
    private int intHighScore = 0;
    private int intAttempt = 0;
    private string strName;

    private SnakeId()
    {

    }

    public static SnakeId GetInstance()
    {
        if (snake == null)
        {
            snake = new SnakeId();
        }
        return snake;
    }

    public void SetSnakeId(int idplayer)
    {
        this.idplayer = idplayer;
    }
    public int GetSnakeId()
    {
        return this.idplayer;
    }

    public void SetHighScore(int intHighScore)
    {
        if (this.intHighScore < intHighScore)
        {
            this.intHighScore = intHighScore;
        }
    }

    public int GetHighScore()
    {
        return this.intHighScore;
    }

    public void SetName(string strName)
    {
        this.strName = strName;
    }

    public string GetName()
    {
        return this.strName;
    }

    public void NewAttempt()
    {
        this.intAttempt++;
    }
    public int GetAttempt()
    {
        return this.intAttempt;
    }
}
