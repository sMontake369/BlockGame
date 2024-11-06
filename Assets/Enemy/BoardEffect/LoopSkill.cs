using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "LoopSkill", menuName = "Effect/LoopSkill")]
public class LoopSkill : BaseEffectData
{
    [Header("ループするエフェクト")]
    public List<BaseEffectData> loopEffectList;

    [Header("ループ回数")]
    public int loopCount;
    public override async UniTask Execute()
    {
        if(isWait) await LoopEffect();
        else _ = LoopEffect();
    }

    async UniTask LoopEffect()
    {
        for (int i = 0; i < loopCount; i++)
        {
            foreach (BaseEffectData effect in loopEffectList)
            {
                if(effect.isWait) await effect.Execute();
                else _ = effect.Execute();
            }
        }
    }

    public override void Init(Enemy enemy)
    {
        this.enemy = enemy;
        foreach (BaseEffectData effect in loopEffectList) effect.Init(enemy);
    }
}
