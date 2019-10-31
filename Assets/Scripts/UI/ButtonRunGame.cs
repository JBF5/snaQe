using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonRunGame : MonoBehaviour
{
    private Button btn;

    public Text name;
    public Toggle isbot;

    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        PlayerPrefs.SetInt("isbot", isbot.isOn ? 1: 0);
        StartCoroutine(LogNewSnake());
    }

    IEnumerator LogNewSnake()
    {
        //Connect to questions database
        string domain = "http://3.87.156.253/";
        string attempts_url = domain + "new_snake.php";

        // Create a form object for sending data to the server
        WWWForm form = new WWWForm();
        form.AddField("name", name.text.ToString());
        form.AddField("isbot", (isbot.isOn ? 1: 0).ToString());

        var download = UnityWebRequest.Post(attempts_url, form);

        // Wait until the download is done
        yield return download.SendWebRequest();

        if (download.isNetworkError || download.isHttpError)
        {
            Debug.Log("Error downloading: " + download.error);
        }
        else
        {
            Debug.Log(download.downloadHandler.text);
            PlayerPrefs.SetInt("idplayer", int.Parse(download.downloadHandler.text));
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
