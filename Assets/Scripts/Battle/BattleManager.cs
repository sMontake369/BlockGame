using UnityEngine;

public class BattleManager : BaseManager
{
    public MainGameManager gamM { get; private set; }
    public AttackManager attM { get; private set; }
    public StageManager staM { get; private set; }
    public FrameManager fraM { get; private set; }
    public EnemyManager eneM { get; private set; }
    public CameraManager camM { get; private set; }
    public ControllerManager conM { get; private set; }
    public AudioManager audM { get; private set; }

    public BattleData battleData { get; private set; } //バトルのデータ
    public int battleIndex { get; private set; } //バトルのインデックス

    public override void Init()
    {
        this.staM = StageManager.Instance;

        conM = staM.ConM;
        camM = staM.CamM;
        audM = staM.AudM;

        gamM = new GameObject("GameManager").AddComponent<MainGameManager>();
        fraM = new GameObject("NewFrameManager").AddComponent<FrameManager>();
        eneM = new GameObject("EnemyManager").AddComponent<EnemyManager>();
        attM = new GameObject("AttackManager").AddComponent<AttackManager>();

        foreach (BaseManager manager in new BaseManager[] { gamM, fraM, eneM, attM })
        {
            manager.transform.SetParent(this.transform);
            manager.transform.SetPositionAndRotation(transform.position, transform.rotation);
            manager.Init();
        }
    }

    public void Generate(BattleData battleData, int index)
    {
        this.battleData = battleData;
        this.battleIndex = index;
        fraM.SetFrame(battleData.frameData.frameSize);
        eneM.Generate(battleData.enemyDataList);
    }

    public void StartBattle()
    {
        camM.SetPosAndOrtho((Vector2)this.transform.position + battleData.cameraPos, battleData.orthoSize); //カメラの設定
        audM.PlayNormalSound(NormalSound.NextBattle);

        fraM.StartBattle();
        eneM.StartBattle();
        gamM.StartBattle();
    }

    public void ClearBattle()
    {
        Debug.Log(battleData.name + " Clear!");
        staM.ClearBattle();
    }

    // public Texture GetTexture(EBlockColor colorType) //色に対応するテクスチャを取得
    // {
    //     if(textureData == null) return new Texture2D(128, 128);
    //     switch(colorType)
    //     {
    //         case EBlockColor.SkyBlue:
    //             return textureData.skyBlue;
    //         case EBlockColor.Purple:
    //             return textureData.purple;
    //         case EBlockColor.Yellow:
    //             return textureData.yellow;
    //         case EBlockColor.Green:
    //             return textureData.green;
    //         case EBlockColor.Red:
    //             return textureData.red;
    //         case EBlockColor.Blue:
    //             return textureData.blue;
    //         case EBlockColor.Orange:
    //             return textureData.orange;
    //     }
    //     Debug.Log("色が見つかりません");
    //     return null;
    // }
}


/// <summary>
///ゲームの情報が入ってる
/// </summary>
public static class GameStatus
{
    public static int score = 0;
    public static float fallTime = 100.0f;
}
