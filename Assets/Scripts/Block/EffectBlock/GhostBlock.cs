// using System;
// using UnityEngine;

// public class GhostBlock : BaseEffectBlock
// {
//     public override void Set(BaseBlockData blockData)
//     {
//         block.name = "GhostBlock";
//     }

//     public override void Unset()
//     {

//     }

//     // フレームからブロックを削除
//     public override void Delete()
//     {
//         block.transform.parent = null;
//         block.frameIndex = Vector2Int.zero;
//     }

//     // 攻撃ブロックに変換する処理
//     public void ToAttack()
//     {
//         // 攻撃ブロックに変換する処理
//         // 例えば、攻撃力や範囲を設定するなど
//     }

//     public override void Release()
//     {
//         block.blockType = EBlockType.Air;
//         block.blockColor = EBlockColor.None;
//         block.frameIndex = Vector2Int.zero;
//     }

//     public override void Destroy()
//     {
//         Release();
//         GameObject.Destroy(block.gameObject);
//         block = null;
//     }
// }
