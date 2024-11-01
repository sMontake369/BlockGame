using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class AttackUI : MonoBehaviour
{
    public TextMeshProUGUI powerText;
    AttackManager AttM;
    // Start is called before the first frame update
    void Awake()
    {
        AttM = FindFirstObjectByType<AttackManager>();
    }

    public void SetPower(int power)
    {
        int nowNumber = int.Parse(powerText.text);
        DOTween.To(() => nowNumber, (n) => nowNumber = n, power, 0.3f)
            .OnUpdate(() => powerText.text = nowNumber.ToString());
        float scale = Mathf.Clamp(3 * Mathf.Sin((Mathf.PI / 2) * ((float)power / 1000)), 0, 1);
        powerText.rectTransform.sizeDelta = new Vector2(scale, scale);
    }

    public void SetPos(int edge)
    {
        float centerPos = (edge * 20 - 5) / 2.0f;
        powerText.rectTransform.DOKill();
        powerText.rectTransform.DOLocalMove(new Vector3(centerPos, centerPos, 0),1.0f);
    
    }

    public void Reset()
    {
        powerText.text = "0";
        powerText.rectTransform.DOLocalMove(new Vector3(0,-26, 0), 0.5f);
        powerText.rectTransform.sizeDelta = new Vector2(20, 20);
    }
}
