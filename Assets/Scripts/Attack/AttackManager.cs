using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AttackManager : BaseManager
{
    MainGameManager GamM;
    FrameManager FraM;
    EnemyManager EneM;
    BattleManager BatM;

    int targetIndex = 0; //攻撃対象の番号

    [SerializeField]
    public int maxEdge = 30; //最大の辺の長さ

    AttackController aRBlock;

    public override void Init()
    {
        BatM = this.transform.parent.GetComponent<BattleManager>();
        GamM = BatM.gamM;
        FraM = BatM.fraM;
        EneM = BatM.eneM;
    }

    public void Reset()
    {
        if(aRBlock != null)
        // aRBlock.Destroy();
        aRBlock = null;
    }

    public void SetTarget() //ターゲットを設定
    {
        targetIndex++;
        targetIndex %= EneM.EnemyList.Count;
    }

    public void AddAttackQueue(Block block) //消されたブロックを攻撃ブロックとして待機リストに追加
    {
        if (aRBlock == null) //攻撃ブロックがない場合は新しく作成
        {
            aRBlock = new GameObject("AttackRBlock").AddComponent<AttackController>();
            aRBlock.transform.position = (Vector2)BatM.transform.position + BatM.battleData.attackPos;
            aRBlock.transform.SetParent(this.gameObject.transform); //親ブロックを設定
            aRBlock.Init(this);
            aRBlock.SetTarget(EneM.EnemyList[targetIndex]);
        }

        aRBlock.AddWaitingBlock(block);
    }
}
