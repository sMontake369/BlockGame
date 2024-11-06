using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

public class AttackManager : MonoBehaviour
{
    MainGameManager GamM;
    FrameManager FraM;
    EnemyManager EneM;
    BattleManager BatM;
    AttackUI attackUI;
    int targetIndex = 0; //攻撃対象の番号
    [SerializeField]
    public int maxEdge = 30; //最大の辺の長さ

    AttackRBlock aRBlock;
    List<BaseBlock> blockQueue = new List<BaseBlock>(); //待機中の攻撃ブロックのリスト

    GameObject centerObject; //回転の中心
    float radius = 4f; //半径
    float rotateSpeed = 180; //回転速度

    public bool doUpdating = false; //攻撃ブロックを設定中か
    bool doAttacking = false; //攻撃開始を命令 
    CancellationTokenSource cts;
    CancellationToken token;

    public void Init(BattleManager BatM) //初期化
    {
        this.BatM = BatM;
        GamM = BatM.GamM;
        FraM = BatM.FraM;
        EneM = BatM.EneM;
        
        attackUI = Addressables.InstantiateAsync("AttackCanvas").WaitForCompletion().GetComponent<AttackUI>();
        attackUI.GetComponent<Canvas>().worldCamera = Camera.main;
        attackUI.transform.SetParent(this.transform);
        attackUI.gameObject.SetActive(false);

        centerObject = new GameObject("WaitingList");
        centerObject.transform.SetParent(this.transform);

        cts = new CancellationTokenSource();  
        token = cts.Token;  
    }

    public void Update()
    {
        if(aRBlock == null) return;

        rotateSpeed = 90 + Mathf.Clamp(aRBlock.GetBlockNum() * 3f, 0, 180);
        centerObject.transform.Rotate(new Vector3(-1, -1, -1), rotateSpeed * Time.deltaTime);

        if(doAttacking && !doUpdating) Attack(); //攻撃命令が来て、設定中でない場合は攻撃
    }

    public void OrderAttack()
    {
        if(aRBlock == null) return;
        doAttacking = true;
    }

    void Attack() //攻撃を実行
    {
        if(aRBlock == null) return;
        doUpdating = false;
        doAttacking = false;

        attackUI.Reset();
        attackUI.gameObject.SetActive(false);
        attackUI.transform.SetParent(this.transform);

        //if(1 < attackRBlock.GetBlockNum()) attackRBlock.ToOneBlock(); ------------------------------------------------------
        aRBlock.Attack(EneM.EnemyList[targetIndex]);
        aRBlock = null;

        //とりあえず全部消す。いずれこれも一緒に攻撃する
        foreach(BaseBlock block in blockQueue) Destroy(block.gameObject);
        blockQueue.Clear();
    }

    public void Reset()
    {
        cts.Cancel();
        attackUI.gameObject.SetActive(false);
        if(aRBlock != null) BlockPool.ReleaseNotRootBlock(aRBlock);
        aRBlock = null;

        foreach(BaseBlock block in blockQueue) BlockPool.ReleaseNotBaseBlock(block);
        blockQueue.Clear();

        doUpdating = false;
        attackUI.Reset();
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
        attackRBlock.transform.position = FraM.WFrameBorder.lowerLeft + BatM.battleData.attackPos + new Vector3(0, 0, -3f);
        attackUI.gameObject.SetActive(true);
        return attackRBlock;
    }

    public async void AddAttackQueue(List<BaseBlock> blockList, int lineNum) //消されたブロックを攻撃ブロックとして待機リストに追加
    {
        if(token.IsCancellationRequested) return;

        blockQueue.AddRange(blockList);

        if(aRBlock == null) //攻撃ブロックがない場合は新しく作成
        {
            aRBlock = CreateAttackRBlock();

            attackUI.gameObject.SetActive(true);
            attackUI.transform.SetParent(aRBlock.transform);
            attackUI.transform.position = aRBlock.transform.position + new Vector3(0, 0, -1f);
        }
        aRBlock.AddPower(blockList.Count, lineNum);
        attackUI.SetPower(aRBlock.power);

        //設定中でない場合はブロックを設定 
        if(!doUpdating)
        {
            doUpdating = true;
            await CheckEnoughBlocks(token);
            doUpdating = false;
        }
        else SetWaitingBlocks(blockQueue, token);
    }
    
    async UniTask CheckEnoughBlocks(CancellationToken token) //必要数を攻撃ブロックに、余りを待機ブロックに設定
    {
        while(aRBlock != null && aRBlock.CanSquire(blockQueue.Count))
        {
            int needNum = (aRBlock.edge + 1) * (aRBlock.edge + 1) - aRBlock.edge * aRBlock.edge;
            List<BaseBlock> blockList = blockQueue.GetRange(0, needNum);
            blockQueue.RemoveRange(0, needNum);
            await aRBlock.SetSquire(blockList);
            attackUI.SetPos(aRBlock.edge);
            if(aRBlock.edge >= maxEdge) Attack();
        }
        SetWaitingBlocks(blockQueue, token);
    }

    async void SetWaitingBlocks(List<BaseBlock> blockList, CancellationToken token) //待機ブロックを設定
    {
        radius = aRBlock.edge;
        if(aRBlock) _ = centerObject.transform.DOMove(aRBlock.transform.position + new Vector3(radius / 2.0f - 1f, radius / 2.0f - 1f, 0), 0.5f);
        foreach(BaseBlock baseBlock in blockList) baseBlock.transform.parent = centerObject.transform;
        
        for(int i = 0; i < blockQueue.Count; i++)
        {
            var point =  ((float)i / blockQueue.Count) * 2 * Mathf.PI;
            float x = Mathf.Cos(point) * radius;
            float y = Mathf.Sin(point) * radius;

            _ = blockQueue[i].transform.DOLocalRotate(new Vector3(0,0, 360 / blockQueue.Count * i + 90), 0.5f);
            _ = blockQueue[i].transform.DOLocalMove(new Vector3(x, y, 0), 0.5f);

            try { await UniTask.Delay(10, cancellationToken: token); }
            catch(OperationCanceledException) { return;}
        }
    }
}
