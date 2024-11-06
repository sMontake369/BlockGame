using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Sleep", menuName = "Effect/Sleep")]
public class Sleep : BaseEffectData
{

    [SerializeField] [Header("スリープする時間(ms)")] 
    int sleepTime;

    public override void Init(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override async UniTask Execute()
    {
        await UniTask.Delay(sleepTime);
    }
}
