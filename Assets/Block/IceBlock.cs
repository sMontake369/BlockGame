using UnityEngine;

public class IceBlock : BaseBlock
{
    public Texture iceMaterial;
    Texture previousMaterial;

    public override void Awake()
    {
        mainRenderer = gameObject.GetComponent<Renderer>();
        if(previousMaterial == null) previousMaterial = mainRenderer.material.mainTexture;
        mainRenderer.material.mainTexture = iceMaterial;
    }

    public override BaseBlock OnDelete(bool checkNeighbor = true) //ブロックが削除された後の処理
    {
        mainRenderer.material.mainTexture = previousMaterial;
        RootBlock.GamM.BlockConvert<BaseBlock>(this);
        return null;
    }
}
