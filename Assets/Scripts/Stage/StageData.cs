using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage_", menuName = "CreateStageData")]
public class StageData : ScriptableObject
{
    public new string name;
    public string description;
    public List<BattleData> battleDataList;
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}