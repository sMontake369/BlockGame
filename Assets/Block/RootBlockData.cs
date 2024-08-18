using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block_", menuName = "CreateBlockData")]
public class RootBlockData : ScriptableObject //名前をいつかBlockShapeDataに変えたい
{
    public new string name;
    public BlockType blockType;
    public ColorType colorType;
    public Vector3 pivotPos;
    public List<Vector3Int> blockPosList; //名前をいつかposに変えたい
}
