using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Frame_", menuName = "CreateFrameData")]
[System.Serializable]
public class FrameData : ScriptableObject
{
    [Header("フレームのサイズ")]
    public Vector3Int frameSize;

    [Header("ブロックが移動できるかとブロックの情報")] //サイズはlength、設置負荷の場合は-2,何もない場合は-1,ブロックの情報は0~割り当てる。
    public List<int> indexDataList;

    [Header("ブロックの情報とテクスチャのリスト")]
    public List<BlockData> blockDataList;
    
}

[System.Serializable]
public class BlockData //ブロックの情報とindexを紐づける
{
    public BlockType blockType;
    public ColorType colorType;
    public Texture texture;

    public BlockData(BlockType blockType = default, ColorType colorType = default, Texture texture = default)
    {
        this.blockType = blockType;
        this.colorType = colorType;
        this.texture = texture;
    }
}

[System.Serializable]
public class BorderInt
{
    public Vector3Int lowerLeft { get { return min; } }
    public Vector3Int upperRight { get { return max; } }
    public Vector3Int lowerRight { get { return new Vector3Int(max.x, min.y, min.z); } }
    public Vector3Int upperLeft { get { return new Vector3Int(min.x, max.y, max.z); } }
    public int width { get { return max.x - min.x; } }
    public int height { get { return max.y - min.y; } }
    
    public Vector3Int min = new Vector3Int(0, 0, 0);
    public Vector3Int max = new Vector3Int(0, 0, 0);

    public BorderInt(Vector3Int min = default, Vector3Int max = default)
    {
        this.min = min;
        this.max = max;
    }

    public void Encapsulate(Vector3Int point)
    {
        min = Vector3Int.Min(min, point);
        max = Vector3Int.Max(max, point);
    }

    public void SetMinMax(Vector3Int min, Vector3Int max)
    {
        this.min = min;
        this.max = max;
    }

    public Vector3Int GetCorner(DirPivot corner)
    {
        switch(corner)
        {
            case DirPivot.left:
                return lowerLeft;
            case DirPivot.right:
                return lowerRight;
            case DirPivot.upper:
                return upperLeft;
            case DirPivot.bottom:
                return lowerLeft;
            case DirPivot.origin:
                return Vector3Int.zero;
        }
        Debug.LogError("Invalid corner");
        return Vector3Int.zero;
    }

    public static BorderInt operator +(BorderInt a, BorderInt b)
    {
        return new BorderInt(a.min + b.min, a.max + b.max);
    }

    public static BorderInt operator -(BorderInt a, BorderInt b)
    {
        return new BorderInt(a.min - b.min, a.max - b.max);
    }
}

public enum DirPivot
{
    upper = default,
    bottom,
    left,
    right,
    origin //移動しない
}


