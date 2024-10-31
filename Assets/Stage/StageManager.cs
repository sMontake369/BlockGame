using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    List<BattleManager> battleList;
    CameraManager CamM;
    ControllerManager ConM;

    int battleIndex = 0;


    public StageData stageData;
    public string stageName;

    public void Awake()
    {
        ConM = GetComponent<ControllerManager>();
        CamM = FindFirstObjectByType<CameraManager>();

        CamM.Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(stageData == null) 
        {
            if(stageName == "") return;
            else stageData = ReadWrite.Read<StageData>(stageName);
            if(stageData == null) return;
        }

        battleList = new List<BattleManager>();
        this.name = stageData.name;

        int i = 0;
        foreach(BattleData battleData in stageData.battleDataList_easy)
        {
            BattleManager BatM = new GameObject("BattleManager").AddComponent<BattleManager>();
            BatM.name = "Battle" + battleData.name;
            
            if(i != 0) 
            {
                Vector3 offset = battleData.offset;
                BatM.transform.position = battleList[i - 1].transform.position + offset;
            }
            BatM.transform.SetParent(this.transform);
            BatM.transform.SetPositionAndRotation(transform.position, transform.rotation);
            BatM.Init();
            battleList.Add(BatM);
            if(battleData) BatM.SetData(battleData);
            i++;
        }
        
        battleList[battleIndex].PlayBattle();
    }

    public void PlayNextBattle()
    {
        battleIndex++;
        if(battleIndex < battleList.Count) battleList[battleIndex].PlayBattle();
        else ClearStage();
    }

    public void ClearStage()
    {
        Debug.Log("Stage" + stageData.name + "Clear");
        //SceneManager.LoadScene("TitleScene");
        return;
    }

    public BattleManager GetCurBattle()
    {
        return battleList[battleIndex];
    }
}
