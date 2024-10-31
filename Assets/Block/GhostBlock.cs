using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBlock : RootBlock
{
    [SerializeField]
    public RootBlock rootBlock; //ghostBlockの元となるRootBlock

    public override void Init(MainGameManager gamM, FrameManager fraM)
    {
        base.Init(gamM, fraM);
        name = "GhostBlock";
    }

    public override bool Transform(Vector3Int offset) //ブロックを移動(ボードを更新)
    {
        foreach(List<BaseBlock> blockList in BlockListList) //移動できるか
        foreach(BaseBlock baseBlock in blockList)
        {
            if(baseBlock == null) continue;
            if(!baseBlock.canMove) return true;
        }
        if(FraM.IsConflict(this, offset)) return true;
        Move(offset);
        return false;
    }

    public override void Rotation(Vector3 offset) //ブロックを回転(ボードを更新)
    {
        SRS(offset);
        if(FraM.IsConflict(this,Vector3Int.zero)) SRS(-offset);
    }

    public override void Move(Vector3Int offset) //ブロックを移動(ボードを更新しない)
    {
        transform.localPosition += offset;
        foreach(List<BaseBlock> blockList in BlockListList)
        foreach(BaseBlock baseBlock in blockList)
        {
            if(baseBlock == null) continue;
            baseBlock.frameIndex += offset;
        }
    }

    public override void SRS(Vector3 offset) //SRSによる回転
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

    public void CopyRootBlock(RootBlock rootBlock)
    {
        this.rootBlock = rootBlock;

        pivot.transform.localPosition = rootBlock.pivot.transform.localPosition;
        //ゴーストブロックにプレイヤーブロックの情報をコピー
        foreach(BaseBlock block in rootBlock.BlockList)
        {
            BaseBlock baseBlock = GamM.GenerateBlock(block.blockType, block.colorType, block.mainRenderer.material.mainTexture);
            baseBlock.frameIndex = block.frameIndex;
            AddBlock(baseBlock, block.shapeIndex);
        }
    }

    public override void AddBlock(BaseBlock baseBlock, Vector3Int index, bool setPos = true) //ブロックを追加　setPos = falseの場合、ブロックの位置を設定
    {
        base.AddBlock(baseBlock, index, setPos);
        foreach(BaseBlock block in BlockList)
        {
            block.mainRenderer.material.color = new Color(1, 1, 1, 0.6f);
            block.blockType = BlockType.Ghost;
        }
    }

    public override void Destroy()
    {
        foreach(BaseBlock block in BlockList) block.DestroyBlock(false);
    }

    public void ResetTransformAndIndex()
    {
        transform.position = rootBlock.transform.position;
        pivot.transform.rotation = rootBlock.pivot.transform.rotation;
        for(int y = 0; y < BlockListList.Count; y++)
        for(int x = 0; x < BlockListList[y].Count; x++)
        {
            if(BlockListList[y][x] == null || rootBlock.BlockListList[y][x] == null) continue;
            BlockListList[y][x].transform.position = rootBlock.BlockListList[y][x].transform.position;
            BlockListList[y][x].transform.rotation = rootBlock.BlockListList[y][x].transform.rotation;
            BlockListList[y][x].frameIndex = rootBlock.BlockListList[y][x].frameIndex;
        }
    }

    public void FallUntilConflict() //衝突するまで落下
    {
        ResetTransformAndIndex();
        int count = 0;
        while(!Transform(Vector3Int.down))
        {
            count++;
            if(count > 100)
            {
                Debug.Log("落ちすぎ");
                break;
            }
        }
        if(rootBlock.GetType() == typeof(SandBlock)) Move(Vector3Int.down); //応急措置　砂ブロックの場合は+1
    }
}
