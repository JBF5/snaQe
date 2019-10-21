using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : GamePiece
{
    public SnakeBody sbSnakeHead;
    private int length = 1;
    private SnakeBody sbSnakeTail;

    public GameObject goBodyPrefab;

    private Turning dir = Turning.FORWARD;
    private Compas compas = Compas.NORTH;

    private Vector2 dest;

    // Start is called before the first frame update
    void Start()
    {
        sbSnakeTail = sbSnakeHead;
        dest = transform.position;
    }

    public override void GameStep()
    {
        GameMaster.Register(sbSnakeHead);

        switch (dir)
        {
            case Turning.FORWARD:
                break;
            case Turning.LEFT:
                Debug.Log("Turning L");
                compas = (Compas)Mod((int)compas - 1, 4);
                break;
            case Turning.RIGHT:
                Debug.Log("Turning R");
                compas = (Compas)Mod((int)compas + 1, 4);
                break;
        }
        dir = Turning.FORWARD;

        switch (compas)
        {
            case Compas.NORTH:
                dest.y += 1;
                break;
            case Compas.EAST:
                dest.x += 1;
                break;
            case Compas.SOUTH:
                dest.y -= 1;
                break;
            case Compas.WEST:
                dest.x -= 1;
                break;
        }

        sbSnakeHead.MoveTo(dest);
    }

    public void TurnTo(Compas c)
    {
        int dirDiff = Mod(c - compas, 4);
        if (dirDiff == 1)
        {
            dir = Turning.RIGHT;
        }
        else if (dirDiff == 3)
        {
            dir = Turning.LEFT;
        }
    }

    public void TurnTo(int turn)
    {
        //1 turns left, -1 turns
        if (turn == 1)
        {
            dir = Turning.LEFT;
        }
        else if (turn == -1)
        {
            dir = Turning.RIGHT;
        }
    }

    public void Eat()
    {
        GameObject go = Instantiate(goBodyPrefab, sbSnakeTail.transform.position, Quaternion.identity);
        SnakeBody sb = go.GetComponent<SnakeBody>();

        GameMaster.AddBody(sb);
        GameMaster.AddScore(1);

        if (length == 1)
        {
            sbSnakeHead.back = sb;
            sb.front = sbSnakeHead;
            sbSnakeTail = sb;
        } else
        {
            sbSnakeTail.back = sb;
            sb.front = sbSnakeTail;
            sbSnakeTail = sb;
        }

        length++;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Wall>() != null)
        {
            Debug.Log("Dead");
            GameMaster.GameOver();
        } else if (collision.gameObject.GetComponent<Wall>() != null)
        {
            Debug.Log("Apple");
        }
    }

    public enum Turning {FORWARD, LEFT, RIGHT};
    public enum Compas {NORTH, EAST, SOUTH, WEST};

    private int Mod(int num, int div)
    {
        int rem = num % div;
        return rem < 0 ? rem + div : rem;
    }

}
