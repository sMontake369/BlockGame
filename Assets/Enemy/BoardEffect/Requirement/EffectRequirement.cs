using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EffectRequirement : ScriptableObject //攻撃の条件を満たしているか
{
    protected Enemy enemy;
    public abstract void Init(Enemy enemy); //初期化
    public abstract void isSelected(); //この攻撃が選ばれた時に実行される処理
    public abstract bool isAttack(); //攻撃の実行条件を満たしているか
    public abstract void isEnd(); //この攻撃が終了した時に実行される処理
    public abstract IntervalUI GetAttackUIText();
}