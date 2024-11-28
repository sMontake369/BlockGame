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
    public AudioManager AudM { get; private set; }

    public BattleState battleState { get; private set; }

    public BattleData battleData { get; private set; }
    [HideInInspector]
    public TextureData textureData;
    [HideInInspector]
    public BlockBagData blockShapeData;

    public BorderInt battlePos { get; private set; } //battleの位置

    public void Init(StageManager StaM)
    {
        this.StaM = StaM;
        ConM = StaM.ConM;
        CamM = StaM.CamM;
        AudM = StaM.AudM;
        
        battleState = BattleState.Pause;

        if(!StaM || !ConM || !CamM || !AudM) 
        {
            Debug.Log("any manager is not found");
            return;
        }

        GamM = new GameObject("GameManager").AddComponent<MainGameManager>();
        GamM.transform.SetParent(this.transform);
        GamM.transform.SetPositionAndRotation(transform.position, transform.rotation);

        FraM = new GameObject("FrameManager").AddComponent<FrameManager>();
        FraM.transform.SetParent(this.transform);
        FraM.transform.SetPositionAndRotation(transform.position, transform.rotation);

        EneM = new GameObject("EnemyManager").AddComponent<EnemyManager>();
        EneM.transform.SetParent(this.transform);
        EneM.transform.SetPositionAndRotation(transform.position, transform.rotation);

        AttM = new GameObject("AttackManager").AddComponent<AttackManager>();
        AttM.transform.SetParent(this.transform);
        AttM.transform.SetPositionAndRotation(transform.position, transform.rotation);

        GamM.Init(this);
        FraM.Init(this);
        EneM.Init(this);
        AttM.Init(this);
    }

    public void SetData(BattleData battleData)
    {
        this.battleData = battleData;
        textureData = battleData.textureData;
        blockShapeData = battleData.blockShapeData;

        //フレームを生成
        Vector3Int frameSize = FraM.SetFrame(battleData.frameData);

        //battleの位置を設定
        Vector3Int pos = Vector3Int.RoundToInt(this.transform.position);
        battlePos = new BorderInt(pos, pos + frameSize);

        //敵を生成
        EneM.Generate(battleData.enemyDataList);
    }

    public async void PlayBattle() //バトルを開始
    {
        //応急措置
        // EneM.Init(this);
        // EneM.Generate(battleData.enemyDataList);
        //ここまで
        battleState = BattleState.Play;
        
        Vector2 battlePos = this.transform.position;
        CamM.SetPosAndOrtho(battlePos + battleData.cameraPos, battleData.orthoSize); //カメラの設定
        AudM.PlayNormalSound(NormalSound.NextBattle);

        await UniTask.Delay(1000); //カメラの移動待ち

        // // GameUIManager UIM = StaM.UIM;
        // for (int i = 0; i < 3; i++)
        // {
        //     UIM.SetCenterText((3 - i).ToString() , 500);
        //     await UniTask.Delay(500);
        // }
        // UIM.SetCenterText("", 0);

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
