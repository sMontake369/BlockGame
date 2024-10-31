using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "isEnemyHp", menuName = "EffectRequirement/CreateIsEnemyHp")]
public class isEnemyHp : OccurRequirement
{
    [Header("発動するHPの割合")]
    public int HPRatio; //HPの割合
    public bool isAbove = false; //trueならHPが指定した割合以上の時に発動、falseなら指定した割合以下の時に発動

    public override void Init(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override bool IsOccur()
    {
        if(!isAbove && enemy.hp <= enemy.maxHp * HPRatio / 100)
        {
            return true;
        }
        else if(isAbove && enemy.hp >= enemy.maxHp * HPRatio / 100)
        {
            return true;
        }
        return false;
    }
}
