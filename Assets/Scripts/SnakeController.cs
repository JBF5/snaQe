using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SnakeController : GamePiece
{
    //Instance of SnakeBody Object
    public SnakeBody sbSnakeHead;

    private int length = 1;

    //Instance of SnakeBody Object
    private SnakeBody sbSnakeTail;

    //Unity GameObject
    public GameObject goBodyPrefab;

    //Direction turning
    private Turning dir = Turning.FORWARD;
    //Direction traveling
    private Compas compas = Compas.NORTH;

    private Vector2 dest;

    private int steps = 0;
    private int apples = 0;
    private int turns = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Length of snake is 1 so head = tail
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
                turns++;
                break;
            case Turning.RIGHT:
                Debug.Log("Turning R");
                compas = (Compas)Mod((int)compas + 1, 4);
                turns++;
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

        steps++;

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

    //RoboSnake Turning Actions Left & Right
    public void TurnTo(int turn)
    {
        //1 turns left, -1 turns right
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

        apples++;
        length++;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Wall>() != null)
        {
            Debug.Log("Dead");
            GameMaster.GameOver();
            StartCoroutine(LogSnake());

        } else if (collision.gameObject.GetComponent<Wall>() != null)
        {
            Debug.Log("Apple");
        }
    }

    IEnumerator LogSnake()
    {
        //Connect to questions database
        string domain = "http://3.87.156.253/";
        string attempts_url = domain + "snake_stats.php";

        // Create a form object for sending data to the server
        WWWForm form = new WWWForm();
        form.AddField("steps", steps.ToString());
        form.AddField("apples", apples.ToString());
        form.AddField("turns", turns.ToString());

        var download = UnityWebRequest.Post(attempts_url, form);

        // Wait until the download is done
        yield return download.SendWebRequest();

        if (download.isNetworkError || download.isHttpError)
        {
            Debug.Log("Error downloading: " + download.error);

        }
        else
        {
            Debug.Log(download.downloadHandler.text + "\nAttempt sent successfully");
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
