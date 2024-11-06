using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "deleteSkill", menuName = "Effect/deleteSkill")]
public class deleteSkill : BaseEffectData
{
    public string deleteSkillName;

    public override UniTask Execute()
    {
        foreach (EnemySkill skill in enemy.skillList)
        {
            if (skill.name == deleteSkillName)
            {
                enemy.skillList.Remove(skill);
                break;
            }
        }
        return UniTask.CompletedTask;
    }

    public override void Init(Enemy enemy)
    {
        this.enemy = enemy;
    }
}
