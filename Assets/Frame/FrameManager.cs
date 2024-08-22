using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FrameManager : MonoBehaviour
{
    MainGameManager GamM;
    BattleManager BatM;

    public FrameData frameData { get; private set; }
    BorderInt frameBorder;
    BorderInt movableBorder;
    
    List<List<FrameBox>> frameListList = new List<List<FrameBox>>();
    //public List<List<FrameBox>> FrameListList { get => frameListList; private set => FrameListList = value; }

    public BorderInt LMovableBorder { get => movableBorder; }
    public BorderInt LFrameBorder { get => frameBorder; }
    public BorderInt WMovableBorder { get => BatM.battlePos + movableBorder; }
    public BorderInt WFrameBorder { get => BatM.battlePos + frameBorder; }

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

    public void EditFrame(Vector2Int size) //Editor用のフレームを生成 いつか下のGenerateと統合したい
    {
        // if(frameBox) frameBox.Destroy();
        
        // frameData = ScriptableObject.CreateInstance<FrameData>();
        // frameBox = new GameObject("FrameRBlock").AddComponent<FrameBox>();
        // frameBox.SetBlock(GamM.GenerateRBlock());
        // //frameRBlock.SetBlockSize(size); よくないかも
        // frameBox.transform.parent = this.transform;
        // frameBox.name = "RootFrameBlock";
        // LMovableBorder = new BorderInt(new Vector3Int(0, 0, 0), new Vector3Int(size.x, size.y, 0));
    }

    public void SetFrame(FrameData frameData)
    {
        this.frameData = frameData;
        frameBorder = new BorderInt(Vector3Int.zero, frameData.frameSize);
        movableBorder = frameData.moveSize;

        //frameListListを再生成
        DeleteAllBlocks();
        frameListList.Clear();
        frameListList = FrameUtility.Generate<FrameBox>(frameData.frameSize.x + 1, frameData.frameSize.y + 1);

        for(int y = 0; y < frameListList.Count; y++)
        for(int x = 0; x < frameListList[y].Count; x++)
        frameListList[y][x] = new FrameBox();

        foreach(PosTextureType data in frameData.framePosList)
        {
            BaseBlock baseBlock = GamM.GenerateBlock(data.blockType, ColorType.None, data.texture); //仮
            baseBlock.transform.parent = this.transform;
            baseBlock.transform.position = this.transform.position + new Vector3(data.blockPos.x, data.blockPos.y, 0);
            frameListList[data.blockPos.y][data.blockPos.x].SetBlock(baseBlock);
        }
    }

    public void DeleteAllFrame()
    {
        DeleteAllBlocks();
        frameListList.Clear();
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
        if(!frameListList[pos.y][pos.x].IsContain()) return false;
        if(frameListList[pos.y][pos.x].BaseBlock.RootBlock == block.RootBlock || 
        frameListList[pos.y][pos.x].BaseBlock.blockType == BlockType.Ghost ||
        frameListList[pos.y][pos.x].BaseBlock.RootBlock == GamM.playerBlock) return false; //ブロックが自分自身か、ゴーストブロックかどうか
        else return true;
    }

    public bool IsWithinBoard(Vector3Int pos) //posがボード内かどうか
    {
        if(LMovableBorder.lowerLeft.x <= pos.x && pos.x <= LMovableBorder.upperRight.x && 
        LMovableBorder.lowerLeft.y <= pos.y && pos.y <= LMovableBorder.upperRight.y) return true;
        else return false;
    }

    public RootBlock DeleteRBlock(RootBlock rootBlock) //ルートブロックをボードから消す
    {
        if(rootBlock == null) return null;
        for(int y = 0; y < rootBlock.BlockListList.Count; y++)
        for(int x = 0; x < rootBlock.BlockListList[y].Count; x++)
        DeleteBlock(rootBlock.BlockListList[y][x]);

        if(GamM.playerBlock) GamM.playerBlock.GenerateGhostBlock();
        return rootBlock;
    }

    public BaseBlock DeleteBlock(BaseBlock block) //ベースブロックをフレームから消す よそのブロックは消せない
    {
        if(block == null) return null;
        if(!IsWithinBoard(block.frameIndex)) return null;
        if(frameListList[block.frameIndex.y][block.frameIndex.x].IsContain() && 
        frameListList[block.frameIndex.y][block.frameIndex.x].BaseBlock.RootBlock == block.RootBlock) 
        {
            return frameListList[block.frameIndex.y][block.frameIndex.x].Delete();
        }
        return null;
    }

    public List<BaseBlock> DeleteBlocks(Vector2Int from, Vector2Int to) //指定範囲のブロックをボードから消す
    {
        List<BaseBlock> blockList = new List<BaseBlock>();
        for(int y = from.y; y < to.y; y++)
        for(int x = from.x; x < to.x; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            blockList.Add(frameListList[y][x].Delete());
        }
        return blockList;
    }

    public List<BaseBlock> DeleteAllBlocks() //全てのブロックをボードから消す
    {
        List<BaseBlock> blockList = new List<BaseBlock>();
        for(int y = 0; y < frameListList.Count; y++)
        for(int x = 0; x < frameListList[y].Count; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            blockList.Add(frameListList[y][x].Delete());
        }
        return blockList;
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
        if(block.frameIndex.y < 0 || block.frameIndex.y > frameListList.Count || block.frameIndex.x < 0 || block.frameIndex.x > frameListList[0].Count) return false;
        if(frameListList[block.frameIndex.y][block.frameIndex.x].IsContain()) return false;

        frameListList[block.frameIndex.y][block.frameIndex.x].SetBlock(block);

        if(doUpdateGhost && GamM.playerBlock) GamM.playerBlock.GenerateGhostBlock(); //GamMがプレイヤーブロックを持っているかどうか確認してほしい

        return true;
    }

    public BaseBlock GetBlock(Vector3Int pos) //指定位置のベースブロックを取得
    {
        if(!IsWithinBoard(pos)) return null;
        if(!frameListList[pos.y][pos.x].IsContain()) return null;
        return frameListList[pos.y][pos.x].BaseBlock;
    }

    public List<List<BaseBlock>> GetBlocks(Vector3Int from, Vector3Int to) //指定範囲のベースブロックリストを取得
    {
        if(!IsWithinBoard(from) && !IsWithinBoard(to)) return null;
        List<List<BaseBlock>> blockListList = new List<List<BaseBlock>>();
        for(int y = from.y; y < to.y; y++)
        {
            List<BaseBlock> blockList = new List<BaseBlock>();
            for(int x = from.x; x < to.x; x++)
            {
                if(!frameListList[y][x].IsContain()) continue;
                blockList.Add(frameListList[y][x].BaseBlock);
            }
            blockListList.Add(blockList);
        }
        return blockListList;
    }

    public List<BaseBlock> GetBlockLine(int y)
    {
        if(y < 0 || y >= frameListList.Count) return null;
        List<BaseBlock> blockList = new List<BaseBlock>();
        for(int x = 0; x < frameListList[y].Count; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            blockList.Add(frameListList[y][x].BaseBlock);
        }
        return blockList;
    }

    public List<RootBlock> GetRBlockLine(int y)
    {
        if(y < 0 || y >= frameListList.Count) return null;
        List<RootBlock> rootBlockList = new List<RootBlock>();
        for(int x = 0; x < frameListList[y].Count; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            if(frameListList[y][x].BaseBlock.RootBlock == GamM.playerBlock) continue;
            if(!rootBlockList.Contains(frameListList[y][x].BaseBlock.RootBlock)) rootBlockList.Add(frameListList[y][x].BaseBlock.RootBlock);
        }
        return rootBlockList;
    }

    public List<RootBlock> GetRBlocks(Vector3Int from, Vector3Int to) //指定範囲のルートブロックリストを取得 //ラインじゃなくて範囲にしたい なくてもいいかも
    {
        if(!IsWithinBoard(from) || !IsWithinBoard(to)) return null;
        List<RootBlock> rootBlockList = new List<RootBlock>();
        for(int y = from.y; y < to.y; y++)
        for(int x = from.x; x < to.x; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            if(frameListList[y][x].BaseBlock.blockType != BlockType.Mino) continue;
            if(frameListList[y][x].BaseBlock.RootBlock == GamM.playerBlock) continue;
            if(!rootBlockList.Contains(frameListList[y][x].BaseBlock.RootBlock)) rootBlockList.Add(frameListList[y][x].BaseBlock.RootBlock);
        }
        return rootBlockList;
    }

    public void OnPlayerGround() //プレイヤーブロックが地面に着地した時
    {
        for(int y = 0; y < frameListList.Count; y++)
        for(int x = 0; x < frameListList[y].Count; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            frameListList[y][x].OnPlayerGround();
        }
    }
}