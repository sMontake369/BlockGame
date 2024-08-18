using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class StageMenu : MonoBehaviour
{
    public StageData LoadStage(string stageName)
    {
        return Addressables.LoadAssetAsync<StageData>(stageName).WaitForCompletion();
    }

    public void StartStage(StageData stageData)
    {
        //BattleDataを受け取ってゲームを開始する
        GameInformation.stageData = stageData;
        SceneManager.LoadScene("GameScene");
    }
}

public static class GameInformation
{
    public static StageData stageData = Addressables.LoadAssetAsync<StageData>("Stage_0").WaitForCompletion();
    public static string difficulty;
}
