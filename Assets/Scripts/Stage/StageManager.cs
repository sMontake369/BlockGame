using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : BaseManager
{
    public static StageManager Instance { get; private set; }

    [HideInInspector]
    public CameraManager CamM { get; private set; }
    [HideInInspector]
    public ControllerManager ConM { get; private set; }
    [HideInInspector]
    public AudioManager AudM { get; private set; }
    [HideInInspector]
    public List<BattleManager> batMList { get; private set; }
    public int battleIndex { get; private set; } = 0;
    public string stageName { get; set; }
    public Difficulty difficulty { get; set; }
    public StageData stageData;

    [SerializeField]
    public ContainerBlockData test;

    public void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Init();
    }

    public override void Init()
    {
        ConM = FindFirstObjectByType<ControllerManager>();
        AudM = FindFirstObjectByType<AudioManager>();
        CamM = FindFirstObjectByType<CameraManager>();

        ConM.Init();
        AudM.Init();
        CamM.Init();

        DG.Tweening.DOTween.SetTweensCapacity(tweenersCapacity: 20000, sequencesCapacity: 200);
        batMList = new List<BattleManager>();
        battleIndex = 0;

        InitStage(stageData, Difficulty.Normal);
        
        NextBattle();
    }

    public void InitStage(StageData stageData, Difficulty difficulty)
    {
        this.name = stageData.name;
        this.difficulty = difficulty;

        foreach ((BattleData battleData, int index) in stageData.battleDataList.Select((data, i) => (data, i)))
        {
            BattleManager BatM = new GameObject("BattleManager").AddComponent<BattleManager>();
            BatM.name = "Battle_" + index;
            BatM.transform.SetParent(this.transform);
            batMList.Add(BatM);
            Vector2 lastPos;
            if (index != 0) lastPos = stageData.battleDataList[index - 1].offset;
            else lastPos = Vector2.zero;
            BatM.transform.position = lastPos;

            BatM.Init();
            BatM.Generate(battleData, index);
        }
    }

    void NextBattle()
    {
        if (battleIndex < batMList.Count)
        {
            batMList[battleIndex].StartBattle();
            ConM.NextBattle();
        }
        else ClearStage();
    }

    public void ClearBattle()
    {
        battleIndex++;
        NextBattle();
    }

    public void ClearStage()
    {
        Debug.Log(stageName + " Clear");
        SceneManager.LoadScene("TitleScene");
        return;
    }

    public BattleManager GetCurBattle()
    {
        return batMList[battleIndex];
    }
}
