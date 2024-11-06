using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "isEnemyHp", menuName = "EffectRequirement/CreateIsEnemyHp")]
public class isEnemyHp : OccurRequirement
{
    [Header("HPの最大割合")] [Range(0, 100)]
    public int max = 100; //HPの割合
    [Header("HPの最低割合")] [Range(0, 100)]
    public int min = 0; //HPの割合

    public override void Init(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override bool IsOccur()
    {
        if(enemy.hp >= enemy.maxHp * min / 100 && enemy.hp <= enemy.maxHp * max / 100)
        {
            return true;
        }
        return false;
    }
}
