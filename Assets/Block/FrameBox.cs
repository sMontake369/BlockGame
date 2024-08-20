using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameBox
{
    BaseBlock baseBlock;
    public BaseBlock BaseBlock { get { return baseBlock; } }

    BaseBlock tmpBlock;
    //public List<List<BaseBlock>> BlockListList { get { return rootBlock.BlockListList; } }


    public void OnPlayerBlockGrounded() //プレイヤーブロックが地面に着地した時
    {

    }

    public void OnSetBlock() //ブロックが自身の位置にセットされた時
    {
        
    }

    public bool SetBlock(BaseBlock baseBlock)
    {
        if(baseBlock == null) return false;
        if(IsContain()) return false;
        this.baseBlock = baseBlock;
        OnSetBlock();
        return true;
    }

    public bool IsContain()
    {
        return baseBlock != null;
    }

    public BaseBlock Delete() //フレームから削除
    {
        BaseBlock tmp = baseBlock;
        baseBlock = null;
        tmpBlock = null;
        return tmp;
    }

    public void DestroyBlock()
    {
        if(baseBlock) baseBlock.DestroyBlock();
        if(tmpBlock) tmpBlock.DestroyBlock();
        baseBlock = null;
        tmpBlock = null;
    }
}
