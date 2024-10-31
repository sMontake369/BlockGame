using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBlock : MonoBehaviour
{
    public MainGameManager GamM;
    public FrameManager FraM;
    public RootBlockData blockData; //自身のブロックのデータ

    public GameObject pivot;
    GhostBlock ghostBlock;
    public GhostBlock GhostBlock { get { return ghostBlock; } }
    List<List<BaseBlock>> blockListList = new List<List<BaseBlock>>(); //ブロックのリスト
    public List<List<BaseBlock>> BlockListList { get { return blockListList;} }
    public List<BaseBlock> BlockList { get { return GetBaseBlockList(); } }

    [SerializeField]
    public int generationNum = 0; //世代番号

    public virtual void Init(MainGameManager gamM, FrameManager fraM)
    {
        this.GamM = gamM;
        this.FraM = fraM;
    }

    public virtual bool Transform(Vector3Int offset) //ブロックを移動(ボードを更新)
    {
        foreach(BaseBlock baseBlock in BlockList) if(!baseBlock.canMove) return true; //移動できないブロックがある場合

        if(FraM.IsConflict(this, offset)) return true;
        FraM.DeleteRBlock(this);
        Move(offset);
        FraM.SetRBlock(this);
        return false;
    }

    public virtual void Rotation(Vector3 offset) //ブロックを回転(ボードを更新)
    {
        FraM.DeleteRBlock(this);
        SRS(offset);
        if(FraM.IsConflict(this,Vector3Int.zero))
        {
            SRS(-offset);
        }
        FraM.SetRBlock(this);
    }

    public virtual void Move(Vector3Int offset) //ブロックを移動(ボードを更新しない)
    {
        transform.localPosition += offset;
        foreach(List<BaseBlock> blockList in BlockListList)
        foreach(BaseBlock baseBlock in blockList)
        {
            if(baseBlock == null) continue;
            baseBlock.frameIndex += offset;
        }
    }

    public virtual void SRS(Vector3 offset) //SRSによる回転
    {
        List<Vector3> prePosList = new List<Vector3>();
        foreach(List<BaseBlock> blockList in BlockListList)
        foreach(BaseBlock baseBlock in blockList)
        {
            if(baseBlock == null) continue;
            prePosList.Add(baseBlock.transform.position);
        }
        pivot.transform.Rotate(offset * 90);

        int index = 0;
        foreach(List<BaseBlock> blockList in BlockListList)
        foreach(BaseBlock baseBlock in blockList)
        {
            if(baseBlock == null) continue;
            baseBlock.frameIndex += Vector3Int.RoundToInt(baseBlock.transform.position - prePosList[index]);
            index++;
        }
    }

    public void CheckOutline()
    {

    }

    public void CheckExistBlock()
    {
        if(GetBlockNum() == 0) BlockPool.ReleaseNotRootBlock(this);
    }

    public void CheckDivision()
    {
        if(GetBlockNum() == 0)
        {
            BlockPool.ReleaseNotRootBlock(this);
            return;
        }

        List<BaseBlock> baseBlockList = new List<BaseBlock>();
        Vector2Int index = new Vector2Int(0, 0);
        
        for(int y = 0; y < blockListList.Count; y++)
        for(int x = 0; x < blockListList[y].Count; x++)
        if(blockListList[y][x] != null) index = new Vector2Int(x, y);
        baseBlockList.Add(blockListList[index.y][index.x]);
        CheckNeighbor(baseBlockList, index);

        if(baseBlockList.Count != GetBlockNum())
        {
            DivideBlock(baseBlockList);
            //CheckValidBlock(); たまに固まる
        }
    }

    public void CheckNeighbor(List<BaseBlock> baseBlockList, Vector2Int index) //隣接するブロックをチェック
    {
        List<Vector2Int> neighborIndexList = new List<Vector2Int>()
        {
            new Vector2Int(0, 1), //上
            new Vector2Int(1, 0), //右
            new Vector2Int(0,-1), //下
            new Vector2Int(-1, 0) //左
        };

        foreach(Vector2Int neighborIndex in neighborIndexList)
        {
            Vector2Int newIndex = index + neighborIndex;
            if(newIndex.y < 0 || newIndex.y >= blockListList.Count || newIndex.x < 0 || newIndex.x >= blockListList[newIndex.y].Count) continue; //範囲外
            if(blockListList[newIndex.y][newIndex.x] != null && !baseBlockList.Contains(blockListList[newIndex.y][newIndex.x])) //隣接するブロックがある
            {
                baseBlockList.Add(blockListList[newIndex.y][newIndex.x]);
                CheckNeighbor(baseBlockList, newIndex);
            }
        }
    }

    void DivideBlock(List<BaseBlock> baseBlockList)
    {
        FraM.DeleteRBlock(this);

        RootBlock DRootBlock = GamM.GenerateRBlock(null);
        DRootBlock.generationNum = generationNum;
        DRootBlock.name = "DRootBlock";
        DRootBlock.transform.position = transform.position;
        DRootBlock.pivot.transform.SetPositionAndRotation(pivot.transform.position, pivot.transform.rotation);

        foreach(BaseBlock baseBlock in baseBlockList)
        {
            if(baseBlock == null) continue;
            for(int y = 0; y < blockListList.Count; y++)
            for(int x = 0; x < blockListList[y].Count; x++)
            if(blockListList[y][x] == baseBlock)
            {
                blockListList[y][x].transform.parent = null;
                blockListList[y][x] = null;
                break;
            }
            DRootBlock.AddBlock(baseBlock, baseBlock.shapeIndex, false);
        }       
        FraM.SetRBlock(this);
        FraM.SetRBlock(DRootBlock);
    }

    public int GetBlockNum() //ブロックの数を取得
    {
        int blockNum = 0;
        foreach(List<BaseBlock> blockList in blockListList)
        foreach(BaseBlock baseBlock in blockList)
        if(baseBlock != null) blockNum++;
        return blockNum;
    }

    public void SetBlockSize(Vector2Int size)
    {
        blockListList.Clear();
        blockListList = new List<List<BaseBlock>>();
        for(int i = 0; i < size.y; i++) 
        {
            blockListList.Add(new List<BaseBlock>());
            for(int j = 0; j < size.x; j++) blockListList[i].Add(null);
        }
    }

    public virtual void AddBlock(BaseBlock baseBlock, Vector3Int shapeIndex, bool setPos = true) //ブロックを追加　setPos = falseの場合、ブロックの位置を設定
    {
        //ブロックの位置が二次元配列の範囲外の場合、リストを拡張
        if(shapeIndex.y >= blockListList.Count) 
        for(int i = blockListList.Count; i <= shapeIndex.y; i++) blockListList.Add(new List<BaseBlock>(4));

        if(shapeIndex.x >= blockListList[shapeIndex.y].Count) 
        for(int i = blockListList[shapeIndex.y].Count; i <= shapeIndex.x; i++) blockListList[shapeIndex.y].Add(null);

        //ブロックを追加
        blockListList[shapeIndex.y][shapeIndex.x] = baseBlock;

        //ブロックの位置を設定するため、一度RootBlockを親に設定。　このあとpivotを親にするため二度手間、どうにかしたい
        baseBlock.transform.parent = this.transform;
        if(setPos) baseBlock.transform.localPosition = shapeIndex;

        baseBlock.transform.parent = pivot.transform;        
        baseBlock.SetRootBlock(this);
        baseBlock.shapeIndex = shapeIndex;
    }

    public virtual void Destroy()
    {
        FraM.DeleteRBlock(this);
        foreach(BaseBlock block in BlockList) block.DestroyBlock(false);
        DestroyGhostBlock();
    }

    public void GenerateGhostBlock(bool fall = true) //落下地点を表示するゴーストブロックを生成
    {
        if(ghostBlock == null)//ゴーストブロックがない場合新たに生成する
        {
            ghostBlock = GamM.RootConvert<GhostBlock>(GamM.GenerateRBlock());
            ghostBlock.CopyRootBlock(this);
            if(fall) ghostBlock.FallUntilConflict();
        }
        else //ゴーストブロックがある場合、プレイヤーブロックの位置に合わせる
        {
            if(fall) ghostBlock.FallUntilConflict();
        }

    }

    public void DestroyGhostBlock() //ゴーストブロックを解放
    {
        if(ghostBlock != null)
        {
            ghostBlock.Destroy();
            ghostBlock = null;
        }
    }

    public List<BaseBlock> GetBaseBlockList()
    {
        List<BaseBlock> baseBlockList = new List<BaseBlock>();
        foreach(List<BaseBlock> blockList in blockListList)
        foreach(BaseBlock baseBlock in blockList)
        if(baseBlock != null) baseBlockList.Add(baseBlock);
        return baseBlockList;
    }
}




