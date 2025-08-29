using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "isPlayNum", menuName = "EffectRequirement/CreateIsPlayNum")]
public class isPlayNum : AttackRequirement
{
    MainGameManager GamM;
    ContainerBlock playerBlock;
    int playerBlockCount; //現在の置いたプレイヤーブロックの数
    [Header("置けるプレイヤーブロックの数")]
    public int activationCount; //発動条件
    public override void Init(Enemy enemy)
    {
        BattleManager BatM = FindFirstObjectByType<StageManager>().GetCurBattle();
        GamM = BatM.gamM;
        this.enemy = enemy;
        playerBlockCount = 0;
    }

    public override void isSelected()
    {
        playerBlockCount = 0;
        playerBlock = GamM.playerCBlock;
    }

    public override bool isAttack()
    {
        if(playerBlock != GamM.playerCBlock)
        {
            playerBlockCount++;
            playerBlock = GamM.playerCBlock;
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
