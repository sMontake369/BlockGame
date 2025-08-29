using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    MainInputAction mainInputAction;
    StageSelectUI stageSelectUI;
    public GameObject player;
    public List<WorldData> worldDataList;
    List<StageShape> stageList = new List<StageShape>();
    List<WorldShape> worldList = new List<WorldShape>();
    List<int> worldFirstStageList = new List<int>();
    int curWorld = 0;
    int curStage = 0;
    int visibilityStart = 0;
    int visibilityEnd = 0;
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainInputAction = new MainInputAction();
        mainInputAction.UI.LeftRight.started += context => ChangeStage(curStage + Mathf.RoundToInt(context.ReadValue<float>()));
        mainInputAction.UI.UpDown.started += context => ChangeWorld(curWorld + Mathf.RoundToInt(context.ReadValue<float>()));
        mainInputAction.UI.Enter.started += context => Play();
        mainInputAction.UI.Back.started += context => SelectStage();
        mainCamera = Camera.main;
        stageSelectUI = GetComponent<StageSelectUI>();
        mainInputAction.Enable();
        Init();
    }

    void Init()
    {
        int rot = 0;
        for (int i = 0; i < worldDataList.Count; i++)
        {
            WorldShape worldShape = new GameObject(worldDataList[i].worldName).AddComponent<WorldShape>();
            worldShape.transform.parent = this.transform;
            worldShape.transform.position = new Vector3(0, 0, 0);
            worldList.Add(worldShape);
            stageList.AddRange(worldShape.Create(worldDataList[i], rot));

            if(i == 0) worldFirstStageList.Add(0);
            else worldFirstStageList.Add(worldFirstStageList[i - 1] + worldDataList[i - 1].stageDataList.Count);
            rot += worldDataList[i].stageDataList.Count;
        }
        SetVisibility(0, 3);
    }

    /// <summary>
    /// ステージを任意の位置に変更する
    /// </summary>
    /// <param name="value">移動するステージの位置</param>
    async void ChangeStage(int value)
    {
        mainInputAction.Disable();
        int moveNum = curStage - value;
        if (moveNum != 0)
        {
            player.transform.DOComplete();
            _ = player.transform.DOLocalJump(player.transform.localPosition, 0.5f, 1, 0.25f);
            for (int i = 0; i < Math.Abs(moveNum); i++) await Rotate(moveNum < 0, 0.2f / Math.Abs(moveNum));
        }
        mainInputAction.Enable();
    }

    /// <summary>
    /// ワールドを変更する
    /// </summary>
    /// <param name="value">移動するワールドの位置</param>
    void ChangeWorld(int value)
    {
        if (value < 0 || value >= worldList.Count) return;
        int stageNum = worldFirstStageList[value];
        ChangeStage(stageNum);
    }

    async UniTask Rotate(bool isLeft, float time)
    {
        if(isLeft && curStage + 1 != stageList.Count - 1)
        {
            if(curStage + 2 < stageList.Count && stageList[curStage + 2].gameObject.activeInHierarchy) SetVisibility(visibilityStart, visibilityEnd + 1);
            if(curStage - 1 > 0 && stageList[curStage - 1].gameObject.activeInHierarchy) SetVisibility(visibilityStart + 1, visibilityEnd);
            // this.transform.Rotate(0, 0, 36);
            await this.transform.DORotate(new Vector3(0, 0, 36), time, RotateMode.WorldAxisAdd);
            curStage++;
            if(curWorld + 1 < worldFirstStageList.Count && worldFirstStageList[curWorld + 1] <= curStage) curWorld++;
        }
        else if(!isLeft && curStage != 0) 
        {
            if(curStage + 2 < stageList.Count && stageList[curStage + 2].gameObject.activeInHierarchy) SetVisibility(visibilityStart, visibilityEnd - 1);
            if(curStage - 1 > 0 && stageList[curStage - 1].gameObject.activeInHierarchy) SetVisibility(visibilityStart - 1, visibilityEnd);
            // this.transform.Rotate(0, 0, -36);
            await this.transform.DORotate(new Vector3(0, 0, -36), time, RotateMode.WorldAxisAdd);
            curStage--;
            if(worldFirstStageList[curWorld] - 1 >= curStage) curWorld--;
        }
    }

    /// <summary>
    /// ステージを表示領域を変更する
    /// </summary>
    /// <param name="start">表示の開始位置</param>
    /// <param name="end">表示の終了位置</param>
    public void SetVisibility(int start, int end)
    {
        if(start < 0 || start >= stageList.Count || end < start || end >= stageList.Count) return;
        for(int i = visibilityStart; i < start; i++) stageList[i].gameObject.SetActive(false);
        for(int i = start; i < end; i++) stageList[i].gameObject.SetActive(true);
        for(int i = end; i < visibilityEnd; i++) stageList[i].gameObject.SetActive(false);
        visibilityStart = start;
        visibilityEnd = end;
    }

    void SelectDiff()
    {
        mainCamera.transform.DOMove(new Vector3(0, 14, -12), 0.5f).SetEase(Ease.OutCubic);
    }

    void SelectStage()
    {
        mainCamera.transform.DOMove(new Vector3(0, 12, -12), 0.5f).SetEase(Ease.OutCubic);
    }

    void Play()
    {
        SendStage.stageData = stageList[curStage].GetStageData();
        SceneManager.LoadScene("GameScene");
    }

    enum Direction
    {
        Left,
        Right,
    }
}


public enum MenuState
{
    StageSelect,
    DifficultySelect,
}

public static class SendStage
{
    public static StageData stageData;
}