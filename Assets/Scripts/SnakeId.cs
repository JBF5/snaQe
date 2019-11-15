using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeId
{
    private static SnakeId snake;
    private int idplayer;

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
}
