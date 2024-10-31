using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "isTime", menuName = "EffectRequirement/CreateIsTime")]
public class isTime : AttackRequirement
{
    float selectedTime; //選択されてからの時間

    [Header("発動時間")]
    public float activationTime;

    public override void Init(Enemy enemy)
    {
        this.enemy = enemy;
        selectedTime = 0;
    }

    public override void isSelected()
    {
        selectedTime = Time.time;
    }

    public override bool isAttack()
    {
        if(Time.time - selectedTime > activationTime)
        {
            return true;
        }
        return false;
    }

    public override void isEnd()
    {
        selectedTime = 0;
    }

    public override IntervalUI GetAttackUIText()
    {
        return new IntervalUI(activationTime, math.max(0, activationTime - (Time.time - selectedTime)), Color.white, Color.yellow);
    }
}
