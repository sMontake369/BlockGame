using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TitleButton : MonoBehaviour
{
    // TODO いつか消す!
    public StageData kari;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement ButtonElements = root.Q<VisualElement>("Title").Q<VisualElement>("ButtonElements");
        root.Q<Button>("Start_Button").clicked += StartGame;
        root.Q<Button>("Exit_Button").clicked += ExitGame;
    }

    void StartGame()
    {
        // SceneManager.LoadScene("StageSelect");
        SendStage.stageData = kari;
        SceneManager.LoadScene("GameScene");
    }

    void ExitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
