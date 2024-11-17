using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TitleButton : MonoBehaviour
{
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("Start_Button").clicked += StartGame;
        root.Q<Button>("Exit_Button").clicked += ExitGame;
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
