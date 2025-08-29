using UnityEngine;

public class Block : MonoBehaviour
{
    public EBlockColor blockColor { get; private set; }
    public EBlockType blockType { get; private set; } = EBlockType.Block;
    public BaseEffectBlock effectBlock { get; private set; }

    // Debugようにpublicにしている
    public Vector2Int frameIndex; //{ get; private set; }

    FrameManager fraM;

    Renderer mainRenderer; // メインのレンダラー

    /// <summary>
    /// ブロックが生成された際に呼び出される。ObjectPoolで生成されたタイミングではない。
    /// </summary>
    public void OnGenerate(BaseEffectBlock effectBlock)
    {
        mainRenderer = GetComponent<Renderer>();
        blockColor = EBlockColor.None;
        frameIndex = Vector2Int.zero;
        fraM = StageManager.Instance.GetCurBattle().fraM;
        this.effectBlock = effectBlock;
    }

    public FrameResult WorldMove(Vector2Int framePos)
    {
        this.transform.position = fraM.transform.position + new Vector3(framePos.x, framePos.y, 0);
        this.frameIndex = framePos;
        return fraM.SetBlock(this, this.frameIndex);
    }

    public FrameResult WorldOffset(Vector2Int framePos)
    {
        this.transform.position += new Vector3(framePos.x, framePos.y, 0);
        this.frameIndex += framePos;
        return fraM.SetBlock(this, this.frameIndex);
    }

    public FrameResult LocalMove(Vector2Int framePos)
    {
        this.transform.localPosition = fraM.transform.position + new Vector3(framePos.x, framePos.y, 0);
        this.frameIndex = framePos;
        return fraM.SetBlock(this, this.frameIndex);
    }

    public FrameResult LocalOffset(Vector2Int framePos)
    {
        this.transform.localPosition += new Vector3(framePos.x, framePos.y, 0);
        this.frameIndex += framePos;
        return fraM.SetBlock(this, this.frameIndex);
    }

    public void SetTexture(Texture texture)
    {
        mainRenderer.material.mainTexture = texture;
    }

    public void SetName(string name)
    {
        this.gameObject.name = name;
    }

    public void SetColor(EBlockColor color)
    {
        blockColor = color;
        // mainRenderer.material.color = ColorMapping.GetColor(color);
    }

    public void OnGround()
    {
        transform.SetParent(fraM.transform);
    }

    public void CheckLine()
    {
        fraM.TryLine(frameIndex.y);
    }

    public void MoveRelease()
    {
        fraM.DeleteBlock(this);
    }

    // 消されることによって解放されるとき
    public void ReleaseFrame()
    {
        fraM.DeleteBlock(this);
        frameIndex = Vector2Int.zero;
    }

    // 強制的に削除されるとき
    public void DeleteFrame()
    {
        fraM.DeleteBlock(this);
        frameIndex = Vector2Int.zero;
    }
}

public enum EBlockColor
{
    SkyBlue,
    Red,
    Orange,
    Blue,
    Green,
    Yellow,
    Purple,
    None
}

public enum EBlockType
{
    Air,
    Block,
    Wall
}