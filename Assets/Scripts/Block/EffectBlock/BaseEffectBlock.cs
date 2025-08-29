using UnityEngine;

public abstract class BaseEffectBlock
{
    protected Block block; // このブロックの元となるBlock
    public Vector2Int frameIndex { get { return block.frameIndex; } } // フレーム上の位置

    public void ChangeBlockEffect<T>(BaseBlockData blockData) where T : BaseEffectBlock, new()
    {
        OnUnsetBefore();
        T newBlock = new T();
        newBlock.block = block;
        newBlock.block.SetColor(blockData.eBlockColor);
        OnSetAfter();
    }

    public void SetParent(ContainerBlock container,  Vector2Int shapePos)
    {
        block.transform.SetParent(container.transform);
        block.transform.localPosition = new Vector3(shapePos.x, shapePos.y, 0);
    }

    public void OnGround()
    {
        block.OnGround();
    }

    public void CheckLine()
    {
        block.CheckLine();
    }

    public void MoveRelease()
    {
        block.MoveRelease();
    }

    /// <summary>
    /// エフェクトを付けた後に呼び出される。
    /// </summary>
    protected abstract void OnSetAfter();
    
    /// <summary>
    /// エフェクトを削除する前に呼び出される。
    /// </summary>
    protected abstract void OnUnsetBefore();

    /// <summary>
    /// ブロックをフレームから解放する
    /// </summary>
    public abstract void Release();

    /// <summary>
    /// ブロックをフレームから削除する
    /// </summary>
    public abstract void Destroy();

    /// <summary>
    /// ブロックを移動させる
    /// </summary>
    /// <param name="framePos">移動した位置</param>
    public abstract FrameResult Move(Vector2Int framePos);

    /// <summary>
    /// ブロックをオフセットさせる
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public abstract FrameResult Shift(Vector2Int offset);

    /// <summary>
    /// ブロックを回転させる
    /// </summary>
    /// <param name="framePos">移動した位置</param>
    public abstract FrameResult Rotate(Vector2Int framePos);
}
