using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonRunGame : MonoBehaviour
{
    private Button btn;

    public InputField txtName;
    public InputField txtBoardSize;
    public InputField txtLearningRate;
    public InputField txtDiscount;
    public InputField txtEpsilon;
    public InputField txtEpsilonDec;
    public InputField txtMove;
    public InputField txtApple;
    public InputField txtWall;
    public InputField txtVision;

    public Toggle isbot;

    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        PlayerPrefs.SetString("name", txtName.text);
        SnakeId.GetInstance().SetName(txtName.text);

        PlayerPrefs.SetInt("isbot", isbot.isOn ? 1 : 0);
        PlayerPrefs.SetInt("boardsize", int.Parse(txtBoardSize.text));

        PlayerPrefs.SetFloat("learningRate", float.Parse(txtLearningRate.text));
        PlayerPrefs.SetFloat("discount", float.Parse(txtDiscount.text));
        PlayerPrefs.SetFloat("epsilon", float.Parse(txtEpsilon.text));
        PlayerPrefs.SetFloat("epsilonDec", float.Parse(txtEpsilonDec.text));

        PlayerPrefs.SetInt("move", int.Parse(txtMove.text));
        PlayerPrefs.SetInt("wall", int.Parse(txtWall.text));
        PlayerPrefs.SetInt("apple", int.Parse(txtApple.text));

        PlayerPrefs.SetInt("idvision", int.Parse(txtVision.text));

        StartCoroutine(LogNewSnake());
    }

    IEnumerator LogNewSnake()
    {
        //Connect to questions database
        string domain = "http://34.205.7.163/";
        string attempts_url = domain + "new_snake.php";

        // Create a form object for sending data to the server
        WWWForm form = new WWWForm();
        form.AddField("name", txtName.text.ToString());
        form.AddField("isbot", (isbot.isOn ? 1: 0).ToString());
        form.AddField("boardsize",  (txtBoardSize.text));

        form.AddField("learningRate",  (txtLearningRate.text));
        form.AddField("discount",  (txtDiscount.text));
        form.AddField("epsilon",  (txtEpsilon.text));
        form.AddField("epsilonDec",  (txtEpsilonDec.text));

        form.AddField("move",  (txtMove.text));
        form.AddField("wall",  (txtWall.text));
        form.AddField("apple", (txtApple.text));

        form.AddField("vision", (txtVision.text));

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
