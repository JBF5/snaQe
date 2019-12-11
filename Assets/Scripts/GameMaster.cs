using System.Collections;
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
    public Text highscoreText;
    public Text nameText;
    public Slider gameSpeedSlider;
    private int score;
    private int highscore;

    public GameObject gameOverOverlay;
    public bool gameRunning = true;
    public bool gameOver = false;
    private bool newGame = true;
    private bool firstFrame = true;

    private Apple a;

    void Start()
    {

        GetInstance().NewSnake();

        size = PlayerPrefs.GetInt("boardsize");
        board = new int[size, size];

        offset = size / 2;
        bool odd = size % 2 != 0;
        int wallOffset = offset + 1;

        lowerBound = -wallOffset;
        upperBound = wallOffset - (odd ? 0 : 1);
        //spawn all walls
        for (int i = -wallOffset; i < wallOffset + (odd ? 1 : 0); i++)
        {
            Instantiate(pfWall, new Vector2(i, lowerBound), Quaternion.identity);
            Instantiate(pfWall, new Vector2(lowerBound, i), Quaternion.identity);
            Instantiate(pfWall, new Vector2(i, upperBound), Quaternion.identity);
            Instantiate(pfWall, new Vector2(upperBound, i), Quaternion.identity);
        }

        a = FindObjectOfType<Apple>();

        gameSpeedSlider.onValueChanged.AddListener(delegate { SetGameSpeed(); });
        if (PlayerPrefs.GetInt("isbot") == 0)
        {
            gameSpeedSlider.value = 8;
            gameSpeedSlider.enabled = false;
            gameStepsPerTurn = 8;
        }

        nameText.text = SnakeId.GetInstance().GetName();
        highscoreText.text = SnakeId.GetInstance().GetHighScore().ToString();
    }

    private void SetGameSpeed()
    {
        gameStepsPerTurn = (int)gameSpeedSlider.value;
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
        SnakeId.GetInstance().NewAttempt();
    }

    void FixedUpdate()
    {
        gameStep++;
        //If game is over reset board
        if (gameStep % gameStepsPerTurn == 0 && gameRunning)
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

                }
                else
                {
                    newGame = false;

                    a.MoveToNewSpace();
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


        if (firstFrame)
        {
            firstFrame = false;

            if (PlayerPrefs.GetInt("isbot") == 1)
            {
                gameSpeedSlider.value = 1;
                gameStepsPerTurn = 1;
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
        GetInstance().scoreText.text = GetInstance().score.ToString();
        GetInstance().highscoreText.text = SnakeId.GetInstance().GetHighScore().ToString();
    }

    public int idplayer;
    public static void SetPlayerId(int idplayer)
    {
        GetInstance().idplayer = idplayer;
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
            GetInstance().gameRunning = false;
        } else
        {
            GetInstance().gameOver = false;
            GetInstance().newGame = true;

            GetInstance().NewSnake();
        }

    }

    public static Vector2 GetRandomOpenSpace()
    {
        GameMaster mygm = GetInstance();
        Vector2 v2NewApple;
        do
        {
            v2NewApple = new Vector2(Random.Range(0, mygm.size), Random.Range(0, mygm.size));
        } while (GetInstance().CheckSurrounding(AdjustBoardToPos(v2NewApple)) != 0);
        return AdjustBoardToPos(v2NewApple);
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

        int[] sight = new int[0];

        int dir = (int)sc.compas;
        Vector2 v2 = sc.transform.position;
        Vector2 v2a = (Vector2)mygm.a.transform.position - v2;
        
        if (PlayerPrefs.GetInt("idvision") == 1)
        {
            //Checks for 3 directional positions (empty/wall/apple)
            sight = new int[3];
            Vector2[] v2Possible = new Vector2[4];

            //Add apple to board
            Vector2 v2apple = AdjustPosToBoard((Vector2)mygm.a.transform.position);
            mygm.board[(int)v2apple.x, (int)v2apple.y] = 2;

            v2Possible[0] = v2 + new Vector2(0, 1);
            v2Possible[1] = v2 + new Vector2(1, 0);
            v2Possible[2] = v2 + new Vector2(0, -1);
            v2Possible[3] = v2 + new Vector2(-1, 0);

            for (int i = 0, m = dir - 1; i < 3; i++, m++)
            {
                sight[i] = mygm.CheckSurrounding(v2Possible[SnakeController.Mod(m, 4)]);
                DebugDrawLine(sight[i], v2, v2Possible[SnakeController.Mod(m, 4)]);
            }
        }
        else if (PlayerPrefs.GetInt("idvision") == 2)
        {
            //Checks for 3 directional positions (empty/wall) & apple
            sight = new int[5];
            Vector2[] v2Possible = new Vector2[4];

            v2Possible[0] = v2 + new Vector2(0, 1);
            v2Possible[1] = v2 + new Vector2(1, 0);
            v2Possible[2] = v2 + new Vector2(0, -1);
            v2Possible[3] = v2 + new Vector2(-1, 0);

            for (int i = 0, m = dir - 1; i < 3; i++, m++)
            {
                sight[i] = mygm.CheckSurrounding(v2Possible[SnakeController.Mod(m, 4)]);
                DebugDrawLine(sight[i], v2, v2Possible[SnakeController.Mod(m, 4)]);
            }

            v2a = GetAppleSight(dir, v2a);
            sight[3] = (int)v2a.x;
            sight[4] = (int)v2a.y;
        }
        else if (PlayerPrefs.GetInt("idvision") == 3)
        {
            //Checks for 3 directional positions (empty/wall/apple) & apple
            sight = new int[5];
            Vector2[] v2Possible = new Vector2[4];

            //Add apple to board
            Vector2 v2apple = AdjustPosToBoard((Vector2)mygm.a.transform.position);
            mygm.board[(int)v2apple.x, (int)v2apple.y] = 2;

            v2Possible[0] = v2 + new Vector2(0, 1);
            v2Possible[1] = v2 + new Vector2(1, 0);
            v2Possible[2] = v2 + new Vector2(0, -1);
            v2Possible[3] = v2 + new Vector2(-1, 0);

            for (int i = 0, m = dir - 1; i < 3; i++, m++)
            {
                sight[i] = mygm.CheckSurrounding(v2Possible[SnakeController.Mod(m, 4)]);
                DebugDrawLine(sight[i], v2, v2Possible[SnakeController.Mod(m, 4)]);
            }

            v2a = GetAppleSight(dir, v2a);
            sight[3] = (int)v2a.x;
            sight[4] = (int)v2a.y;
        }
        else if (PlayerPrefs.GetInt("idvision") == 4)
        {
            //Checks for 7 directional positions (empty/wall/apple)
            sight = new int[7];
            Vector2[] v2Possible = new Vector2[8];

            //Add apple to board
            Vector2 v2apple = AdjustPosToBoard((Vector2)mygm.a.transform.position);
            mygm.board[(int)v2apple.x, (int)v2apple.y] = 2;

            v2Possible[0] = v2 + new Vector2(0, 1);
            v2Possible[1] = v2 + new Vector2(1, 1);
            v2Possible[2] = v2 + new Vector2(1, 0);
            v2Possible[3] = v2 + new Vector2(1, -1);
            v2Possible[4] = v2 + new Vector2(0, -1);
            v2Possible[5] = v2 + new Vector2(-1, -1);
            v2Possible[6] = v2 + new Vector2(-1, 0);
            v2Possible[7] = v2 + new Vector2(-1, 1);

            for (int i = 0, m = 2 * (dir - 1) - 1; i < 7; i++, m++)
            {
                sight[i] = mygm.CheckSurrounding(v2Possible[SnakeController.Mod(m, 8)]);
                DebugDrawLine(sight[i], v2, v2Possible[SnakeController.Mod(m, 8)]);
            }

        }
        else if (PlayerPrefs.GetInt("idvision") == 5)
        {
            //Checks for 7 directional positions (empty/wall) & apple
            sight = new int[9];
            Vector2[] v2Possible = new Vector2[9];

            v2Possible[0] = v2 + new Vector2(0, 1);
            v2Possible[1] = v2 + new Vector2(1, 1);
            v2Possible[2] = v2 + new Vector2(1, 0);
            v2Possible[3] = v2 + new Vector2(1, -1);
            v2Possible[4] = v2 + new Vector2(0, -1);
            v2Possible[5] = v2 + new Vector2(-1, -1);
            v2Possible[6] = v2 + new Vector2(-1, 0);
            v2Possible[7] = v2 + new Vector2(-1, 1);

            for (int i = 0, m = 2 * (dir - 1) - 1; i < 7; i++, m++)
            {
                sight[i] = mygm.CheckSurrounding(v2Possible[SnakeController.Mod(m, 8)]);
                DebugDrawLine(sight[i], v2, v2Possible[SnakeController.Mod(m, 8)]);
            }

            v2a = GetAppleSight(dir, v2a);
            sight[7] = (int)v2a.x;
            sight[8] = (int)v2a.y;
        }
        else if (PlayerPrefs.GetInt("idvision") == 6)
        {
            //Checks for 7 directional positions (empty/wall/apple) & apple
            sight = new int[9];
            Vector2[] v2Possible = new Vector2[9];

            //Add apple to board
            Vector2 v2apple = AdjustPosToBoard((Vector2)mygm.a.transform.position);
            mygm.board[(int)v2apple.x, (int)v2apple.y] = 2;

            v2Possible[0] = v2 + new Vector2(0, 1);
            v2Possible[1] = v2 + new Vector2(1, 1);
            v2Possible[2] = v2 + new Vector2(1, 0);
            v2Possible[3] = v2 + new Vector2(1, -1);
            v2Possible[4] = v2 + new Vector2(0, -1);
            v2Possible[5] = v2 + new Vector2(-1, -1);
            v2Possible[6] = v2 + new Vector2(-1, 0);
            v2Possible[7] = v2 + new Vector2(-1, 1);

            for (int i = 0, m = 2 * (dir - 1) - 1; i < 7; i++, m++)
            {
                sight[i] = mygm.CheckSurrounding(v2Possible[SnakeController.Mod(m, 8)]);
                DebugDrawLine(sight[i], v2, v2Possible[SnakeController.Mod(m, 8)]);
            }

            v2a = GetAppleSight(dir, v2a);
            sight[7] = (int)v2a.x;
            sight[8] = (int)v2a.y;
        }

        //robot sight
        return sight;
    }

    private static void DebugDrawLine(int sight, Vector2 origin, Vector2 dest)
    {
        Color clrObject;
        if (sight == 2)
        {
            clrObject = Color.red;
        }
        else if (sight == 1)
        {
            clrObject = Color.cyan;
        }
        else
        {
            clrObject = Color.white;
        }
        Debug.DrawLine(origin, dest, clrObject, GetInstance().gameStepsPerTurn * .02f);
    }

    private static Vector2 GetAppleSight(int dir, Vector2 diff)
    {
        Vector2 v2a = new Vector2();
        if (dir == 0 || dir == 2)
        {
            if (((int)diff.y < 0 && dir == 2) || ((int)diff.y > 0 && dir == 0))
            {
                v2a.y = 2;
            }
            else
            {
                v2a.y = -2;
            }

            if ((int)diff.x < 0)
            {
                v2a.x = dir == 0 ? -1 : 1;
            }
            else if ((int)diff.x > 0)
            {
                v2a.x = dir == 0 ? 1 : -1;
            }
            else
            {
                v2a.x = 0;
            }
        }
        else
        {
            if (((int)diff.x < 0 && dir == 3) || ((int)diff.x > 0 && dir == 1))
            {
                v2a.x = 2;
            }
            else
            {
                v2a.x = -2;
            }

            if ((int)diff.y < 0)
            {
                v2a.y = dir == 3 ? -1 : 1;
            }
            else if ((int)diff.y > 0)
            {
                v2a.y = dir == 3 ? 1 : -1;
            }
            else
            {
                v2a.y = 0;
            }
        }
        return v2a;
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
