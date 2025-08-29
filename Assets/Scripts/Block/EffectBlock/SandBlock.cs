// using UnityEngine;
// using DG.Tweening;
// using Cysharp.Threading.Tasks;

// public class SandBlock : ContainerBlock
// {
//     bool isFalling = false;
//     // Update is called once per frame
//     async void Update()
//     {
//         if(!fraM.IsConflict(this, Vector3Int.down))
//         {
//             foreach(Block baseBlock in BlockList) if(!baseBlock.canMove) return; //移動できないブロックがある場合
//             if(fraM.IsConflict(this, Vector3Int.down)) 
//             {
//                 if(this == GamM.playerBlock) GamM.TurnEnd();
//                 return;
//             }
//             await Fall();
//         }
//     }

//     async UniTask Fall()
//     {
//         if(isFalling) return;
//         isFalling = true;
//         fraM.DeleteRBlock(this);
//         foreach(Block baseBlock in BlockList) baseBlock.frameIndex += Vector3Int.down;
//         fraM.SetRBlock(this);
//         await this.transform.DOMove(this.transform.position + Vector3.down, 0.1f).SetEase(Ease.Linear);
//         isFalling = false;
//     }

//     public override bool Transform(Vector3Int offset)
//     {
//         if(isFalling)
//         {
//             this.transform.DOComplete();
//         }
//         return base.Transform(offset);
//     }   
// }
