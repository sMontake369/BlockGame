using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AddFrame", menuName = "Effect/AddFrameData")]
public class AddFrame : BaseEffectData
{
    public FrameData frameData;
    public Vector3Int pos;
    FrameManager FraM;

    public override void Init(Enemy enemy)
    {
        StageManager StaM = FindFirstObjectByType<StageManager>();
        BattleManager BatM = StaM.GetCurBattle();
        FraM = BatM.FraM;
        this.enemy = enemy;
    }

    public override UniTask Execute()
    {
        if(frameData == null) 
        {
            Debug.LogError("フレームデータが設定されていません");
            return UniTask.CompletedTask;
        }
        FraM.AddFrame(pos, frameData, true);
        return UniTask.CompletedTask;
    }
}
