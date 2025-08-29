using UnityEngine;

public class IceBlock : BaseEffectBlock
{
    public IceBlock(IceBlockData blockData)
    {
        this.block = BlockPool.Instance.blockPool.Get();
        this.block.SetName("IceBlock");
        this.block.SetColor(blockData.eBlockColor);
        this.block.SetTexture(blockData.iceMaterial);
    }

    protected override void OnSetAfter()
    {

    }

    protected override void OnUnsetBefore()
    {
        block.SetName(string.Empty);
    }

    public override FrameResult Move(Vector2Int framePos)
    {
        return block.WorldMove(framePos);
    }

    public override FrameResult Shift(Vector2Int offset)
    {
        return block.WorldOffset(offset);
    }

    public override FrameResult Rotate(Vector2Int shapePos)
    {
        return block.LocalMove(shapePos);
    }

    // 攻撃ブロックに変換する処理
    public void ToAttack()
    {
        // 攻撃ブロックに変換する処理
        // 例えば、攻撃力や範囲を設定するなど
    }

    public override void Release()
    {
        block.ReleaseFrame();
    }

    public override void Destroy()
    {
        Release();

        // TODO ObjectPoolに戻す
        GameObject.Destroy(block.gameObject);
    }
}