using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Console;

[CreateAssetMenu(fileName = "Frame_", menuName = "CreateFrameData")]
[System.Serializable]
public class FrameData : ScriptableObject
{
    [Header("フレームデータの名前")]
    public new string name;
    [Header("地面のデータ")]
    public List<PosTexture> BGPosList;
    [Header("フレームのデータ")]
    public List<PosTextureType> framePosList;
    [Header("フレームのサイズ")] //フレームのデータと同じ情報をもってしまっている。消したい
    public Vector3Int frameSize;
    [Header("ブロックが移動できる範囲")]
    public BorderInt moveSize;
}

[System.Serializable]
public class PosTexture
{    
    public Vector3Int blockPos;
    public Texture texture;
}

[System.Serializable]
public class PosTextureType : PosTexture
{    
    public BlockType blockType;
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


