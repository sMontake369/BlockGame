using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldData", menuName = "Menu/WorldData")]
public class WorldData : ScriptableObject
{
    public string worldName;
    public Material material;
    public List<StageData> stageDataList;
}
