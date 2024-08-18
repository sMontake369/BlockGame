using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage_", menuName = "CreateStageData")]
public class StageData : ScriptableObject
{
    public new string name;
    public string description;
    public List<BattleData> battleDataList_easy;
    public List<BattleData> battleDataList_normal; 
    public List<BattleData> battleDataList_hard;
}
