using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EffectRequirement : ScriptableObject //攻撃の条件を満たしているか
{
    public abstract void Init();
    public abstract void isSelected(); //この攻撃が選ばれた時に実行される処理
    public abstract bool isAttack();
    public abstract void isEnd();
    public abstract IntervalUI GetAttackUIText();
}