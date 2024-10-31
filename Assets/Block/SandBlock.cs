using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class SandBlock : RootBlock
{
    bool isFalling = false;
    // Update is called once per frame
    async void Update()
    {
        if(FraM.IsConflict(this, Vector3Int.down))
        {
            foreach(BaseBlock baseBlock in BlockList) if(!baseBlock.canMove) return; //移動できないブロックがある場合
            if(FraM.IsConflict(this, Vector3Int.down)) 
            {
                if(this == GamM.playerBlock) GamM.TurnEnd();
                return;
            }
            await Fall();
        }
    }

    async UniTask Fall()
    {
        if(isFalling) return;
        isFalling = true;
        FraM.DeleteRBlock(this);
        foreach(BaseBlock baseBlock in BlockList) baseBlock.frameIndex += Vector3Int.down;
        FraM.SetRBlock(this);
        await this.transform.DOMove(this.transform.position + Vector3.down, 0.05f).SetEase(Ease.Linear);
        isFalling = false;
    }

    public override bool Transform(Vector3Int offset)
    {
        if(isFalling)
        {
            this.transform.DOComplete();
        }
        return base.Transform(offset);
    }   
}
