using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using DG.Tweening;
using System.Linq;

public class MainGameManager : MonoBehaviour
{
    BattleManager BatM;
    FrameManager FraM;
    AttackManager AttM;

    private List<int> bagList = new List<int>();

    public RootBlock playerBlock { get; private set; } //プレイヤーブロック
    RootBlock holdBlock; //ホールドブロック

    List<RootBlock> nextRBlockList = new List<RootBlock>(); //ネクストブロックリスト
    int generationNum = 0; //ブロックの世代数

    public MainStateType mainState { get; private set; }

    public void Init()
    {
        BatM = FindFirstObjectByType<BattleManager>();
        FraM = FindFirstObjectByType<FrameManager>();
        AttM = FindFirstObjectByType<AttackManager>();

        if(!BatM || !FraM || !AttM) 
        {
            Debug.Log("BattleManager or FrameManager or AttackManager is not found");
            return;
        }
        
        bagList.Clear();
        generationNum = 0;
        mainState = MainStateType.idle;
    }

    public async void SetNextPlayerBlock() //次のプレイヤーブロックを生成
    {
        if(playerBlock) playerBlock.DestroyGhostBlock();
        playerBlock = null;
        await CheckLine(); //これはここでいいのか？
        mainState = MainStateType.running;

        playerBlock = GetNextBlock();
        playerBlock.transform.position = this.transform.position + BatM.battleData.blockSpawnPos;

        foreach(List<BaseBlock> baseBlockList in playerBlock.BlockListList)
        foreach(BaseBlock baseBlock in baseBlockList)
        if(baseBlock != null) baseBlock.frameIndex = BatM.battleData.blockSpawnPos + baseBlock.shapeIndex;

        FraM.SetRBlock(playerBlock);

        if(FraM.IsConflict(playerBlock, Vector3Int.zero)) 
        {
            Debug.Log("ゲームオーバー");
            return;
        }
    }

    public RootBlock GetNextBlock(int index = 0) //ネクストブロックを取得
    {
        if(BatM.battleData.nextBlockPosList.Count < index) return null; //index個目のネクストブロックがない場合、nullを返す
        RootBlock nextRBlock;
        if(nextRBlockList.Count <= index) //数が足りない場合、新たに生成
        {
            nextRBlockList.Add(GenerateRBlock(BatM.blockShapeData.blockDataList[GetRandomInt()]));
            nextRBlockList.Last().transform.position = this.transform.position + BatM.battleData.nextBlockPosList.Last() + new Vector3(0, 0, -10);
        }
        if(nextRBlockList[index] != null) nextRBlock = nextRBlockList[index]; //ほしい奴
        else nextRBlock = GetNextBlock(index + 1); //ほしい奴がない場合、次の奴を取得

        RootBlock nextNextRBlock = GetNextBlock(index + 1); //次の奴
        if(nextNextRBlock != null)
        {
            nextNextRBlock.transform.DOKill();
            nextNextRBlock.transform.DOJump(this.transform.position + BatM.battleData.nextBlockPosList[index],0.5f,1,0.3f);
            nextRBlockList[index] = nextNextRBlock;
        }
        nextRBlockList.Remove(nextRBlock);
        nextRBlock.transform.DOKill();
        return nextRBlock;
    }

