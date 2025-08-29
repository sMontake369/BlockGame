using UnityEngine;

[CreateAssetMenu(fileName = "Block_", menuName = "Block/NormalBlock")]
public class NormalBlockData : BaseBlockData
{
    public override BaseEffectBlock CreateEffectBlock()
    {
        return new NormalBlock(this);
    }
}
