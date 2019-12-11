using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HeadlessStart : MonoBehaviour
{
    public string strName;
    private bool isBot = true;
    public int intBoardSize = 11;

    public float fltLearningRate = .8f;
    public float fltDiscount = .95f;
    public float fltEpsilon = .999f;
    public float fltEpsilonDec = .001f;

    public int intMoveScore = -1;
    public int intAppleScore = 250;
    public int intWallScore = -100;

    public int intVisionId = 6;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("name", strName);

        PlayerPrefs.SetInt("isbot", isBot ? 1 : 0);
        PlayerPrefs.SetInt("boardsize", intBoardSize);

        PlayerPrefs.SetFloat("learningRate", fltLearningRate);
        PlayerPrefs.SetFloat("discount", fltDiscount);
        PlayerPrefs.SetFloat("epsilon", fltEpsilon);
        PlayerPrefs.SetFloat("epsilonDec", fltEpsilonDec);

        PlayerPrefs.SetInt("move", intMoveScore);
        PlayerPrefs.SetInt("wall", intWallScore);
        PlayerPrefs.SetInt("apple", intAppleScore);

        PlayerPrefs.SetInt("idvision", intVisionId);

        StartCoroutine(LogNewSnake());
    }

    IEnumerator LogNewSnake()
    {
        //Connect to questions database
        string domain = "http://34.205.7.163/";
        string attempts_url = domain + "new_snake.php";

        // Create a form object for sending data to the server
        WWWForm form = new WWWForm();
        form.AddField("name", strName);
        form.AddField("isbot", (isBot ? 1 : 0).ToString());
        form.AddField("boardsize", (intBoardSize));

        form.AddField("learningRate", (fltLearningRate).ToString());
        form.AddField("discount", (fltDiscount).ToString());
        form.AddField("epsilon", (fltEpsilon).ToString());
        form.AddField("epsilonDec", (fltEpsilonDec).ToString());

        form.AddField("move", (intMoveScore));
        form.AddField("wall", (intWallScore));
        form.AddField("apple", (intAppleScore));

        form.AddField("vision", (intVisionId));

        var download = UnityWebRequest.Post(attempts_url, form);

        // Wait until the download is done
        yield return download.SendWebRequest();

        if (download.isNetworkError || download.isHttpError)
        {
            Debug.Log("Error downloading: " + download.error);
        }
        else
        {
            //Debug.Log(download.downloadHandler.text);
            PlayerPrefs.SetInt("idplayer", int.Parse(download.downloadHandler.text));
            SnakeId.GetInstance().SetSnakeId(int.Parse(download.downloadHandler.text));

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
