using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEditor.SceneManagement;

[CreateAssetMenu(fileName = "SpawnBlockData", menuName = "Effect/CreateSpawnBlockData")]
public class SpawnBlockData : BaseEffectData
{
    public List<RootBlockData> blockDataList;
    public SpawnType spawnType;

    MainGameManager GamM;
    FrameManager FraM;
    BattleManager BatM;

    public override void Init()
    {
        StageManager StaM = FindFirstObjectByType<StageManager>();
        BatM = StaM.GetCurBattle();
        GamM = BatM.GamM;
        FraM = BatM.FraM;
    }

    public override async void Execute(Enemy enemy) //ブロックを生成
    {
        if(blockDataList.Count == 0) 
        {
            Debug.LogError("攻撃するブロックが設定されていません");
            return;
        }
        Vector3Int pos = Vector3Int.zero;
        //設置する位置と回転をランダムに決定
        switch(spawnType)
        {
            case SpawnType.RandomPos:
                pos = new Vector3Int(Random.Range(0, FraM.LFrameBorder.width), Random.Range(0, FraM.LFrameBorder.height - 8), 0);
                break;
            case SpawnType.FromSpawnPos:
                pos = new Vector3Int(Random.Range(0, FraM.LFrameBorder.width), BatM.battleData.blockSpawnPos.y, 0);
                break;
            case SpawnType.SandBlock:
                pos = new Vector3Int(Random.Range(0, FraM.LFrameBorder.width), Random.Range(0, FraM.LFrameBorder.height - 8), 0);
                break;
        }
        Vector3Int rot = new Vector3Int(0, 0, Random.Range(0, 4));

        //ブロックを生成
        RootBlock rootBlock = GamM.GenerateRBlock(blockDataList[Random.Range(0, blockDataList.Count)]);
        rootBlock.name = "SpawnBlock";
        rootBlock.transform.position = enemy.transform.position;
        foreach(List<BaseBlock> blockList in rootBlock.BlockListList)
        foreach(BaseBlock baseBlock in blockList)
        if(baseBlock != null) baseBlock.frameIndex = FraM.LFrameBorder.lowerLeft + pos + baseBlock.shapeIndex;

        //先に回転
        rootBlock.SRS(rot);

        //この時点での衝突判定
        for(int y = 0; y < rootBlock.BlockListList.Count; y++)
        for(int x = 0; x < rootBlock.BlockListList[y].Count; x++)
        {
            if(rootBlock.BlockListList[y][x] == null) continue;
            if(FraM.IsConflict(rootBlock.BlockListList[y][x], Vector3Int.zero))
            {
                rootBlock.BlockListList[y][x].DestroyBlock(false);
            }
        } 
        if(!rootBlock.isActiveAndEnabled) return;

        //ゴーストブロックを生成
        rootBlock.GenerateGhostBlock(false);
        rootBlock.GhostBlock.ResetTransformAndIndex();
        rootBlock.GhostBlock.transform.position = BatM.battlePos.lowerLeft + pos;

        //ブロックの所定の位置に移動
        rootBlock.transform.localScale = Vector3.zero;
        _ = rootBlock.transform.DOScale(1, 0.4f).SetEase(Ease.OutBounce);
        await rootBlock.transform.DOJump(FraM.WFrameBorder.lowerLeft + pos, 10, 1, 1.0f).SetEase(Ease.InExpo).SetEase(Ease.InQuint);

        //ゴーストブロックを消す
        rootBlock.DestroyGhostBlock();
        
        //もう一度衝突判定
        for(int y = 0; y < rootBlock.BlockListList.Count; y++)
        for(int x = 0; x < rootBlock.BlockListList[y].Count; x++)
        {
            if(rootBlock.BlockListList[y][x] == null) continue;
            if(FraM.IsConflict(rootBlock.BlockListList[y][x], Vector3Int.zero))
            {
                rootBlock.BlockListList[y][x].DestroyBlock(false);
            }
        }
        if(!rootBlock || !rootBlock.isActiveAndEnabled) return;

        if(spawnType == SpawnType.SandBlock) rootBlock = GamM.RootConvert<SandBlock>(rootBlock);

        FraM.SetRBlock(rootBlock);

        if(spawnType == SpawnType.FromSpawnPos)
        {
            int downNum = 0;
            while(!rootBlock.Transform(Vector3Int.down))
            {
                downNum++;
                if(downNum > 100)
                {
                    Debug.Log("落ちすぎ");
                    rootBlock.transform.position = new Vector3(-5, 0, 0);
                    break;
                }
            }
        }
    }
}

public enum SpawnType
{
    RandomPos,
    FromSpawnPos,
    SandBlock
}