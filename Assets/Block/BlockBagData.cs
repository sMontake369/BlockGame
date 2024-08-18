using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockShape_", menuName = "CreateBlockShapeData")]
public class BlockBagData : ScriptableObject
{
    public List<RootBlockData> blockDataList;
}
