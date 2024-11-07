using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Talk", menuName = "Effect/Talk")]
public class Talk : BaseEffectData
{
    [Header("会話内容")]
    public List<string> textList;

    [Header("表示時間(ms)")] 
    public int interval;

    public override async UniTask Execute()
    {
        foreach(string text in textList) await enemy.Talk(text, interval);
    }

    public override void Init(Enemy enemy)
    {
        this.enemy = enemy;
    }
}
