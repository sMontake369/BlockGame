using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContainerBlock : MonoBehaviour
{
    FrameManager fraM;
    public Vector2 pivotPos { get; private set; } // ブロックの回転の中心
    public Dictionary<Vector2Int, BaseEffectBlock> blocks { get; private set; } // ブロックの形状を表すBaseEffectBlockの辞書

    public void Create(ContainerBlockData data)
    {
        fraM = StageManager.Instance.GetCurBattle().fraM;
        blocks = new Dictionary<Vector2Int, BaseEffectBlock>();
        this.transform.SetParent(fraM.transform);

        foreach (var (shapePos, blockData) in data.blockDataList.GetDictionary)
        {
            BaseEffectBlock block = blockData.CreateEffectBlock();
            AddBlock(block, shapePos);
        }
    }

    public void SetPivotPos(Vector2 pivotPos)
    {
        this.pivotPos = pivotPos;
    }

    // ブロックを追加
    public void AddBlock(BaseEffectBlock block, Vector2Int shapePos)
    {
        blocks[shapePos] = block;
        block.SetParent(this, shapePos);
    }

    public bool Move(Vector2Int frameIndex)
    {
        List<Vector2Int> preFrameIndexes = new List<Vector2Int>();
        List<Vector2Int> preShapeIndexes = new List<Vector2Int>();
        foreach (var (shapeIndex, block) in blocks)
        {
            preFrameIndexes.Add(block.frameIndex);
            preShapeIndexes.Add(shapeIndex);
            block.MoveRelease();
        }

        foreach (var (shapeIndex, block) in blocks)
        {
            FrameResult result = block.Move(frameIndex + shapeIndex);
            if (result != FrameResult.Success)
            {
                ResetPos(preFrameIndexes, preShapeIndexes);
                return false;
            }
        }
        return true;
    }

    //　ブロックを現在の位置からオフセットに移動
    public bool Shift(Vector2Int offset)
    {
        List<Vector2Int> preFrameIndexes = new List<Vector2Int>();
        List<Vector2Int> preShapeIndexes = new List<Vector2Int>();
        foreach (var (shapeIndex, block) in blocks)
        {
            preFrameIndexes.Add(block.frameIndex);
            preShapeIndexes.Add(shapeIndex);
            block.MoveRelease();
        }

        foreach (var (_, block) in blocks)
        {
            FrameResult result = block.Shift(offset);
            if (result != FrameResult.Success)
            {
                ResetPos(preFrameIndexes, preShapeIndexes);
                return false;
            }
        }
        return true;
    }

    // ブロックを回転 
    public bool Rotate(float angle)
    {
        List<Vector2Int> preFrameIndexes = new List<Vector2Int>();
        List<Vector2Int> preShapeIndexes = new List<Vector2Int>();
        
        foreach (var (shapeIndex, block) in blocks)
        {
            preFrameIndexes.Add(block.frameIndex);
            preShapeIndexes.Add(shapeIndex);
            block.MoveRelease();
        }

        var newBlocks = new Dictionary<Vector2Int, BaseEffectBlock>();

        foreach (var (shapePos, block) in blocks)
        {
            Vector2 relativePos = (Vector2)shapePos - pivotPos;
            Vector2 rotatedPos;

            if (angle < 0) rotatedPos = new Vector2(relativePos.y, -relativePos.x);
            else rotatedPos = new Vector2(-relativePos.y, relativePos.x);

            Vector2Int newShapePos = Vector2Int.RoundToInt(pivotPos + rotatedPos);
            Vector2Int newFramePos = block.frameIndex + newShapePos - shapePos;

            newBlocks[newShapePos] = block;

            FrameResult result = block.Rotate(newFramePos);

            if (result != FrameResult.Success)
            {
                ResetPos(preFrameIndexes, preShapeIndexes);
                return false;
            }
        }

        blocks.Clear();
        foreach (var kv in newBlocks) blocks[kv.Key] = kv.Value;
        return true;
    }

    // 位置をframeIndex通りの位置にリセット
    void ResetPos(List<Vector2Int> preFrameIndex, List<Vector2Int> preShapeIndexes)
    {
        foreach (var (_, block) in blocks) block.MoveRelease();

        List<Vector2Int> curShapeIndexes = blocks.Keys.ToList();
        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[preShapeIndexes[i]] = blocks[curShapeIndexes[i]];
            FrameResult result = blocks[curShapeIndexes[i]].Move(preFrameIndex[i]); // ここで元の位置に戻れるはず
            if (result != FrameResult.Success) throw new Exception($"ResetPos failed {curShapeIndexes[i]} to {preFrameIndex[i]}");
        }
    }

    // ブロックを解放
    public void OnGround()
    {
        // TODO 一度OnGroundでtransform.SetParent(fraM.transform);しないと、列がそろったとき、AttackControllerのSetParentで親子関係が変わり
        // その後、OnGroundでtransform.SetParent(fraM.transform)が呼ばれてしまう。
        foreach (BaseEffectBlock block in blocks.Values) block.OnGround();
        foreach (BaseEffectBlock block in blocks.Values) block.CheckLine();
        Destroy(this.gameObject);
    }
    
    public void Release()
    {
        foreach (BaseEffectBlock block in blocks.Values) block.Release();
    }

    public void Destroy()
    {
        Release();
        Destroy(this.gameObject);
    }
}




