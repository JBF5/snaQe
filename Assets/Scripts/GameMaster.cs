using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    private static GameMaster gm;

    private bool[,] board;
    public int size = 5;
    private int offset;
    private List<GamePiece> gps;
    private List<SnakeBody> sbs;

    public SnakeBody sbHead;
    public GameObject pfWall;

    public int gameStepsPerTurn = 8;
    private int gameStep = 0;

    public Text scoreText;
    private int score;

    public GameObject gameOverOverlay;
    private bool gameOver = false;

    void Start()
    {
        gps = new List<GamePiece>(FindObjectsOfType<GamePiece>());
        sbs = new List<SnakeBody>(FindObjectsOfType<SnakeBody>());

        board = new bool[size, size];
        offset = size / 2;
        bool odd = size % 2 != 0;
        int wallOffset = offset + 1;
        //spawn all walls
        for (int i = -wallOffset; i < wallOffset + (odd ? 1: 0); i++)
        {
            Instantiate(pfWall, new Vector2(i, -wallOffset), Quaternion.identity);
            Instantiate(pfWall, new Vector2(-wallOffset, i), Quaternion.identity);
            Instantiate(pfWall, new Vector2(i, wallOffset - (odd ? 0 : 1)), Quaternion.identity);
            Instantiate(pfWall, new Vector2(wallOffset - (odd ? 0 : 1), i), Quaternion.identity);
        }
    }

    void FixedUpdate()
    {
        gameStep++;
        if (gameStep % gameStepsPerTurn == 0 && !gameOver)
        {
            //Reset board
            board = new bool[size, size];

            //Build board
            foreach (GamePiece gp in gps)
            {
                gp.GameStep();
            }

        }


    }

    public static GameMaster GetInstance()
    {
        if (gm == null)
        {
            gm = FindObjectOfType<GameMaster>();
        }
        return gm;
    }

    public static void AddPiece(GamePiece gp)
    {
        GetInstance().gps.Add(gp);
    }
    public static void AddBody(SnakeBody sb)
    {
        GetInstance().sbs.Add(sb);
    }

    public static void AddScore(int points)
    {
        GetInstance().score += points;
        GetInstance().scoreText.text = "Score: " + GetInstance().score.ToString();
    }

    public static void GameOver()
    {
        GetInstance().gameOver = true;
        GetInstance().gameOverOverlay.SetActive(true);
    }

    public static Vector2 GetRandomOpenSpace()
    {
        GameMaster mygm = GetInstance();
        return new Vector2(Random.Range(0, mygm.size) - mygm.offset, Random.Range(0, mygm.size) - mygm.offset);
    }

    public static void Register(SnakeBody sb)
    {
        GameMaster mygm = GetInstance();
        do
        {
            Vector2 sbPos = sb.transform.position + new Vector3(mygm.offset, mygm.offset);
            mygm.board[(int)sbPos.x, (int)sbPos.y] = true;
            sb = sb.back;
        } while (sb != null);
    }
    public static float[] GetSight(SnakeController sc)
    {
        GameMaster mygm = GetInstance();
        //robot sight
        return null;
    }
}
