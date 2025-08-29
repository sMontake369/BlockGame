using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class MainGameManager : BaseManager
{
    BattleManager batM;
    FrameManager fraM;
    AttackManager attM;
    AudioManager audM;

    private List<int> bagList; //袋リスト

    public ContainerBlock playerCBlock { get; private set; } //プレイヤーのブロック
    ContainerBlock holdBlock; //ホールドブロック

    List<ContainerBlock> nextCBlocks; //ネクストブロックリスト 


    // TODO OnGround時に実行されるUniRxを使う。　eventマネージャークラスに様々な条件を定義したい

    public override void Init()
    {
        BattleManager batM = this.transform.parent.GetComponent<BattleManager>();
        fraM = batM.fraM;
        attM = batM.attM;
        audM = batM.audM;

        nextCBlocks = new List<ContainerBlock>();
        bagList = new List<int>();
    }

    public void StartBattle()
    {
        // playerCBlock = GetNextBlock();
        playerCBlock = StageManager.Instance.test.Generate();
        playerCBlock.Move(new Vector2Int(5, 17)); // 初期位置に配置
    }

    public void OnGround()
    {
        ContainerBlock tmp = playerCBlock;
        playerCBlock = null; // これ大事
        tmp.OnGround();

        playerCBlock = StageManager.Instance.test.Generate();
        playerCBlock.Move(new Vector2Int(5, 17)); // 初期位置に配置
    }

    ContainerBlock GetNextBlock(int index = 0) //ネクストブロックを取得
    {
        if (batM.battleData.nextBlockPosList.Count < index) return null; //index個目が配置できるnextBlockを超えた場合、nullを返す
        ContainerBlock nextRBlock;
        if (nextCBlocks.Count <= index) //ブロックの数が足りない場合、新たに生成
        {
            // BaseCBlockData cBlockData = batM.battleData.blockShapeData.blockDataList[GetRandomInt()];
            ContainerBlockData cBlockData = StageManager.Instance.test;
            nextCBlocks.Add(cBlockData.Generate());
            nextCBlocks.Last().transform.localPosition = batM.battleData.nextBlockPosList.Last() + new Vector3(0, 0, -3);
        }
        if (nextCBlocks[index] != null) nextRBlock = nextCBlocks[index]; //ほしい奴
        else nextRBlock = GetNextBlock(index + 1); //ほしい奴がない場合、次の奴を取得

        ContainerBlock nextNextRBlock = GetNextBlock(index + 1); //次の奴
        if (nextNextRBlock != null)
        {
            if (index < batM.battleData.nextBlockPosList.Count) nextNextRBlock.gameObject.SetActive(true);
            nextNextRBlock.transform.DOKill();
            nextNextRBlock.transform.DOLocalJump(batM.battleData.nextBlockPosList[index], 0.5f, 1, 0.3f);
            nextCBlocks[index] = nextNextRBlock;
        }
        nextCBlocks.Remove(nextRBlock);
        nextRBlock.transform.DOKill();
        return nextRBlock;
    }

    public void SetNextBlock(ContainerBlock cBlock, int index)
    {
        if (index > batM.battleData.nextBlockPosList.Count)
        {
            for (int i = batM.battleData.nextBlockPosList.Count; i < index; i++)
            {
                // BaseCBlockData cBlockData = batM.battleData.blockShapeData.blockDataList[GetRandomInt()];
                ContainerBlockData cBlockData = StageManager.Instance.test;
                ContainerBlock nextCBlock = cBlockData.Generate();
                nextCBlocks.Add(nextCBlock);
                nextCBlocks[i].transform.localPosition = batM.battleData.nextBlockPosList.Last() + new Vector3(0, 0, -3);
                nextCBlocks[i].gameObject.SetActive(false);
            }
        }

        nextCBlocks.Insert(index, cBlock);
        for (int i = index; i < nextCBlocks.Count; i++)
        {
            if (i < batM.battleData.nextBlockPosList.Count)
            {
                nextCBlocks[i].transform.DOKill();
                nextCBlocks[i].transform.DOLocalJump(batM.battleData.nextBlockPosList[i], 0.5f, 1, 0.3f);
            }
            else
            {
                nextCBlocks[i].transform.localPosition = batM.battleData.nextBlockPosList.Last() + new Vector3(0, 0, -3);
                nextCBlocks[i].gameObject.SetActive(false);
            }
        }
    }

    int GetRandomInt() //袋からランダムに数を返す
    {
        if (bagList.Count == 0) bagList.AddRange(Enumerable.Range(0, batM.battleData.blockShapeData.blockDataList.Count));

        int num = bagList[Random.Range(0, bagList.Count)];
        bagList.Remove(num);
        return num;
    }

    public void Move(Vector2Int pos)
    {
        playerCBlock.Move(pos);
    }

    public void Shift(Vector2Int offset)
    {
        bool result = playerCBlock.Shift(offset);
        if (!result && offset == Vector2Int.down) OnGround();
    }

    public void Rotate(float offset)
    {
        playerCBlock.Rotate(offset);
    }

    public void Drop()
    {
        int count = 0;
        while (playerCBlock.Shift(Vector2Int.down))
        {
            count++;
            if (count >= 20) // 無限ループ防止
            {
                Debug.Log("落ちすぎ");
                break;
            }
        }
        OnGround();
    }

    public void Hold()
    {

    }
    
    public void OnDestroy()
    {
        playerCBlock = null;
    }
}