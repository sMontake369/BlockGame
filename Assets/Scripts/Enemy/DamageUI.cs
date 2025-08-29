using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class DamageUI : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public Image weakImage;

    public void Generate(Enemy enemy, int damage, bool isWeak)
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        transform.SetParent(enemy.transform);
        transform.position = enemy.transform.position + new Vector3(0,0,-3);
        float scale = Mathf.Clamp(3 * Mathf.Sin((Mathf.PI / 2) * ((float)damage / 1000)), 0, 1);
        transform.localScale += new Vector3(scale, scale, scale);
        damageText.text = damage.ToString();
        if(isWeak) weakImage.gameObject.SetActive(true);

        transform.DOLocalMove(new Vector3(0, 1, -0.1f), 0.5f).SetEase(Ease.OutQuart).OnComplete(() => Destroy(gameObject));
    }
}
