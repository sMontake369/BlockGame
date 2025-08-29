using UnityEngine;

[CreateAssetMenu(fileName = "IceBlock_", menuName = "Block/IceBlock")]
public class IceBlockData : BaseBlockData
{
    public Texture iceMaterial; // 氷のテクスチャ

    public override BaseEffectBlock CreateEffectBlock()
    {
        return new IceBlock(this);
    }
}