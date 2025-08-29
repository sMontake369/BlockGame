// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using DG.Tweening;
// using Cysharp.Threading.Tasks;

// [CreateAssetMenu(fileName = "SpawnBlockData", menuName = "Effect/CreateSpawnBlockData")]
// public class SpawnBlockData : BaseEffectData
// {
//     [Header("生成するブロックのデータ(ランダムで一つ選ばれる)")]
//     public List<ContainerBlockData> blockDataList;
//     [Header("生成タイプ")]
//     public SpawnType spawnType;
//     [Header("特定の位置に生成する場合の座標")]
//     public Vector2Int specificPos;
//     [Header("ランダムに回転するか")]
//     public bool isRandomRot = true;
//     [Header("特定の位置に生成するか")]
//     public bool isSpecificPos = false;

//     MainGameManager GamM;
//     NewFrameManager FraM;
//     BattleManager BatM;
//     AudioManager AudM;

//     [Header("SE")]
//     public AudioClip setBlockSE = default;
//     public AudioClip AttackSE = default;

//     public override void Init(Enemy enemy)
//     {
//         StageManager StaM = FindFirstObjectByType<StageManager>();
//         BatM = StaM.GetCurBattle();
//         GamM = BatM.gamM;
//         FraM = BatM.fraM;
//         AudM = StaM.AudM;
//         this.enemy = enemy;
//     }

//     public override async UniTask Execute() //ブロックを生成
//     {
//         if(blockDataList.Count == 0) 
//         {
//             Debug.LogError("攻撃するブロックが設定されていません");
//             return;
//         }
//         Vector3Int pos = Vector3Int.zero;
//         //設置する位置と回転をランダムに決定
//         switch(spawnType)
//         {
//             case SpawnType.RandomPos:
//                 pos = new Vector3Int(Random.Range(0, FraM.LFrameBorder.width), Random.Range(0, FraM.LFrameBorder.height - 8), 0);
//                 break;
//             case SpawnType.Drop:
//                 pos = new Vector3Int(Random.Range(0, FraM.LFrameBorder.width), BatM.battleData.blockSpawnPos.y, 0);
//                 break;
//             case SpawnType.SandBlock:
//                 pos = new Vector3Int(Random.Range(0, FraM.LFrameBorder.width), Random.Range(0, FraM.LFrameBorder.height - 8), 0);
//                 break;
//         }
        
//         if(isSpecificPos) pos = new Vector3Int(specificPos.x, specificPos.y, 0);
//         Vector3Int rot = Vector3Int.zero;
//         if(isRandomRot) rot = new Vector3Int(0, 0, Random.Range(0, 4));

//         //ブロックを生成
//         ContainerBlock rootBlock = GamM.GenerateCBlock(blockDataList[Random.Range(0, blockDataList.Count)]);
//         rootBlock.name = "SpawnBlock";
//         rootBlock.transform.position = enemy.transform.position;
//         foreach(List<Block> blockList in rootBlock.eBlocks)
//         foreach(Block baseBlock in blockList)
//         if(baseBlock != null) baseBlock.frameIndex = FraM.LFrameBorder.lowerLeft + pos + baseBlock.shapeIndex;

//         //先に回転
//         rootBlock.SRS(rot);

//         //ゴーストブロックを生成
//         rootBlock.GenerateGhostBlock(false);
//         rootBlock.GhostBlock.ResetTransformAndIndex();
//         rootBlock.GhostBlock.transform.position = BatM.battlePos.lowerLeft + pos;

//         //ブロックの所定の位置に移動
//         AudM.PlaySound(AttackSE, 0.5f);
//         rootBlock.transform.localScale = Vector3.zero;
//         _ = rootBlock.transform.DOScale(1.1f, 0.4f);
//         await rootBlock.transform.DOJump(FraM.WFrameBorder.lowerLeft + pos + new Vector3(0, 0, -0.1f), 10, 1, 1.0f).SetEase(Ease.InQuart);
//         rootBlock.DestroyGhostBlock();
//         await rootBlock.pivot.transform.DOScale(1.5f, 0.55f).SetEase(Ease.OutExpo);
//         await rootBlock.pivot.transform.DOScale(1, 0.15f).SetEase(Ease.InQuint);
//         rootBlock.transform.position += new Vector3(0, 0, 0.1f);
//         rootBlock.transform.localScale = Vector3.one;
//         AudM.PlaySound(setBlockSE);

        
//         //もう一度衝突判定
//         for(int y = 0; y < rootBlock.eBlocks.Count; y++)
//         for(int x = 0; x < rootBlock.eBlocks[y].Count; x++)
//         {
//             if(rootBlock.eBlocks[y][x] == null) continue;
//             if(FraM.IsConflict(rootBlock.eBlocks[y][x], Vector3Int.zero))
//             {
//                 rootBlock.eBlocks[y][x].DestroyBlock(false);
//             }
//         }
//         if(!rootBlock || !rootBlock.isActiveAndEnabled) return;

//         if(spawnType == SpawnType.SandBlock) rootBlock = GamM.RootConvert<SandBlock>(rootBlock);

//         FraM.SetRBlock(rootBlock);

//         if(spawnType == SpawnType.Drop)
//         {
//             int downNum = 0;
//             while(!rootBlock.Transform(Vector3Int.down))
//             {
//                 downNum++;
//                 if(downNum > 100)
//                 {
//                     Debug.Log("落ちすぎ");
//                     rootBlock.transform.position = new Vector3(-5, 0, 0);
//                     break;
//                 }
//             }
//         }
//     }
// }

// public enum SpawnType
// {
//     RandomPos,
//     Drop,
//     SandBlock
// }