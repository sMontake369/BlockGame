using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SetNextBlock", menuName = "Effect/SetNextBlock")]
public class SetNextBlock : BaseEffectData
{
    [SerializeField] [Header("追加するブロックのデータ")]
    public List<RootBlockData> rootBlockDataList;
    [SerializeField] [Header("追加するブロックの位置")] [Range(0, 20)]
    public int index;

    MainGameManager GamM;
    public override void Init(Enemy enemy)
    {
        this.enemy = enemy;
        StageManager StaM = FindFirstObjectByType<StageManager>();
        GamM = StaM.GetCurBattle().GamM;
    }

    public override UniTask Execute()
    {
        foreach (RootBlockData rootBlockData in rootBlockDataList)
        {
            RootBlock rootBlock = GamM.GenerateRBlock(rootBlockData);
            rootBlock.transform.position = enemy.transform.position;
            GamM.SetNextBlock(rootBlock, index);
        }
        return UniTask.CompletedTask;
    }
}
