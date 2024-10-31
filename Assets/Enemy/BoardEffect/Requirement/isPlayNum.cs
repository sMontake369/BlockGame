using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "isPlayNum", menuName = "EffectRequirement/CreateIsPlayNum")]
public class isPlayNum : AttackRequirement
{
    MainGameManager GamM;
    RootBlock playerBlock;
    int playerBlockCount; //現在の置いたプレイヤーブロックの数
    [Header("置けるプレイヤーブロックの数")]
    public int activationCount; //発動条件
    public override void Init(Enemy enemy)
    {
        BattleManager BatM = FindFirstObjectByType<StageManager>().GetCurBattle();
        GamM = BatM.GamM;
        this.enemy = enemy;
        playerBlockCount = 0;
    }

    public override void isSelected()
    {
        playerBlockCount = 0;
        playerBlock = GamM.playerBlock;
    }

    public override bool isAttack()
    {
        if(playerBlock != GamM.playerBlock)
        {
            if(playerBlock) playerBlockCount++;
            playerBlock = GamM.playerBlock;
        }

        if(playerBlockCount >= activationCount)
        {
            return true;
        }
        return false;
    }

    public override void isEnd()
    {
        playerBlockCount = 0;
    }

    public override IntervalUI GetAttackUIText()
    {
        return new IntervalUI(activationCount, activationCount - playerBlockCount, Color.white, Color.blue);
    }
}
