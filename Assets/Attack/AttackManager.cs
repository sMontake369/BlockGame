using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using Unity.Mathematics;

public class AttackManager : MonoBehaviour
{
    MainGameManager GamM;
    FrameManager FraM;
    EnemyManager EneM;
    BattleManager BatM;

    int targetIndex = 0; //攻撃対象の番号

    [SerializeField]
    public int maxEdge = 30; //最大の辺の長さ

    AttackRBlock aRBlock;
    public void Init(BattleManager BatM) //初期化
    {
        this.BatM = BatM;
        GamM = BatM.GamM;
        FraM = BatM.FraM;
        EneM = BatM.EneM;
    }

    public void DoAttack() //攻撃命令
    {
        if(aRBlock == null) return;
        aRBlock.DoAttack(EneM.EnemyList[targetIndex]);
        aRBlock = null;
    }

    public void Reset()
    {
        if(aRBlock != null)
        aRBlock.Destroy();
        aRBlock = null;
    }

    public void SetTarget() //ターゲットを設定
    {
        targetIndex++;
        targetIndex %= EneM.EnemyList.Count;
    }

    AttackRBlock CreateAttackRBlock()
    {
        AttackRBlock attackRBlock = GamM.RootConvert<AttackRBlock>(GamM.GenerateRBlock());
        attackRBlock.transform.parent = this.transform;
        attackRBlock.name = "AttackRBlock";
        attackRBlock.transform.position = FraM.WFrameBorder.lowerLeft + BatM.battleData.attackPos + new Vector3(0, 0, -10f);
        return attackRBlock;
    }

    public void AddAttackQueue(List<BaseBlock> blockList, int lineNum) //消されたブロックを攻撃ブロックとして待機リストに追加
    {
        if(aRBlock == null) //攻撃ブロックがない場合は新しく作成
        {
            aRBlock = CreateAttackRBlock();
            aRBlock.Init(this);
        }

        aRBlock.AddAttackQueue(blockList, lineNum);
    }
}
