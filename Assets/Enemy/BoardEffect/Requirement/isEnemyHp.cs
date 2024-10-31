using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "isEnemyHp", menuName = "EffectRequirement/CreateIsEnemyHp")]
public class isEnemyHp : EffectRequirement
{
    [Header("発動するHPの割合")]
    public int HPRatio; //HPの割合
    public override void Init(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override void isSelected()
    {

    }

    public override bool isAttack()
    {
        if(enemy.hp <= enemy.maxHp * HPRatio / 100)
        {
            return true;
        }
        return false;
    }

    public override void isEnd()
    {

    }

    public override IntervalUI GetAttackUIText()
    {
        return null;
    }
}
