using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    void Start()
    {
        // ボタンAのOnClickイベントにハンドラを追加
        Button startButton = GameObject.Find("Start").GetComponent<Button>();
        startButton.onClick.AddListener(StartGame);

        // ボタンBのOnClickイベントにハンドラを追加
        Button exitButton = GameObject.Find("Exit").GetComponent<Button>();
        exitButton.onClick.AddListener(ExitGame);
    }

    void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    void ExitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
