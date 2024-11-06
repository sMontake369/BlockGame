using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "DeleteBlock", menuName = "Effect/DeleteBlock")]
public class DeleteBlock : BaseEffectData
{
    [Header("削除タイプ")]
    public DeleteType deleteType;
    [Header("特定の位置に削除する場合の座標")]
    public List<Vector2Int> deletePosList;
    [Header("ランダムに削除する場合の数")]
    public int deleteCount;

    public AudioClip shakeBlockSE = default;
    public AudioClip deleteBlockSE = default;

    FrameManager FraM;
    AudioManager AudM;

    public override async UniTask Execute()
    {
        List<BaseBlock> deleteBlockList = new List<BaseBlock>();
        switch (deleteType)
        {
            case DeleteType.All:
                deleteBlockList = FraM.DeleteAllBlocks();
                break;
            case DeleteType.RandomPos:
                for(int i = 0; i < deleteCount; i++)
                {
                    Vector3Int pos = new Vector3Int(Random.Range(0, FraM.LFrameBorder.width), Random.Range(0, FraM.LFrameBorder.height - 8));
                    BaseBlock block = FraM.GetBlock(pos);
                    deleteBlockList.Add(FraM.DeleteBlock(block));
                }
                break;
            case DeleteType.SpecificPos:
                foreach(Vector2Int pos in deletePosList)
                {
                    BaseBlock block = FraM.GetBlock(new Vector3Int(pos.x, pos.y, 0));
                    deleteBlockList.Add(FraM.DeleteBlock(block));
                }
                break;
        }

        AudM.PlaySound(shakeBlockSE);
        foreach(BaseBlock block in deleteBlockList)
        {
            _ = DOTween.Sequence()
                .Append(block.transform.DOShakePosition(1f, 0.1f, 40, 10, false, false))
                .Append(block.transform.DOScale(Vector3.zero, 0.5f))
                .OnComplete(() =>
                {
                    BlockPool.ReleaseNotBaseBlock(block);
                });
        }
        await UniTask.Delay(1000);
        AudM.PlaySound(deleteBlockSE);
    }

    public override void Init(Enemy enemy)
    {
        this.enemy = enemy;
        StageManager StaM = FindFirstObjectByType<StageManager>();
        BattleManager BatM = StaM.GetCurBattle();
        FraM = BatM.FraM;
        AudM = StaM.AudM;
    }
}

public enum DeleteType
{
    RandomPos,
    SpecificPos,
    All
}
