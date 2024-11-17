using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    Label centerText;
    public void Init()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        centerText = root.Q<Label>("CenterLabel");
    }

    public async void SetCenterText(string text, float time)
    {
        centerText.text = text;
        await UniTask.Delay((int)(time * 1000));
        centerText.text = "";
    }
}
