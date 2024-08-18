using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class AttackUI : MonoBehaviour
{
    public Slider attackTimeSlider;
    public TextMeshProUGUI powerText;
    AttackManager AttM;
    // Start is called before the first frame update
    void Awake()
    {
        AttM = FindFirstObjectByType<AttackManager>();
        attackTimeSlider.maxValue = 10;
    }

    public void SetPower(int power)
    {
        int nowNumber = int.Parse(powerText.text);
        DOTween.To(() => nowNumber, (n) => nowNumber = n, power, 0.3f)
            .OnUpdate(() => powerText.text = nowNumber.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if(AttM.AttackTime > 0)
        {
            attackTimeSlider.value = AttM.AttackTime;
        }
    }

    public void SetPos(int edge)
    {
        float centerPos = edge * 20 / 2.0f;
        int textSize = edge * 20;
        powerText.rectTransform.DOKill();
        powerText.rectTransform.DOLocalMove(new Vector3(centerPos - 10, centerPos - 10, 0),1.0f);
        attackTimeSlider.transform.DOKill();
        attackTimeSlider.transform.DOLocalMove(new Vector3(centerPos - 10, -26, 0), 1.0f);
        powerText.rectTransform.sizeDelta = new Vector2(textSize, textSize);
    }

    public void Reset()
    {
        attackTimeSlider.value = 0;
        powerText.text = "0";
        powerText.rectTransform.DOLocalMove(new Vector3(0,-26,0), 0.5f);
        attackTimeSlider.transform.DOLocalMove(new Vector3(0,0,0), 0.5f);
        powerText.rectTransform.sizeDelta = new Vector2(20, 20);
    }
}
