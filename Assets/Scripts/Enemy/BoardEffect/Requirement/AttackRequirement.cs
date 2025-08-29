using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class AttackRequirement : ScriptableObject //攻撃の発動条件(条件を満たすと攻撃を行う)
{
    protected Enemy enemy;
    public abstract void Init(Enemy enemy); //初期化
    public abstract void isSelected(); //この攻撃が選ばれた時に実行される処理
    public abstract bool isAttack(); //攻撃の実行条件を満たしているか
    public abstract void isEnd(); //この攻撃が終了した時に実行される処理
    public abstract IntervalUI GetAttackUIText();
}