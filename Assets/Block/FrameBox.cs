using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameBox
{
    public bool canMove = true; //移動できるか
    BaseBlock baseBlock;
    public BaseBlock BaseBlock { get { return baseBlock; } }

    BaseBlock tmpBlock; 


    public void OnPlayerGround() //プレイヤーブロックが地面に着地した時
    {

    }

    public void OnSetBlock() //ブロックが自身の位置にセットされた時
    {

    }

    public bool SetBlock(BaseBlock baseBlock)
    {
        if(baseBlock == null) return false;
        if(IsContain()) return false;
        //baseBlockがAirだったらtmpBlockによける
        if(baseBlock.blockType == BlockType.BackGround) tmpBlock = baseBlock;
        this.baseBlock = baseBlock;
        OnSetBlock();
        return true;
    }

    //ブロックがセットされているか
    public bool IsContain()
    {
        if(!canMove) return false;
        if(baseBlock == null) return false;
        if(baseBlock.blockType == BlockType.BackGround) return false;
        return true;
    }

    public BaseBlock Delete() //フレームから削除
    {
        BaseBlock tmp = baseBlock;
        baseBlock = null;

        if(tmpBlock != null) baseBlock = tmpBlock;
        tmpBlock = null;
        return tmp;
    }

    public void Destroy()
    {
        if(baseBlock) baseBlock.DestroyBlock();
        if(tmpBlock) tmpBlock.DestroyBlock();
        baseBlock = null;
        tmpBlock = null;
    }
}
