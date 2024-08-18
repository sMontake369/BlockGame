using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FrameManager : MonoBehaviour
{
    MainGameManager GamM;
    BattleManager BatM;

    RootBlock frontRBlock;
    RootBlock backRBlock;

    public FrameData frameData { get; private set; }
    public List<List<BaseBlock>> FrameListList { get => frontRBlock.BlockListList; } //いつかpublic消したい

    public BorderInt LMovableBorder { get => frameData.moveSize; private set => frameData.moveSize = value; }
    public BorderInt LFrameBorder { get => frameData.frameSize; private set => frameData.frameSize = value; }
    public BorderInt WMovableBorder { get => BatM.battlePos + frameData.moveSize; }
    public BorderInt WFrameBorder { get => BatM.battlePos + frameData.frameSize; }

    public void Init()
    {
        BatM = FindFirstObjectByType<BattleManager>();
        GamM = FindFirstObjectByType<MainGameManager>();

        if(!BatM || !GamM) 
        {
            Debug.Log("BattleManager or MainGameManager is not found");
            return;
        }
    }

    public void SetFrame(Vector2Int size) //Editor用のフレームを生成 いつか下のGenerateと統合したい
    {
        if(frontRBlock) BlockPool.ReleaseNotRootBlock(frontRBlock);
        
        frameData = ScriptableObject.CreateInstance<FrameData>();
        frontRBlock = GamM.GenerateRBlock();
        frontRBlock.SetBlockSize(size);
        frontRBlock.transform.parent = this.transform;
        frontRBlock.name = "RootFrameBlock";
        LMovableBorder = new BorderInt(new Vector3Int(0, 0, 0), new Vector3Int(size.x, size.y, 0));
    }

    public void SetFrame(FrameData frameData)
    {
        if(frontRBlock) BlockPool.ReleaseNotRootBlock(frontRBlock);
        if(backRBlock) BlockPool.ReleaseNotRootBlock(backRBlock);

        this.frameData = frameData;
        frontRBlock = GamM.GenerateRBlock();
        frontRBlock.transform.position = this.transform.position;
        frontRBlock.transform.parent = this.transform;
        frontRBlock.name = frameData.name;
        //frameRBlock.SetListList(new Vector2Int(frameData.frameSize.max.x, frameData.frameSize.max.y)); addBlockで自動で生成されるはず
        foreach(PosTextureType data in frameData.framePosList)
        {
            BaseBlock baseBlock = GamM.GenerateBlock(data.blockType, ColorType.None, data.texture); //仮
            frontRBlock.AddBlock(baseBlock, data.blockPos);
        }

        backRBlock = GamM.GenerateRBlock();
        backRBlock.transform.position = this.transform.position;
        backRBlock.transform.parent = this.transform;
        backRBlock.name = "BackGroundRBlock";
        //backGroundBlock.SetListList(new Vector2Int(frameData.frameSize.max.x, frameData.frameSize.max.y)); //お試し
        backRBlock.transform.Translate(new Vector3(0, 0, 0.1f));
        foreach(PosTexture data in frameData.BGPosList)
        {
            BaseBlock baseBlock = GamM.GenerateBlock(BlockType.BackGround, ColorType.None, data.texture); //仮
            backRBlock.AddBlock(baseBlock, data.blockPos);
        }

        //Texture texture = Addressables.LoadAssetAsync<Texture>("g1610").WaitForCompletion();
        // 全ての配列にブロックを生成
        // for(int y = 0; y < backGroundBlock.BlockListList.Count; y++)
        // for(int x = 0; x < backGroundBlock.BlockListList[y].Count; x++)
        // {
        //     if(backGroundBlock.BlockListList[y][x] != null) continue;
        //     BaseBlock baseBlock = GamM.GenerateBBlock(BlockType.BackGround, ColorType.None, texture); //仮
        //     backGroundBlock.AddBlock(baseBlock, new Vector3Int(x, y, 0));
        // }
    }

    public bool IsConflict(RootBlock rootBlock, Vector3Int offset) //ブロックが衝突しているかどうか
    {
        foreach(BaseBlock baseBlock in rootBlock.BlockList)
        if(IsConflict(baseBlock, offset)) return true;
        
        return false;
    }

    public bool IsConflict(BaseBlock block, Vector3Int offset) //ブロックが衝突しているかどうか
    {
        if(block == null) return false;
        Vector3Int pos = block.frameIndex + offset;

        //ボード外に出ているかどうか
        if(!IsWithinBoard(pos)) return true;

        //ブロックが存在しているかどうか
        if(FrameListList[pos.y][pos.x] == null ||
        FrameListList[pos.y][pos.x].rootBlock == block.rootBlock || 
        block.rootBlock == FrameListList[pos.y][pos.x].rootBlock.GhostBlock) return false; //ブロックが自分自身か、ゴーストブロックかどうか
        else return true;
    }

    public bool InFrame(RootBlock rootBlock) //ルートブロックがフレームに存在するかどうか これいる？
    {
        foreach(BaseBlock block in rootBlock.BlockList)
        {
            if(!IsWithinBoard(block.frameIndex)) return false;
            if(!FrameListList[block.frameIndex.y][block.frameIndex.x]) return false;
            if(FrameListList[block.frameIndex.y][block.frameIndex.x].rootBlock != rootBlock) return false;
        }
        return true;
    }

    public bool IsWithinBoard(Vector3Int pos) //posがボード内かどうか
    {
        if(LMovableBorder.lowerLeft.x <= pos.x && pos.x < LMovableBorder.upperRight.x && 
        LMovableBorder.lowerLeft.y <= pos.y && pos.y < LMovableBorder.upperRight.y) return true;
        else return false;
    }

    public void DeleteRBlock(RootBlock rootBlock) //ルートブロックをボードから消す
    {
        if(rootBlock == null) return;
        for(int y = 0; y < rootBlock.BlockListList.Count; y++)
        for(int x = 0; x < rootBlock.BlockListList[y].Count; x++)
        DeleteBlock(rootBlock.BlockListList[y][x]);

        if(GamM.playerBlock) GamM.playerBlock.GenerateGhostBlock();
    }

    public void DeleteBlock(BaseBlock block) //ベースブロックをボードから消す
    {
        if(block == null) return;
        if(!IsWithinBoard(block.frameIndex)) return;
        if(FrameListList[block.frameIndex.y][block.frameIndex.x] != null && 
        FrameListList[block.frameIndex.y][block.frameIndex.x].rootBlock == block.rootBlock) 
        {
            FrameListList[block.frameIndex.y][block.frameIndex.x] = null;
        }
    }

    public void DeleteBlocks(Vector2Int from, Vector2Int to) //指定範囲のブロックをボードから消す
    {
        for(int y = from.y; y < to.y; y++)
        for(int x = from.x; x < to.x; x++)
        {
            if(FrameListList[y][x] == null) continue;
            FrameListList[y][x].DestroyBlock();
            FrameListList[y][x] = null;
        }
    }

    public void SetRBlock(RootBlock rootBlock) //ルートブロックをボードにセット　//ベースブロックをボードにセット Vector2Int posを追加すべき
    {
        for(int y = 0; y < rootBlock.BlockListList.Count; y++)
        for(int x = 0; x < rootBlock.BlockListList[y].Count; x++)
        if(rootBlock.BlockListList[y][x] != null) SetBlock(rootBlock.BlockListList[y][x], false);

        if(GamM.playerBlock) GamM.playerBlock.GenerateGhostBlock(); //GamMがプレイヤーブロックを持っているかどうか確認してほしい
    }

    public bool SetBlock(BaseBlock block, bool doUpdateGhost = true) //ベースブロックをボードにセット Vector2Int posを追加すべき
    {
        if(GamM.mainState == MainStateType.idle && block != null) 
        {
            block.DestroyBlock();
            return false;
        }
        if(block == null) return false;
        {
            if(block.frameIndex.y < 0 || block.frameIndex.y > FrameListList.Count || block.frameIndex.x < 0 || block.frameIndex.x > FrameListList[0].Count) return false;
            if(FrameListList[block.frameIndex.y][block.frameIndex.x] != null) return false;
            FrameListList[block.frameIndex.y][block.frameIndex.x] = block;
        }

        if(doUpdateGhost && GamM.playerBlock) GamM.playerBlock.GenerateGhostBlock(); //GamMがプレイヤーブロックを持っているかどうか確認してほしい

        return true;
    }

    public List<RootBlock> GetBlocks(int from, int to) //指定範囲のルートブロックリストを取得 //ラインじゃなくて範囲にしたい
    {
        List<RootBlock> rootBlockList = new List<RootBlock>();
        for(int y = from; y < to; y++)
        for(int x = 0; x < FrameListList[y].Count; x++)
        {
            if(FrameListList[y][x] == null) continue;
            if(FrameListList[y][x].blockType != BlockType.Mino) continue;
            if(FrameListList[y][x].rootBlock == GamM.playerBlock) continue;
            if(!rootBlockList.Contains(FrameListList[y][x].rootBlock)) rootBlockList.Add(FrameListList[y][x].rootBlock);
        }
        return rootBlockList;
    }
}