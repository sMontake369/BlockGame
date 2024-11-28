using System.Collections;
using System.Collections.Generic;
using ExternPropertyAttributes;
using UnityEngine;
using NaughtyAttributes;

public class WorldShape : MonoBehaviour
{
    int curRot = 0;
    List<StageShape> stageList = new List<StageShape>();

    public List<StageShape> Create(WorldData worldData, int rot)
    {
        List<StageData> stageDataList = worldData.stageDataList;
        for(int i = 0; i < stageDataList.Count; i++)
        {
            StageShape stage = new GameObject("Stage" + i).AddComponent<StageShape>();
            stage.Create(stageDataList[i], 10, rot + i);
            stage.transform.parent = transform;
            stage.gameObject.transform.position = new Vector3(0, 0, 0);
            stage.GetComponent<MeshRenderer>().material = worldData.material;

            stage.gameObject.SetActive(false);
            stageList.Add(stage);
        }
        return stageList;
    }
}
