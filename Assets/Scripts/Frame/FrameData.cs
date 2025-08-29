using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Frame_", menuName = "CreateFrameData")]
[System.Serializable]
public class FrameData : ScriptableObject
{
    [Header("フレームのサイズ")]
    public Vector2Int frameSize;

    [Header("ブロックが移動できるかとブロックの情報")] //サイズはlength、設置不可の場合は-1,ブロックの情報は0~割り当てる。
    public List<FrameIndexData> frameIndexList;

    [Header("ブロックの情報とテクスチャのリスト")]
    public List<BaseBlockData> blockDataList;
}

[System.Serializable]
public class FrameIndexData
{
    public Vector2Int pos;
    public int dataIndex;
}

[System.Serializable]
public abstract class BaseBlockData : ScriptableObject
{
    public EBlockColor eBlockColor;
    public abstract BaseEffectBlock CreateEffectBlock();
}

[System.Serializable]
public class FrameCorner
{
    public Vector2Int lowerLeft { get { return min; } }
    public Vector2Int upperRight { get { return max; } }
    public Vector2Int lowerRight { get { return new Vector2Int(max.x, min.y); } }
    public Vector2Int upperLeft { get { return new Vector2Int(min.x, max.y); } }
    public int width { get { return max.x - min.x; } }
    public int height { get { return max.y - min.y; } }

    public Vector2Int min;
    public Vector2Int max;

    public FrameCorner(Vector2Int min = default, Vector2Int max = default)
    {
        this.min = min;
        this.max = max;
    }

    public bool IsInside(Vector2Int pos)
    {
        return pos.x >= min.x && pos.x <= max.x && pos.y >= min.y && pos.y <= max.y;
    }

    public void Encapsulate(Vector2Int point)
    {
        min = Vector2Int.Min(min, point);
        max = Vector2Int.Max(max, point);
    }

    public Vector2Int GetCorner(DirPivot corner)
    {
        switch (corner)
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
                return Vector2Int.zero;
        }
        Debug.LogError("Invalid corner");
        return Vector2Int.zero;
    }

    public static FrameCorner operator +(FrameCorner a, FrameCorner b)
    {
        return new FrameCorner(a.min + b.min, a.max + b.max);
    }

    public static FrameCorner operator -(FrameCorner a, FrameCorner b)
    {
        return new FrameCorner(a.min - b.min, a.max - b.max);
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


