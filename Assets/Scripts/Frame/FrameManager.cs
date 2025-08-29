using System;
using System.Collections.Generic;
using UnityEngine;

public class FrameManager : BaseManager
{
    MainGameManager gamM;
    // フレームデータ
    LinkedDictionary<Vector2Int, Block> frame;
    FrameCorner corner;

    public override void Init()
    {
        
    }

    public void StartBattle()
    {
        gamM = StageManager.Instance.GetCurBattle().gamM;
    }

    ///////////////////////////////////
    ///////  Frameに関する処理群 ///////
    ///////////////////////////////////

    public void AddFrame(FrameData frameData, Vector2Int pos)
    {

    }

    // フレームを設定
    public void SetFrame(Vector2Int frameSize)
    {
        frame = new LinkedDictionary<Vector2Int, Block>();
        corner = new FrameCorner(Vector2Int.zero, frameSize);
        Debug.Log($"Frame set with corner: {corner.min} to {corner.max}");
    }

    public BlockResult IsConflict(Vector2Int pos)
    {
        if (!corner.IsInside(pos)) return BlockResult.OutOfRange; // 範囲外の場合は失敗
        if (frame.ContainsKey(pos)) return BlockResult.Conflict;
        return BlockResult.Air;
    }

    public (BlockResult, Block) GetBlock(Vector2Int pos)
    {
        if (!corner.IsInside(pos)) return (BlockResult.OutOfRange, null); // 範囲外の場合は失敗
        if (!frame.ContainsKey(pos)) return (BlockResult.Air, null); // ブロックがない場合は成功
        return (BlockResult.Block, frame[pos]);
    }

    ////////////////////////////////////
    ///// Base Blockに関する処理群 //////
    ////////////////////////////////////

    public FrameResult SetBlock(Block block, Vector2Int framePos)
    {
        if (IsConflict(framePos) != BlockResult.Air) return FrameResult.Conflict; // 範囲外の場合は失敗

        frame.Remove(block);
        frame.Add(framePos, block);
        TryLine(framePos.y);

        return FrameResult.Success;
    }

    public FrameResult DeleteBlock(Block block)
    {
        if (block == null || !frame.ContainsValue(block))
        {
            return FrameResult.Conflict; // ブロックが存在しない場合は失敗
        }

        frame.Remove(block);
        return FrameResult.Success;
    }

    public void TryLine(int y)
    {
        if (CheckLine(y))
        {
            ReleaseLine(y);
            Drop(y + 1);
        }
    }

    public bool CheckLine(int y)
    {
        for (int x = corner.min.x; x <= corner.max.x; x++)
        {
            Block block;
            frame.TryGetValue(new Vector2Int(x, y), out block);
            if (block == null) return false;
            else if (gamM.playerCBlock && gamM.playerCBlock.blocks.ContainsValue(block.effectBlock)) return false;
            else if (block.blockType == EBlockType.Block) continue;
        }
        return true;
    }

    void ReleaseLine(int y)
    {
        for (int x = corner.min.x; x <= corner.max.x; x++)
        {
            Block block;
            frame.TryGetValue(new Vector2Int(x, y), out block);
            if (block == null) continue;
            else if (gamM.playerCBlock && gamM.playerCBlock.blocks.ContainsValue(block.effectBlock)) continue;
            else if (block.blockType == EBlockType.Block) block.effectBlock.Release();
        }
    }

    void Drop(int minY)
    {
        for (int y = minY; y <= corner.max.y; y++)
        {
            for (int x = corner.min.x; x <= corner.max.x; x++)
            {
                Block block;
                frame.TryGetValue(new Vector2Int(x, y), out block);
                if (block == null) continue;
                else if (gamM.playerCBlock && gamM.playerCBlock.blocks.ContainsValue(block.effectBlock)) continue;
                else if (block.blockType == EBlockType.Block) block.effectBlock.Move(new Vector2Int(x, y - 1));
            }
        }
    }

    // フレーム座標をリストのインデックスに変換
    Vector2Int posTransform(Vector2Int pos)
    {
        // 例 その列の一番左のx座標の番号が-5の場合+5して返す。 0ならそのまま返す。
        // y座標が-3の場合は+3して返す。
        throw new NotImplementedException();
    }

    /////////////////////////////////////
    ///////// 演出に関する処理群 /////////
    /////////////////////////////////////

    // ブロックが移動した時や消えた時に何かが起こってほしいブロックはUniRXでこの関数をサブスクライブする
}

public enum FrameResult
{
    Success, // 成功
    Conflict, // 衝突
    OutOfRange, // 範囲外
}

public enum BlockResult
{
    Block, // ブロックがある
    Conflict, // 衝突
    OutOfRange, // 範囲外
    Air // ブロックがない
}