    public RootBlock GenerateRBlock(RootBlockData blockData = null) //ルートブロックを生成
    {
        RootBlock rootBlock = BlockPool.Instance.rootPool.Get();
        rootBlock.name = "RootBlock";
        rootBlock.transform.parent = this.transform;
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

    public BaseBlock GenerateBlock(BlockType blockType, ColorType colorType, Texture texture)
    {
        BaseBlock block = BlockPool.Instance.blockPool.Get();
        block.name = "BaseBlock";
        block.blockType = blockType;
        block.colorType = colorType;
        block.mainRenderer.material.mainTexture = texture;
        return block;
    }

    int GetRandomInt() //袋からランダムに数を返す
    {
        int count = 0;
        int random;
        while(true) 
        {
            random = Random.Range(0, BatM.blockShapeData.blockDataList.Count);
            if(!bagList.Contains(random))
            {
                bagList.Add(random);
                break;
            }
            count++;
            if(count > 100) 
            {
                Debug.Log("無限ループ");
                break;
            }
        }
        if(bagList.Count >= BatM.blockShapeData.blockDataList.Count) bagList.Clear();
        return random;
    }

    public void SetHoldBlock()
    {
        if(holdBlock == null)
        {
            FraM.DeleteRBlock(playerBlock);
            holdBlock = playerBlock;
            holdBlock.transform.position = this.transform.position + BatM.battleData.holdBlockPos;
            holdBlock.pivot.transform.rotation = Quaternion.identity;
            SetNextPlayerBlock();
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
            holdBlock.transform.position = this.transform.position + BatM.battleData.holdBlockPos;
            holdBlock.pivot.transform.rotation = Quaternion.identity;

            FraM.SetRBlock(playerBlock);
        }
    }

    public async UniTask CheckLine() //ラインが揃っているかチェック
    {
        mainState = MainStateType.checkLine;
        List<int> lineList = new List<int>();
        for(int y = FraM.LFrameBorder.lowerLeft.y; y < FraM.LFrameBorder.upperRight.y; y++)
        {
            bool isLine = true;
            bool canDelete = false;

            for(int x = FraM.LFrameBorder.lowerLeft.x; x < FraM.LFrameBorder.upperRight.x; x++)
            {
                BaseBlock baseBlock = FraM.GetBlock(new Vector3Int(x, y, 0));
                if(baseBlock == null)
                {
                    isLine = false;
                    break;
                }
                if(baseBlock.blockType == BlockType.Mino) canDelete = true;
            }
            if(canDelete && isLine) lineList.Add(y);
        }
        if(lineList.Count > 0) await DeleteLine(lineList);
        else if(mainState != MainStateType.idle) mainState = MainStateType.running;
    }

    async UniTask DeleteLine(List<int> lineList) //ラインを消す
    {
        mainState = MainStateType.deleting;
        List<BaseBlock> deleteBlockList = new List<BaseBlock>(); //削除するブロックリスト
        List<BaseBlock> curSearchBlockList = new List<BaseBlock>(); //現在探索しているブロックリスト
        List<BaseBlock> nextSearchBlockList = new List<BaseBlock>(); //次に探索するブロックリスト
        ColorType colorType; //変更する色

        List<Vector3Int> neighborIndexList = new List<Vector3Int>()
        {
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left
        };

        //最大の世代数を持つルートブロックを取得
        List<BaseBlock> blockList = new List<BaseBlock>(); //ブロックリスト
        BaseBlock maxGenBlock = null;
        int maxGenerationNum = 0;
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
        nextSearchBlockList.AddRange(maxGenBlock.RootBlock.BlockList);

        foreach(BaseBlock baseBlock in nextSearchBlockList) //最大の世代数を持つブロックを削除する
        {
            BaseBlock deleteBlock = baseBlock.OnDelete(); //削除
            if(deleteBlock != null)
            {
                deleteBlock.SetColor(colorType, BatM.GetTexture(colorType)); //色を変更
                FraM.DeleteBlock(deleteBlock);
                deleteBlockList.Add(deleteBlock);
            }
        }

        int count = 0;
        while(nextSearchBlockList.Count > 0) //隣接するブロックがなくなるまで探索
        {
            curSearchBlockList.Clear();
            curSearchBlockList.AddRange(nextSearchBlockList); //次に探索するブロックリストを現在の探索ブロックリストに変更
            nextSearchBlockList.Clear();

            foreach(BaseBlock baseBlock in curSearchBlockList)
            {
                if(baseBlock == null) continue;
                Vector3Int index = baseBlock.frameIndex;
                foreach(Vector3Int neighborIndex in neighborIndexList)
                {
                    Vector3Int searchIndex = index + neighborIndex;
                    if(!lineList.Contains(searchIndex.y)) continue; //削除するラインに含まれているかどうか

                    BaseBlock searchBlock = FraM.GetBlock(searchIndex); //隣接するブロックを取得
                    if(searchBlock == null) continue;
                    if(searchBlock.blockType == BlockType.Mino && !deleteBlockList.Contains(searchBlock)) //削除するブロックリストに含まれているかどうか
                    {
                        BaseBlock deleteBlock = searchBlock.OnDelete(); //削除
                        if(deleteBlock != null)
                        {
                            FraM.DeleteBlock(deleteBlock);
                            deleteBlock.SetColor(colorType, BatM.GetTexture(colorType)); // 色を変更
                            deleteBlockList.Add(deleteBlock);
                            nextSearchBlockList.Add(deleteBlock);
                        }
                    }
                }
            await UniTask.Delay(3); //削除したブロックがある場合、少し待つ
            }
            count++;
            if(count > 100) 
            {
                Debug.Log("無限ループ");
                break;
            }
        }

        if(mainState != MainStateType.idle) AttM.AddAttackQueue(deleteBlockList); //攻撃ブロックを生成
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

        mainState = MainStateType.idle;
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