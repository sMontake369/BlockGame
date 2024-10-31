using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using DG.Tweening;
using System.Linq;
using UnityEngine.UIElements;

public class MainGameManager : MonoBehaviour
{
    BattleManager BatM;
    FrameManager FraM;
    AttackManager AttM;
    AudioManager AudM;

    private List<int> bagList; //袋リスト

    public RootBlock playerBlock { get; private set; } //プレイヤーブロック
    RootBlock holdBlock; //ホールドブロック

    List<RootBlock> nextRBlockList; //ネクストブロックリスト
    int generationNum; //ブロックの世代数
    bool isLined; //このターンで列が揃ったか


    public MainStateType mainState { get; private set; }

    public void Init()
    {
        BatM = FindFirstObjectByType<BattleManager>();
        FraM = BatM.FraM;
        AttM = BatM.AttM;
        AudM = FindFirstObjectByType<StageManager>().AudM;

        if(!BatM || !FraM || !AttM) 
        {
            Debug.Log("BattleManager or FrameManager or AttackManager is not found");
            return;
        }
        
        nextRBlockList = new List<RootBlock>();
        bagList = new List<int>();
        generationNum = 0;
        mainState = MainStateType.idle;
    }

    /// <summary>
    /// 次のターンを開始
    /// </summary>
    public void TurnStart()
    {
        mainState = MainStateType.running;
        isLined = false;

        playerBlock = GetNextBlock();
        playerBlock.transform.localPosition = BatM.battleData.blockSpawnPos;


        foreach(BaseBlock baseBlock in playerBlock.BlockList)
        baseBlock.frameIndex = BatM.battleData.blockSpawnPos + baseBlock.shapeIndex;

        if(!FraM.SetRBlock(playerBlock)) 
        {
            Debug.Log("ゲームオーバー");
            return;
        }
    }

    /// <summary>
    /// プレイヤーブロックの操作を終了し、ターン終了する
    /// </summary>
    public void TurnEnd()
    {
        if(playerBlock) playerBlock.DestroyGhostBlock();
        playerBlock = null;
        CheckLine(); //playerBlockは列の判定対象外のため、nullにしてからでないと、列が揃っているか判定できない

        if(!isLined) AttM.Attack(); //攻撃

        TurnStart();
    }

    /// <summary>
    /// プレイヤーブロックを生成できるだけ生成し、ネクストブロックを取得
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public RootBlock GetNextBlock(int index = 0) //ネクストブロックを取得
    {
        if(BatM.battleData.nextBlockPosList.Count < index) return null; //index個目のネクストブロックがない場合、nullを返す
        RootBlock nextRBlock;
        if(nextRBlockList.Count <= index) //ブロックの数が足りない場合、新たに生成
        {
            nextRBlockList.Add(GenerateRBlock(BatM.blockShapeData.blockDataList[GetRandomInt()]));
            nextRBlockList.Last().transform.localPosition = BatM.battleData.nextBlockPosList.Last() + new Vector3(0, 0, -10);
        }
        if(nextRBlockList[index] != null) nextRBlock = nextRBlockList[index]; //ほしい奴
        else nextRBlock = GetNextBlock(index + 1); //ほしい奴がない場合、次の奴を取得

        RootBlock nextNextRBlock = GetNextBlock(index + 1); //次の奴
        if(nextNextRBlock != null)
        {
            nextNextRBlock.transform.DOKill();
            nextNextRBlock.transform.DOLocalJump(BatM.battleData.nextBlockPosList[index], 0.5f, 1, 0.3f);
            nextRBlockList[index] = nextNextRBlock;
        }
        nextRBlockList.Remove(nextRBlock);
        nextRBlock.transform.DOKill();
        return nextRBlock;
    }

    /// <summary>
    /// ルートブロックを生成
    /// いつかblockDataがnullの場合はランダムに生成にしたい
    /// </summary>
    /// <param name="blockData"></param>
    /// <returns></returns>
    public RootBlock GenerateRBlock(RootBlockData blockData = null)
    {
        RootBlock rootBlock = BlockPool.Instance.rootPool.Get();
        rootBlock.name = "RootBlock";
        rootBlock.blockData = blockData;
        rootBlock.transform.parent = this.transform;
        rootBlock.transform.rotation = BatM.transform.rotation;
        rootBlock.generationNum = generationNum++;
        rootBlock.Init(this, FraM);

        if(blockData == null) return rootBlock;

        rootBlock.pivot.transform.localPosition = blockData.pivotPos;
        foreach(Vector3Int shapeIndex in blockData.blockPosList)
        {
            BaseBlock block = GenerateBlock(blockData.blockType, blockData.colorType, BatM.GetTexture(blockData.colorType));
            rootBlock.AddBlock(block, shapeIndex);
        }
        return rootBlock;
    }

