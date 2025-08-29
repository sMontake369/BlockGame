using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Battle_", menuName = "CreateBattleData")]
public class BattleData : ScriptableObject //ステージの内の１バトル分のみのデータ
{
    [Header("バトルデータの名前")]
    public new string name;
    [Header("フレームデータ")]
    public FrameData frameData;
    [Header("敵データ")]
    public List<EnemyData> enemyDataList;
    [Header("このバトルで使用するカラーテクスチャのデータ")]
    public TextureData textureData;
    [Header("このバトルで使用するルートブロックのデータ")]
    public BlockBagData blockShapeData;
    [Header("プレイヤーブロックの生成位置")]
    public Vector2Int blockSpawnPos = new Vector2Int(0,0);
    [Header("ネクストブロックの生成位置&数")]
    public List<Vector3Int> nextBlockPosList = new List<Vector3Int>();
    [Header("ホールドブロックの位置")]
    public Vector3Int holdBlockPos = new Vector3Int(0, 0, 0);
    [Header("一つ前のフレームとの位置のオフセット")]
    public Vector2Int offset;
    [Header("アタックの位置")]
    public Vector2 attackPos;
    [Header("次のフレームを生成する位置&クリア後次に進む方向")]
    public DirPivot direction;
    [Header("カメラの位置")]
    public Vector2 cameraPos;
    [Header("カメラの拡大率")]
    public float orthoSize = 10.0f;
    //public List<GameEvent> battleEventList;
}