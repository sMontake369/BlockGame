using UnityEngine;

public class NormalBlock : BaseEffectBlock
{
    public NormalBlock(NormalBlockData blockData)
    {
        this.block = BlockPool.Instance.blockPool.Get();
        this.block.SetName("NormalBlock");
        this.block.SetColor(blockData.eBlockColor);
        block.OnGenerate(this);
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
        StageManager.Instance.GetCurBattle().attM.AddAttackQueue(block);
    }

    public override void Destroy()
    {
        Release();

        // TODO ObjectPoolに戻す
        GameObject.Destroy(block.gameObject);
    }
}