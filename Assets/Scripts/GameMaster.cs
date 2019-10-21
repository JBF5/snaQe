using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    private static GameMaster gm;

    private bool[][] board;
    private List<GamePiece> gps;
    private List<SnakeBody> sbs;

    public SnakeBody sbHead;

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
    }

    void FixedUpdate()
    {
        gameStep++;
        if (gameStep % gameStepsPerTurn == 0 && !gameOver)
        {
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
}