    /// <summary>
    /// ベースブロックを生成
    /// </summary>
    /// <param name="blockType"></param>
    /// <param name="colorType"></param>
    /// <param name="texture"></param>
    /// <returns></returns>
    public BaseBlock GenerateBlock(BlockType blockType, ColorType colorType, Texture texture)
    {
        BaseBlock block = BlockPool.Instance.blockPool.Get();
        block.name = "BaseBlock";
        block.blockType = blockType;
        block.colorType = colorType;
        block.mainRenderer.material.mainTexture = texture;
        block.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 180) + BatM.transform.rotation.eulerAngles);
        return block;
    }

    int GetRandomInt() //袋からランダムに数を返す
    {
        if(bagList.Count == 0) {
            bagList.Clear();
            for(int i = 0; i < BatM.blockShapeData.blockDataList.Count; i++) bagList.Add(i);
        }

        int random = Random.Range(0, bagList.Count);
        int num = bagList[random];
        bagList.RemoveAt(random);
        return num;
    }

    public void SetHoldBlock()
    {
        if(holdBlock == null)
        {
            FraM.DeleteRBlock(playerBlock);
            holdBlock = playerBlock;
            holdBlock.transform.position = BatM.transform.position + BatM.battleData.holdBlockPos;
            holdBlock.pivot.transform.rotation = Quaternion.identity;
            TurnEnd();
        }
        else
        {
            FraM.DeleteRBlock(playerBlock);
            playerBlock.DestroyGhostBlock();
            RootBlock tempBlock = holdBlock;
            holdBlock = playerBlock;
            playerBlock = tempBlock;

            foreach(BaseBlock baseBlock in playerBlock.BlockList)
            if(baseBlock != null) baseBlock.frameIndex = FraM.LFrameBorder.lowerLeft + Vector3Int.RoundToInt(holdBlock.transform.position) + baseBlock.shapeIndex;

            playerBlock.transform.position = holdBlock.transform.position;
            holdBlock.transform.position = BatM.transform.position + BatM.battleData.holdBlockPos;
            holdBlock.pivot.transform.rotation = Quaternion.identity;

            FraM.SetRBlock(playerBlock);
        }
    }

    public void CheckLine() //ラインが揃っているかチェック
    {
        if(mainState != MainStateType.running) return;
        mainState = MainStateType.checkLine;
        List<int> lineList = new List<int>();
        bool canDelete;

        for(int y = FraM.LFrameBorder.lowerLeft.y; y <= FraM.LFrameBorder.upperRight.y; y++)
        {
            canDelete = false;

            for(int x = FraM.LFrameBorder.lowerLeft.x; x <= FraM.LFrameBorder.upperRight.x; x++)
            {
                BaseBlock baseBlock = FraM.GetBlock(new Vector3Int(x, y, 0));
                if(baseBlock == null || baseBlock.RootBlock == playerBlock) 
                {
                    canDelete = false;
                    break;
                }
                if(baseBlock.blockType == BlockType.Mino) canDelete = true;
            }
            if(canDelete) lineList.Add(y);
        }
        if(lineList.Count > 0) 
        {
            isLined = true;
            DeleteLine(lineList);
        }
        else if(mainState == MainStateType.checkLine) mainState = MainStateType.running;
    }

    async void DeleteLine(List<int> lineList) //ラインを消す
    {
        mainState = MainStateType.deleting;
        List<BaseBlock> curSearchBlockList = new List<BaseBlock>(); //現在探索しているブロックリスト
        List<BaseBlock> nextSearchBlockList = new List<BaseBlock>(); //次に探索するブロックリスト
        List<BaseBlock> deleteBlockList = new List<BaseBlock>(); //削除するブロックリスト
        ColorType colorType; //変更する色

        List<Vector3Int> neighborIndexList = new List<Vector3Int>()
        {
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left
        };

        //最大の世代数を持つルートブロックを取得
        BaseBlock maxGenBlock = null;
        int maxGenerationNum = 0;
        List<BaseBlock> blockList = new List<BaseBlock>(); //ブロックリスト
        foreach(int y in lineList) blockList.AddRange(FraM.GetBlockLine(y));

        foreach(BaseBlock block in blockList)
        {
            if(block.blockType != BlockType.Mino) continue;
            if(block.RootBlock.generationNum > maxGenerationNum) 
            {
                maxGenBlock = block;
                maxGenerationNum = block.RootBlock.generationNum;
            }
        }

        colorType = maxGenBlock.colorType;

        AudM.PlayNormalSound(NormalSound.Lined);
        foreach(BaseBlock baseBlock in blockList)
        {
            BaseBlock deleteBlock = baseBlock.OnDelete(); //削除
            if(deleteBlock != null)
            {
                FraM.DeleteBlock(deleteBlock);
                deleteBlock.SetColor(colorType, BatM.GetTexture(colorType)); // 色を変更
                deleteBlockList.Add(deleteBlock);
            }
            await UniTask.Delay(1);
        }

        // nextSearchBlockList.AddRange(maxGenBlock.RootBlock.BlockList);


        // int count = 0;
        // while(nextSearchBlockList.Count > 0) //隣接するブロックがなくなるまで探索
        // {
        //     await UniTask.Delay(3);
        //     /*
        //     baseBlock.OnDelete()内のrootBlock.BlockListList[shapeIndex.y][shapeIndex.x] = null;
        //     を実行する前に1ms以上待機しないと、SetRBlock内でCheckLine()を実行するときに、クラッシュする
        //     */
        //     foreach(BaseBlock baseBlock in nextSearchBlockList) 
        //     {
        //         BaseBlock deleteBlock = baseBlock.OnDelete(); //削除　　　<- ここ
        //         if(deleteBlock != null)
        //         {
        //             FraM.DeleteBlock(deleteBlock);
        //             deleteBlock.SetColor(colorType, BatM.GetTexture(colorType)); // 色を変更
        //             deleteBlockList.Add(deleteBlock);
        //         }
        //     }

        //     curSearchBlockList.Clear();
        //     curSearchBlockList.AddRange(nextSearchBlockList); //次に探索するブロックリストを現在の探索ブロックリストに変更
        //     nextSearchBlockList.Clear();

        //     foreach(BaseBlock baseBlock in curSearchBlockList)
        //     {
        //         if(baseBlock == null) continue;
        //         foreach(Vector3Int neighborIndex in neighborIndexList)
        //         {
        //             Vector3Int searchIndex = baseBlock.frameIndex + neighborIndex;
        //             if(!lineList.Contains(searchIndex.y)) continue; //削除するラインに含まれているかどうか

        //             BaseBlock searchBlock = FraM.GetBlock(searchIndex); //隣接するブロックを取得
        //             if(searchBlock == null) continue;
        //             if(searchBlock.blockType == BlockType.Mino && !deleteBlockList.Contains(searchBlock)) //削除するブロックリストに含まれているかどうか
        //             nextSearchBlockList.Add(searchBlock);
        //         }
        //     }
        //     count++;
        //     if(count > 100) 
        //     {
        //         Debug.Log("無限ループ");
        //         break;
        //     } 
        // }

        if(mainState != MainStateType.idle) 
        {
            AttM.AddAttackQueue(deleteBlockList, lineList.Count); //攻撃ブロックを生成
        }
        RowDown(lineList);
    }

    async void RowDown(List<int> lineList) //ラインを下にずらす
    {
        lineList.Sort();
		lineList.Reverse();
        foreach(int y in lineList)
        {
            await UniTask.Delay(50);
            List<RootBlock> rootBlockList = FraM.GetRBlocks(new Vector3Int(0, y + 1, 0), FraM.LFrameBorder.max);
            foreach(RootBlock rootBlock in rootBlockList) FraM.DeleteRBlock(rootBlock);
            foreach(RootBlock rootBlock in rootBlockList) 
            {
                rootBlock.Transform(Vector3Int.down);
                FraM.SetRBlock(rootBlock); //下に落ちれなかった場合、SetBlockされないため、応急措置
            }
        }
        if(mainState != MainStateType.idle) mainState = MainStateType.running;
    }

    public void ResetBlock() //コントロールブロックを破棄
    {
        mainState = MainStateType.idle;
        
        if(playerBlock != null) 
        {
            playerBlock.DestroyGhostBlock();
            BlockPool.ReleaseNotRootBlock(playerBlock);
        }
        if(holdBlock != null) BlockPool.ReleaseNotRootBlock(holdBlock);
        playerBlock = null;
        holdBlock = null;
        foreach(RootBlock rootBlock in nextRBlockList)
        if(rootBlock != null) BlockPool.ReleaseNotRootBlock(rootBlock);
        nextRBlockList.Clear();
    }

    public T BlockConvert<T>(BaseBlock oldBlock) where T : BaseBlock
    {
        BaseBlock newBlock = oldBlock.AddComponent<T>();
        newBlock.blockType = oldBlock.blockType;
        newBlock.frameIndex = oldBlock.frameIndex;
        oldBlock.RootBlock.AddBlock(newBlock, oldBlock.shapeIndex, false);

        FraM.DeleteBlock(oldBlock);
        DestroyImmediate(oldBlock);
        FraM.SetBlock(newBlock);

        return newBlock as T;
    }

    public T RootConvert<T>(RootBlock oldRootBlock) where T : RootBlock
    {
        T newRootBlock = oldRootBlock.gameObject.AddComponent<T>();
        newRootBlock.Init(this, FraM);
        newRootBlock.pivot = oldRootBlock.pivot;

        foreach(BaseBlock baseBlock in oldRootBlock.BlockList) newRootBlock.AddBlock(baseBlock, baseBlock.shapeIndex, false);

        DestroyImmediate(oldRootBlock);
        return newRootBlock;
    }

    public void SetEditorMode()
    {
        mainState = MainStateType.running;
    }
}

public enum MainStateType
{
    idle,
    running,
    checkLine,
    deleting,
}