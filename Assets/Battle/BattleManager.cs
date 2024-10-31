using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class BattleManager : MonoBehaviour
{
    public MainGameManager GamM { get; private set; }
    public AttackManager AttM { get; private set; }
    public StageManager StaM { get; private set; }
    public FrameManager FraM { get; private set; }
    public EnemyManager EneM { get; private set; }
    public CameraManager CamM { get; private set; }
    public ControllerManager ConM { get; private set; }

    public BattleState battleState { get; private set; }

    public BattleData battleData { get; private set; }
    [HideInInspector]
    public TextureData textureData;
    [HideInInspector]
    public BlockBagData blockShapeData;

    public BorderInt battlePos { get; private set; } //battleの位置

    public void Init()
    {
        StaM = transform.parent.GetComponent<StageManager>();
        ConM = transform.parent.GetComponent<ControllerManager>();
        CamM = FindFirstObjectByType<CameraManager>();
        
        battleState = BattleState.Pause;

        if(!StaM || !ConM) 
        {
            Debug.Log("StageManager or ControllerManager is not found");
            return;
        }

        GamM = new GameObject("GameManager").AddComponent<MainGameManager>();
        GamM.name = "GameManager";
        GamM.transform.SetParent(this.transform);
        GamM.transform.SetPositionAndRotation(transform.position, transform.rotation);

        FraM = new GameObject("FrameManager").AddComponent<FrameManager>();
        FraM.transform.SetParent(this.transform);
        FraM.transform.SetPositionAndRotation(transform.position, transform.rotation);

        EneM = new GameObject().AddComponent<EnemyManager>();
        EneM.name = "EnemyManager";
        EneM.transform.SetParent(this.transform);
        EneM.transform.SetPositionAndRotation(transform.position, transform.rotation);

        AttM = new GameObject("AttackManager").AddComponent<AttackManager>();
        AttM.name = "AttackManager";
        AttM.transform.SetParent(this.transform);
        AttM.transform.SetPositionAndRotation(transform.position, transform.rotation);

        GamM.Init();
        EneM.Init();
        AttM.Init();
        FraM.Init();
    }

    public void SetData(BattleData battleData)
    {   
        this.battleData = battleData;
        FraM.name = "Frame" + battleData.name;
        textureData = battleData.textureData;
        blockShapeData = battleData.blockShapeData;

        //フレームを生成
        FrameData frameData = battleData.frameData;
        
        Vector3Int frameSize = FraM.SetFrame(frameData);

        //battleの位置を設定
        Vector3Int pos = Vector3Int.RoundToInt(this.transform.position);
        battlePos = new BorderInt(pos, pos + frameSize);

        //敵を生成
        EneM.Generate(battleData.enemyDataList);
    }

    public async void PlayBattle() //バトルを開始
    {
        battleState = BattleState.Play;
        
        CamM.SetBattleManager(this);
        CamM.SetPosAndOrtho(battleData.cameraPos, battleData.orthoSize); //カメラの設定

        await UniTask.Delay(2000); //カメラの移動待ち

        EneM.PlayEnemy();
        ConM.SetGameManager(GamM);
        GamM.TurnStart();
    }

    public void ClearBattle() //ステージクリア時の処理
    {
        battleState = BattleState.Clear;
        Debug.Log(battleData.name + " Clear!");

        AttM.Reset();
        GamM.ResetBlock();

        StaM.PlayNextBattle();
    }

    public Texture GetTexture(ColorType colorType) //色に対応するテクスチャを取得
    {
        if(textureData == null) return new Texture2D(128, 128);
        switch(colorType)
        {
            case ColorType.SkyBlue:
                return textureData.skyBlue;
            case ColorType.Purple:
                return textureData.purple;
            case ColorType.Yellow:
                return textureData.yellow;
            case ColorType.Green:
                return textureData.green;
            case ColorType.Red:
                return textureData.red;
            case ColorType.Blue:
                return textureData.blue;
            case ColorType.Orange:
                return textureData.orange;
        }
        Debug.Log("色が見つかりません");
        return null;
    }
}

public enum BattleState
{
    Play,
    Pause,
    Clear,
    GameOver
}



/// <summary>
///ゲームの情報が入ってる
/// </summary>
public static class GameStatus
{
    public static int score = 0;
    public static float fallTime = 100.0f;
}
