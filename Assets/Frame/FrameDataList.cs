using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FrameDataList_", menuName = "CreateFrameDataList_")]
public class FrameDataList : ScriptableObject
{
    public List<FrameData> blockDataList;
}
