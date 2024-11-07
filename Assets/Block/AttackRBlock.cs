using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine.AddressableAssets;

public class AttackRBlock : RootBlock //RAttack
{   
    public int power { private set; get; } = 0;
    public List<ColorType> colorTypeList { private set; get; } = new List<ColorType>(); //攻撃ブロックの色のリスト
    List<Texture> textureList = new List<Texture>();
    AudioManager AudM;

    Enemy targetEnemy; //攻撃対象の敵

    bool isAttack = false; //攻撃指示が出ているか
    bool doAttacking = false; //攻撃中か

    CancellationTokenSource cts;
    CancellationToken token;

    int attackQueueNum = 0; //攻撃待機リストの数
    int playAddAttackQueueNum = 0; //AddAttackQueueの実行回数
    List<BaseBlock> blockQueue = new List<BaseBlock>(); //待機中の攻撃ブロックのリスト
    public bool doUpdating = false; //攻撃ブロックを設定中か
    AttackUI attackUI;
    public int edge { private set; get; } = 0; //現在の正方形の辺の長さ
    AttackManager AttM;
    float rotateSpeed;
    GameObject centerObject;
    GameObject videoObj;

    public void Init(AttackManager AttM) //初期化
    {
        this.AttM = AttM;
        AudM = FindFirstObjectByType<StageManager>().AudM;

        attackUI = Addressables.InstantiateAsync("AttackCanvas").WaitForCompletion().GetComponent<AttackUI>();
        attackUI.GetComponent<Canvas>().worldCamera = Camera.main;
        attackUI.transform.SetParent(this.transform);
        attackUI.transform.position = this.transform.position + new Vector3(0, 0, -1f);

        centerObject = new GameObject("WaitingList");
        centerObject.transform.SetParent(this.transform);
    }
    
    public void Update()
    {
        rotateSpeed = 90 + Mathf.Clamp(GetBlockNum() * 3f, 0, 180);
        centerObject.transform.Rotate(new Vector3(-1, -1, -1), rotateSpeed * Time.deltaTime);

        if(isAttack && !doUpdating && !doAttacking) Attack(); //攻撃命令が来て、設定中でない場合は攻撃
    }

    public async void AddAttackQueue(List<BaseBlock> blockList, int lineNum) //消されたブロックを攻撃ブロックとして待機リストに追加
    {
        if(token.IsCancellationRequested) return;
        doUpdating = true;
        playAddAttackQueueNum++;

        AddPower(blockList.Count, lineNum);
        attackUI.SetPower(power);
        attackQueueNum += blockList.Count;

        UpdateWaitingBlock(token);
        attackUI.SetPos(Mathf.FloorToInt(MathF.Sqrt(attackQueueNum + GetBlockNum())));

        foreach(BaseBlock block in blockList)
        {
            Sequence seq = DOTween.Sequence();
            _ = seq.Append(block.transform.DOMoveX(this.transform.position.x, 0.5f).SetEase(Ease.OutCubic))
            .AppendCallback(() => blockQueue.Add(block))
            .AppendCallback(() => AddWaitingBlock(block));
            await UniTask.Delay(50);
        }
        await UniTask.Delay(450);

        playAddAttackQueueNum--;

        if(playAddAttackQueueNum != 0) return; //AddAttackQueueが複数回実行されている場合は待機

        await CheckEnoughBlocks(token);
        doUpdating = false;
    }
    
    async UniTask CheckEnoughBlocks(CancellationToken token) //必要数を攻撃ブロックに、余りを待機ブロックに設定
    {
        while(CanSquire(blockQueue.Count))
        {
            int needNum = (edge + 1) * (edge + 1) - edge * edge;
            List<BaseBlock> blockList = blockQueue.GetRange(0, needNum);
            blockQueue.RemoveRange(0, needNum);
            await SetSquire(blockList);
            attackQueueNum -= needNum;
            if(edge >= AttM.maxEdge) Attack();
        }
        await UniTask.Delay(500, cancellationToken: token);
        UpdateWaitingBlock(token);
    }

    async void UpdateWaitingBlock(CancellationToken token) //待機ブロックを設定
    {
        float radius = Mathf.FloorToInt(MathF.Sqrt(attackQueueNum + GetBlockNum()));
        _ = centerObject.transform.DOMove(this.transform.position + new Vector3(radius / 2.0f - 1f, radius / 2.0f - 1f, 0), 0.5f);
        
        for(int i = 0; i < blockQueue.Count; i++)
        {
            var point =  ((float)i / attackQueueNum) * 2 * Mathf.PI;
            float x = Mathf.Cos(point) * radius;
            float y = Mathf.Sin(point) * radius;
            
            
            _ = blockQueue[i].transform.DOLocalRotate(new Vector3(0,0, 360 / blockQueue.Count * i + 90), 0.5f).SetEase(Ease.InOutCubic);
            _ = blockQueue[i].transform.DOLocalMove(new Vector3(x, y, 0), 0.5f).SetEase(Ease.InOutCubic);

            await UniTask.Delay(50, cancellationToken: token);
        }
    }

    bool CanSquire(int blockNum)
    {
        int needBlockNum = (edge + 1) * (edge + 1) - edge * edge;
        return blockNum >= needBlockNum;
    }

