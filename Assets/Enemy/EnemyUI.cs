using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public class EnemyUI : MonoBehaviour
{
    public Slider hpBar;
    public Image hpBarImage;
    public TextMeshProUGUI attackInterval;
    public Image attackIntervalImage;
    public TextMeshProUGUI enemyName;
    public Image ShieldImage;
    public TextMeshProUGUI ShieldText;
    Color maxColor = new Color(0.65f, 1, 0.25f, 1);
    Color minColor = new Color(1, 0.2f, 0.1f, 1);

    public void Generate(string name, int? hp = null, float? interval = null)
    {
        enemyName.text = name;
        if(hp != null)
        {
            hpBar.gameObject.SetActive(true);
            hpBar.maxValue = (float)hp;
            hpBar.value = (float)hp;
        }
        if(interval != null)
        {
            attackInterval.enabled = true;
            attackInterval.text = ((int)Mathf.Ceil((float)interval)).ToString();
        }
    }

    public async UniTask SetHP(int hp) //HPを設定
    {
        await hpBar.DOValue(hp, 0.5f).SetEase(Ease.InOutQuint).OnUpdate(() => hpBarImage.color = Color.Lerp(minColor, maxColor, hp / hpBar.maxValue));  
        
    }

    public void SetInterval(IntervalUI intervalUI) //攻撃間隔を設定
    {
        if(attackInterval) 
        {
            if(intervalUI.interval > 10) attackInterval.text = Mathf.Floor(intervalUI.interval).ToString();
            else attackInterval.text = intervalUI.interval.ToString("F1");
            attackIntervalImage.fillAmount = intervalUI.interval / intervalUI.MaxInterval;
            attackInterval.color = intervalUI.textColor;
            attackIntervalImage.color = intervalUI.circleColor;
        }
    }

    public void EnableShield(Sprite sprite, Color color, string text)
    {
        ShieldImage.gameObject.SetActive(true);
        ShieldText.text = text;
        ShieldImage.sprite = sprite;
        ShieldImage.color = color;
    }

    public void DisableShield()
    {
        ShieldImage.gameObject.SetActive(false);
    }
}

public class IntervalUI
{
    public float MaxInterval;
    public float interval;
    public Color textColor;
    public Color circleColor;

    public IntervalUI(float maxInterval, float interval, Color textColor, Color circleColor)
    {
        MaxInterval = maxInterval;
        this.interval = interval;
        this.textColor = textColor;
        this.circleColor = circleColor;
    }
}