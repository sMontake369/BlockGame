using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBlock : MonoBehaviour
{
    RootBlock rootBlock; //親のブロック
    public RootBlock RootBlock { get { if(rootBlock) return rootBlock; else return null; }}
    public BlockType blockType; //ブロックの種類
    public ColorType colorType; //ブロックの色
    public Renderer mainRenderer; //レンダラー
    
    public Vector3Int shapeIndex = Vector3Int.zero; //ルートブロックからの位置 
    public Vector3Int frameIndex = Vector3Int.zero; //ボード上でのブロックの位置
    public bool canMove = true; //移動できるか
    //public bool canFall = false; //自由落下するか

    public virtual void Awake()
    {
        mainRenderer = GetComponent<Renderer>();
    }

    void SetOutline() //アウトラインを設定
    {
        
    }

    public void SetRootBlock(RootBlock rootBlock) //ルートブロックを追加
    {
        this.rootBlock = rootBlock;
    }

    public void DeleteRootBlock() //ルートブロックを削除  　いる？
    {
        rootBlock = null;
    }

    public virtual BaseBlock OnDelete(bool checkNeighbor = true) //ブロックが削除される前の処理
    {
        if(!this.isActiveAndEnabled) return null;
        rootBlock.BlockListList[shapeIndex.y][shapeIndex.x] = null;
        transform.parent = null;
        if(checkNeighbor) rootBlock.CheckDivision();
        else rootBlock.CheckExistBlock();
        rootBlock = null;
        return this;
    }

    public BaseBlock ReleaseBlock(bool checkNeighbor = true) //強制的にrootBlockから抜ける
    {
        rootBlock.BlockListList[shapeIndex.y][shapeIndex.x] = null;
        if(checkNeighbor) rootBlock.CheckDivision();
        else rootBlock.CheckExistBlock();
        rootBlock = null;
        return this;
    }

    public void DestroyBlock(bool checkNeighbor = true, bool checkValid = true) //ブロックを削除
    {
        rootBlock.BlockListList[shapeIndex.y][shapeIndex.x] = null;
        BlockPool.ReleaseNotBaseBlock(this);
        if(checkNeighbor) rootBlock.CheckDivision();
        else if(checkValid) rootBlock.CheckExistBlock();
        rootBlock = null;
    }

    public void SetColor(ColorType colorType, Texture texture) //ブロックの色を設定
    {
        this.colorType = colorType;
        mainRenderer.material.mainTexture = texture;
    }
}

public enum BlockType
{
    Air,
    Wall,
    Mino,
    BackGround,
    Ghost,
}

public enum ColorType
{
    SkyBlue,
    Red,
    Orange,
    Blue,
    Green,
    Yellow,
    Purple,
    None
}