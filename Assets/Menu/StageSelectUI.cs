using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageSelectUI : MonoBehaviour
{
    public TextMeshProUGUI stageName;

    public void SetStageData(StageData stageData)
    {
        stageName.text = stageData.name;
    }
}
