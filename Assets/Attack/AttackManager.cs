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
    AttackUI AttackUI;
    int targetIndex = 0; //攻撃対象のインデックス
    [SerializeField]
    public int maxEdge = 10; //最大の辺の長さ

    AttackRBlock attackRBlock;
    List<BaseBlock> waitingBlockList = new List<BaseBlock>(); //攻撃ブロックのリスト
    float attackTime = 0;
    public float AttackTime { get => attackTime; }
    int edge = 1; //正方形の辺の長さ

    GameObject centerObject; //回転の中心
    float radius = 4f; //半径
    float rotateSpeed = 180; //回転速度

    bool doUpdating = false; //攻撃ブロックを設定中か
    CancellationTokenSource cts;
    CancellationToken token;

    public void Init()
    {
        GamM = FindFirstObjectByType<MainGameManager>();
        FraM = FindFirstObjectByType<FrameManager>();
        EneM = FindFirstObjectByType<EnemyManager>();
        BatM = FindFirstObjectByType<BattleManager>();

        AttackUI = Addressables.InstantiateAsync("AttackCanvas").WaitForCompletion().GetComponent<AttackUI>();
        AttackUI.GetComponent<Canvas>().worldCamera = Camera.main;
        AttackUI.transform.SetParent(this.transform);
        AttackUI.gameObject.SetActive(false);

        centerObject = new GameObject("WaitingList");
        centerObject.transform.SetParent(this.transform);

        cts = new CancellationTokenSource();  
        token = cts.Token;  
    }

    public async void Update() 
    {
        if(attackRBlock == null) return;

        rotateSpeed = 90 + Mathf.Clamp(attackRBlock.GetBlockNum() * 3f, 0, 180);
        centerObject.transform.Rotate(new Vector3(-1, -1, -1), rotateSpeed * Time.deltaTime);

        if(doUpdating) return;
        if(attackTime >= 0 && attackRBlock) 
        {
            attackTime -= Time.deltaTime;
        }

        if(attackTime < 0 && attackRBlock.GetBlockNum() > 0)
        {
            doUpdating = true;
            await Attack(token);
            await CheckEnoughBlocks(token);
            doUpdating = false;
        }
    }

    public async UniTask Attack(CancellationToken token) //攻撃を実行
    {
        try { await UniTask.Delay(1000, cancellationToken: token); }
        catch(OperationCanceledException) { return; }
        AttackUI.Reset();
        AttackUI.transform.SetParent(this.transform);

        //if(1 < attackRBlock.GetBlockNum()) attackRBlock.ToOneBlock(); ------------------------------------------------------
        attackRBlock.Attack(EneM.EnemyList[targetIndex]);

        attackTime = 0;
        attackRBlock = null;
    }

    public void ResetAttackBLock()
    {
        cts.Cancel();
        AttackUI.gameObject.SetActive(false);
        if(attackRBlock != null) BlockPool.ReleaseNotRootBlock(attackRBlock);
        attackRBlock = null;

        foreach(BaseBlock block in waitingBlockList) BlockPool.ReleaseNotBaseBlock(block);
        waitingBlockList.Clear();

        attackTime = 0;
        doUpdating = false;
        AttackUI.Reset();
    }

    public void GenToken() //これいる？
    {
        cts = new CancellationTokenSource();  
        token = cts.Token;
    }

    public void SetTarget() //ターゲットを設定
    {
        targetIndex++;
        targetIndex %= EneM.EnemyList.Count;
    }

    public async void AddAttackQueue(List<BaseBlock> candidateBlockList) //消されたブロックのリストを受け取り、
    {
        if(token.IsCancellationRequested) return;

        waitingBlockList.AddRange(candidateBlockList);

        //設定中でない場合はブロックを設定 
        if(!doUpdating)
        {
            doUpdating = true;
            await CheckEnoughBlocks(token);
            doUpdating = false;
        }
        else AnimateWaitingBlocks(candidateBlockList, token);
    }
    
    async UniTask CheckEnoughBlocks(CancellationToken token) //必要数を攻撃ブロックに、余りを待機ブロックに設定
    {
        if(token.IsCancellationRequested) return;

        //正方形を作るのに追加で必要なブロック数を計算
        int blockNum = 0;
        if(attackRBlock) blockNum = attackRBlock.GetBlockNum();

        edge = 1;
        while((edge * edge) <= blockNum) edge++;
        int needBlockNum = (edge * edge) - blockNum; //一回り大きい正方形を作るのに必要なブロック数

        //追加で必要なブロック数が足りているか
        if(waitingBlockList.Count >= needBlockNum)
        {
            List<BaseBlock> setBlockList = new List<BaseBlock>(waitingBlockList.GetRange(0, needBlockNum)); //設定するブロックのリスト
            waitingBlockList.RemoveRange(0, needBlockNum);
            if(setBlockList.Count != needBlockNum) Debug.Log(setBlockList.Count + " " + needBlockNum);

            await SetAttackBlock(setBlockList, token);
            AnimateWaitingBlocks(waitingBlockList, token);
            if(edge >= maxEdge) await Attack(token); //辺の数が最大になったら待たずに攻撃

            await CheckEnoughBlocks(token); //再帰
        }
        else AnimateWaitingBlocks(waitingBlockList, token);
    }

    private async UniTask SetAttackBlock(List<BaseBlock> setBlockList, CancellationToken token) //攻撃ブロックを設定
    {
        if(attackRBlock == null) //攻撃ブロックがない場合は新しく作成
        {
            attackRBlock = GamM.RootConvert<AttackRBlock>(GamM.GenerateRBlock());
            attackRBlock.transform.parent = this.transform;
            attackRBlock.name = "AttackRBlock";
            attackRBlock.transform.position = FraM.WFrameBorder.lowerLeft + BatM.battleData.attackPos;

            AttackUI.gameObject.SetActive(true);
            AttackUI.transform.SetParent(attackRBlock.transform);
            AttackUI.transform.position = attackRBlock.transform.position + new Vector3(0, 0, -10f);
        }

        int blockNum = setBlockList.Count;
        int x;
        for(int y = 0; y < edge; y++)
        {
            if(y < edge - 1) x = edge - 1;
            else x = 0;
            for(; x < edge; x++)
            {
                BaseBlock block = setBlockList[0];
                setBlockList.RemoveAt(0);
                block.transform.DOKill();

                Vector3Int index = new Vector3Int(x, y, 0); //正方形に配置する座標
                attackRBlock.AddBlock(block, index, false);

                //アニメーション
                _ = block.transform.DOJump(attackRBlock.transform.position + index, 5, 1, 0.7f).SetEase(Ease.InOutQuad);
                _ = block.transform.DORotate(new Vector3(-90,0,0), 1f).SetEase(Ease.InExpo);

                try { await UniTask.Delay(10, cancellationToken: token); }
                catch(OperationCanceledException) { return;}
            }
        }

        attackTime += (2.5f * blockNum) / attackRBlock.GetBlockNum();
        attackRBlock.AddPower(blockNum);
        AttackUI.SetPower(attackRBlock.Power);
        AttackUI.SetPos(edge);
    }

    async void AnimateWaitingBlocks(List<BaseBlock> baseBlockList, CancellationToken token) //待機ブロックを設定
    {
        radius = edge ;
        if(attackRBlock) _ = centerObject.transform.DOMove(attackRBlock.transform.position + new Vector3(radius / 2.0f - 1f, radius / 2.0f - 1f, 0), 0.5f);
        foreach(BaseBlock baseBlock in baseBlockList) baseBlock.transform.parent = centerObject.transform;
        
        for(int i = 0; i < waitingBlockList.Count; i++)
        {
            var point =  ((float)i / waitingBlockList.Count) * 2 * Mathf.PI;
            float x = Mathf.Cos(point) * radius;
            float y = Mathf.Sin(point) * radius;

            _ = waitingBlockList[i].transform.DOLocalRotate(new Vector3(0,0, 360 / waitingBlockList.Count * i + 90), 0.5f);
            _ = waitingBlockList[i].transform.DOLocalMove(new Vector3(x, y, 0), 0.5f);

            try { await UniTask.Delay(10, cancellationToken: token); }
            catch(OperationCanceledException) { return;}
        }
    }
}