    async UniTask SetSquire(List<BaseBlock> blockList)
    {
        int x;
        for(int y = 0; y <= edge; y++)
        {
            if(y < edge) x = edge;
            else x = 0;
            for(; x <= edge; x++)
            {
                BaseBlock block = blockList[0];
                blockList.RemoveAt(0);
                block.transform.DOKill();

                Vector3Int index = new Vector3Int(x, y, 0); //正方形に配置する座標
                AddBlock(block, index, false);

                //アニメーション
                _ = block.transform.DOMove(transform.position + index, 1f).SetEase(Ease.OutQuint);
                _ = block.transform.DORotate(new Vector3(-90,0,0), 0.5f).SetEase(Ease.InExpo).OnComplete(() =>
                {
                    AudM.PlayNormalSound(NormalSound.BlockStacking);
                });
                await UniTask.Delay(50);
            }
        }
        edge++;
    }

    void AddPower(int blockNum, int lineNum) 
    {
        this.power += Mathf.FloorToInt(blockNum * lineNum * (0.5f + 0.5f * lineNum));
    }

    public void DoAttack(Enemy enemy) //攻撃命令を出す
    {
        targetEnemy = enemy;
        isAttack = true;
    }

    async void Attack()
    {
        isAttack = false;
        doAttacking = true;
        GameObject video = Resources.Load<GameObject>("video");
        videoObj = Instantiate(video, transform.position + new Vector3((edge * 0.8f) / 2.0f, (edge * 0.8f) / 2.0f, -0.2f), Quaternion.Euler(90,180,0));
        videoObj.transform.localScale = new Vector3(edge / 10.0f, edge / 10.0f, edge / 10.0f);
        videoObj.transform.parent = transform;
        for(int i = 0; i < 8; i++)
        {
            AudM.PlayNormalSound(NormalSound.Hold);
            await UniTask.Delay(200);
        }
        
        if(targetEnemy == null) 
        {
            Destroy();
            return;
        }  

        _ = transform.DOMove(targetEnemy.transform.position + new Vector3(-5,0,0), 0.6f).SetEase(Ease.InBack, 3); //-5は攻撃ブロックの原点が一番左にあるからあくまで仮の値
        await UniTask.Delay(270);
        AudM.PlayNormalSound(NormalSound.ThrowBlock);
        await UniTask.Delay(330);
        if(targetEnemy) 
        {
            targetEnemy.Damage(this);
            AudM.PlayNormalSound(NormalSound.Attack);
        }
        
        doAttacking = false;
        Destroy();
    }

    public override void Destroy()
    {
        Destroy(attackUI.gameObject);
        Destroy(centerObject);
        if(videoObj) Destroy(videoObj);
        foreach(BaseBlock block in BlockList)
        {
            if(doUpdating) block.DOKill();
            block.DestroyBlock(false); //checkValidのおかげでRootBlockも消える
        }
    }

    async void AddWaitingBlock(BaseBlock block) //待機ブロックを設定
    {
        float radius = Mathf.FloorToInt(MathF.Sqrt(attackQueueNum + GetBlockNum()));
        block.transform.parent = centerObject.transform;
        
        var point =  ((float)blockQueue.IndexOf(block) / attackQueueNum) * 2 * Mathf.PI;
        float x = Mathf.Cos(point) * radius;
        float y = Mathf.Sin(point) * radius;
        
        // _ = block.transform.DOLocalRotate(new Vector3(0,0, 360 / blockQueue.Count * index + 90), 0.5f).SetEase(Ease.InOutCubic);
        _ = block.transform.DOLocalMove(new Vector3(x, y, 0), 0.5f).SetEase(Ease.InOutCubic);

        await UniTask.Delay(50, cancellationToken: token);
    }

    void ToOneBlock()
    {
        foreach(BaseBlock block in BlockList)
        {
            ColorType colorType = block.colorType;
            if(colorTypeList.Contains(colorType)) continue;
            colorTypeList.Add(colorType);
            textureList.Add(block.GetComponent<Renderer>().material.mainTexture);
        }

        int edge = Mathf.CeilToInt(Mathf.Sqrt(GetBlockNum()));
        BaseBlock baseBlock = BlockList[0].ReleaseBlock();
        float scale = baseBlock.transform.localScale.x * edge;

        foreach(BaseBlock block in BlockList) BlockPool.ReleaseNotBaseBlock(block);
        baseBlock.transform.localScale = new Vector3(scale, scale, scale);

        TextureChange(baseBlock);
        float pos = ((edge - 1) * 5) / 10;
        pivot.transform.localPosition = new Vector3(pos, pos);
        pivot.transform.rotation = Quaternion.Euler(new Vector3(0,0,180));
        baseBlock.transform.localPosition = Vector3.zero;
    }

    void TextureChange(BaseBlock baseBlock)
    {
        int textureSize = 512; // テクスチャのサイズ
        int count = textureList.Count;
        int subTextureSize = textureSize / count;  // 各テクスチャのサイズ

        // 結果のテクスチャを作成
        Texture2D result = new Texture2D(textureSize, textureSize, TextureFormat.RGBA64, false);

        // 各テクスチャをリサイズしてコピー
        for(int i = 0; i < count; i++)
        {
            Texture texture = textureList[i];
            int x = i * subTextureSize;

            // テクスチャの一部を新しいテクスチャにコピー
            //textureのimport設定にある形式をRGBA64bitにしないとエラーが出る
            Graphics.CopyTexture(texture, 0, 0, x, 0, subTextureSize, textureSize, result, 0, 0, x, 0);
        }

        result.Apply();
        baseBlock.mainRenderer.material.mainTexture = result;
    }
}
