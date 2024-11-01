using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "SetEnemySkill", menuName = "Effect/SetEnemySkill")]
public class SetEnemySkill : BaseEffectData
{
    [SerializeField] [Tooltip("敵に追加するスキル")]
    public EnemySkill enemySkill;
    [SerializeField] [Tooltip("ネクストスキルにセット")]
    public bool isNow = false;

    public override void Init(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override UniTask Execute()
    {
        enemy.SetNextSkill(enemySkill, isNow);
        return UniTask.CompletedTask;
    }
}
