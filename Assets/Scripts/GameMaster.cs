﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    private static GameMaster gm;

    private int[,] board;
    private int upperBound = 0;
    private int lowerBound = 0;

    private int size;

    private int offset;
    private List<GamePiece> gps;
    
    public GameObject pfSnakeHead;
    public GameObject pfWall;

    public int gameStepsPerTurn = 8;
    private int gameStep = 0;

    public Text scoreText;
    private int score;

    public GameObject gameOverOverlay;
    public bool gameOver = false;
    private bool newGame = true;

    private Apple a;

    void Start()
    {

        GetInstance().NewSnake();

        size = PlayerPrefs.GetInt("boardsize");
        board = new int[size, size];

        string output = "";
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                output += i + " " + j + ", ";
            }
            output += "\n";
        }
        Debug.Log(output);

        offset = size / 2;
        bool odd = size % 2 != 0;
        int wallOffset = offset + 1;

        lowerBound = -wallOffset;
        upperBound = wallOffset - (odd ? 0 : 1);
        //spawn all walls
        for (int i = -wallOffset; i < wallOffset + (odd ? 1: 0); i++)
        {
            Instantiate(pfWall, new Vector2(i, lowerBound), Quaternion.identity);
            Instantiate(pfWall, new Vector2(lowerBound, i), Quaternion.identity);
            Instantiate(pfWall, new Vector2(i, upperBound), Quaternion.identity);
            Instantiate(pfWall, new Vector2(upperBound, i), Quaternion.identity);
        }

        a = FindObjectOfType<Apple>();
    }

    public void NewSnake()
    {
        GameObject go = Instantiate(pfSnakeHead);
        if (PlayerPrefs.GetInt("isbot") == 1)
        {
            go.AddComponent<RoboSnake>();
        }
        else
        {
            go.AddComponent<HumanSnake>();
        }
    }

    void FixedUpdate()
    {
        gameStep++;
        //If game is over reset board
        if (gameStep % gameStepsPerTurn == 0)
        {
            if (!gameOver)
            {

                gps = new List<GamePiece>(FindObjectsOfType<GamePiece>());

                //We need to eval the step before to know the result from collisions in the turn before
                if (!newGame)
                {
                    foreach (GamePiece gp in gps)
                    {
                        gp.PostGameStep();
                    }

                    string output = "";
                    for (int i = 0; i < board.GetLength(0); i++)
                    {
                        for (int j = 0; j < board.GetLength(1); j++)
                        {
                            output += board[j, i];
                        }
                        output += "\n";
                    }
                    Debug.Log(output);
                }
                else
                {
                    newGame = false; 
                    foreach (GamePiece gp in gps)
                    {
                        gp.GameStart();
                    }
                }

                //Setup for movement
                foreach (GamePiece gp in gps)
                {
                    gp.PreGameStep();
                }

                //Reset virtual board
                board = new int[size, size];
                Vector2 v2apple = AdjustPosToBoard((Vector2)a.transform.position);
                board[(int)v2apple.x, (int)v2apple.y] = 2;
                //Move
                foreach (GamePiece gp in gps)
                {
                    gp.GameStep();
                }
            }
            else
            {
                GameOver();
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

    public static void AddScore(int points)
    {
        GetInstance().score += points;
        GetInstance().scoreText.text = "Score: " + GetInstance().score.ToString();
    }

    public static void GameOver()
    {
        //We need to eval the step before to know the result from collisions in the turn before
        //One last post
        foreach (GamePiece gp in GetInstance().gps)
        {
            gp.PostGameStep();
        }

        foreach (GamePiece gp in GetInstance().gps)
        {
            gp.GameOver();
        }

        GetInstance().gameOver = true;
        if (PlayerPrefs.GetInt("isbot") == 0)
        {
            GetInstance().gameOverOverlay.SetActive(true);
        }

        GetInstance().gameOver = false;
        GetInstance().newGame = true;

        GetInstance().NewSnake();
    }

    public static Vector2 GetRandomOpenSpace()
    {
        GameMaster mygm = GetInstance();
        Vector2 v2Rng = AdjustBoardToPos(new Vector2(Random.Range(0, mygm.size), Random.Range(0, mygm.size)));
        return v2Rng;
    }

    public static void Register(SnakeBody sb)
    {
        GameMaster mygm = GetInstance();
        do
        {
            Vector2 sbRealPos = (sb.transform.position);
            if (mygm.CheckBoundry(sbRealPos) != 1)
            {
                Vector2 sbPos = AdjustPosToBoard(sb.transform.position);
                mygm.board[(int)sbPos.x, (int)sbPos.y] = 1;
            }
            sb = sb.back;
        } while (sb != null);
    }

    public static int[] GetSight(SnakeController sc)
    {
        GameMaster mygm = GetInstance();

        int[] sight = new int[5];

        int dir = (int)sc.compas;
        Vector2 v2 = sc.transform.position;
        Vector2 v2a = (Vector2)mygm.a.transform.position - v2;
        
        if (dir == 0 || dir == 2)
        {
            if (((int)v2a.y < 0 && dir == 2) || ((int)v2a.y > 0 && dir == 0))
            {
                v2a.y = 2;
            } else
            {
                v2a.y = -2;
            }

            if ((int)v2a.x < 0)
            {
                v2a.x = dir == 0 ? -1: 1;
            } else if ((int)v2a.x > 0)
            {
                v2a.x = dir == 0 ? 1: -1;
            } else
            {
                v2a.x = 0;
            }
        } else
        {
            if (((int)v2a.x < 0 && dir == 3) || ((int)v2a.x > 0 && dir == 1))
            {
                v2a.x = 2;
            }
            else
            {
                v2a.x = -2;
            }

            if ((int)v2a.y < 0)
            {
                v2a.y = dir == 3 ? -1 : 1;
            }
            else if ((int)v2a.y > 0)
            {
                v2a.y = dir == 3 ? 1 : -1;
            }
            else
            {
                v2a.y = 0;
            }
        }

        Vector2[] v2Possible = new Vector2[4];
        v2Possible[0] = v2 + new Vector2(0, 1);
        v2Possible[1] = v2 + new Vector2(1, 0);
        v2Possible[2] = v2 + new Vector2(0, -1);
        v2Possible[3] = v2 + new Vector2(-1, 0);

        Debug.Log(v2Possible[SnakeController.Mod(dir - 1, 4)].ToString() + ", " + v2Possible[SnakeController.Mod(dir, 4)].ToString() + ", " + v2Possible[SnakeController.Mod(dir + 1, 4)].ToString());

        sight[0] = mygm.CheckSurrounding(v2Possible[SnakeController.Mod(dir - 1, 4)]);
        sight[1] = mygm.CheckSurrounding(v2Possible[SnakeController.Mod(dir, 4)]);
        sight[2] = mygm.CheckSurrounding(v2Possible[SnakeController.Mod(dir + 1, 4)]);
        sight[3] = (int)v2a.x;
        sight[4] = (int)v2a.y;

        //robot sight
        return sight;
    }

    private int CheckBoundry(Vector2 v2Possible)
    {
        v2Possible = AdjustPosToBoard(v2Possible);
        if (v2Possible.x < size && v2Possible.y < size && v2Possible.x >= 0 && v2Possible.y >= 0)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
    private int CheckSurrounding(Vector2 v2Possible)
    {
        Vector2 v2Pos = AdjustPosToBoard(v2Possible);
        if (CheckBoundry(v2Possible) == 0)
        {
            return GetInstance().board[(int)v2Pos.x, (int)v2Pos.y];
        }
        else
        {
            return 1;
        }
    }

    public static Vector2 AdjustPosToBoard(Vector2 realPos)
    {
        return realPos + new Vector2(1, 1) * GetInstance().offset;
    }
    public static Vector2 AdjustBoardToPos(Vector2 boardPos)
    {
        return boardPos - new Vector2(1, 1) * GetInstance().offset;
    }
}
