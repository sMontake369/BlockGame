using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Unity.Collections;

public class AttackRBlock : RootBlock //RAttack
{   
    int power;
    public int Power { get => power; }
    List<ColorType> colorTypeList = new List<ColorType>();
    public List<ColorType> ColorType { get => colorTypeList;}
    List<Texture> textureList = new List<Texture>();

    public void AddPower(int power)
    {
        this.power += power;
    }

    public async void Attack(Enemy enemy)
    {
        int edge = Mathf.CeilToInt(Mathf.Sqrt(GetBlockNum()));
        GameObject video = Resources.Load<GameObject>("video");
        GameObject obj = Instantiate(video, transform.position + new Vector3(edge / 2 - 0.35f, edge / 2 - 0.35f, -10), Quaternion.Euler(90,180,0));
        obj.transform.localScale = new Vector3(edge / 10.0f, edge / 10.0f, edge / 10.0f);
        obj.transform.parent = transform;
        await UniTask.Delay(2000);

        await transform.DOMove(enemy.transform.position + new Vector3(-5,0,0), 0.6f).SetEase(Ease.InBack,3); //-5は攻撃ブロックの原点が一番左にあるからあくまで仮の値
        if(enemy) enemy.Damage(this);

        Destroy(this.gameObject);
    }

    public void ToOneBlock()
    {
        foreach(BaseBlock block in BlockList)
        {
            ColorType colorType = block.colorType;
            if(colorTypeList.Contains(colorType)) continue;
            colorTypeList.Add(colorType);
            textureList.Add(block.GetComponent<Renderer>().material.mainTexture);
        }

        int edge = Mathf.CeilToInt(Mathf.Sqrt(GetBlockNum()));
        BaseBlock baseBlock = BlockList[0].ReleaseBlock();
        float scale = baseBlock.transform.localScale.x * edge;

        foreach(BaseBlock block in BlockList) BlockPool.ReleaseNotBaseBlock(block);
        baseBlock.transform.localScale = new Vector3(scale, scale, scale);

        TextureChange(baseBlock);
        float pos = ((edge - 1) * 5) / 10;
        pivot.transform.localPosition = new Vector3(pos, pos);
        pivot.transform.rotation = Quaternion.Euler(new Vector3(0,0,180));
        baseBlock.transform.localPosition = Vector3.zero;
    }

    void TextureChange(BaseBlock baseBlock)
    {
        int textureSize = 512; // テクスチャのサイズ
        int count = textureList.Count;
        int subTextureSize = textureSize / count;  // 各テクスチャのサイズ

        // 結果のテクスチャを作成
        Texture2D result = new Texture2D(textureSize, textureSize, TextureFormat.RGBA64, false);

        // 各テクスチャをリサイズしてコピー
        for(int i = 0; i < count; i++)
        {
            Texture texture = textureList[i];
            int x = i * subTextureSize;

            // テクスチャの一部を新しいテクスチャにコピー
            //textureのimport設定にある形式をRGBA64bitにしないとエラーが出る
            Graphics.CopyTexture(texture, 0, 0, x, 0, subTextureSize, textureSize, result, 0, 0, x, 0);
        }

        result.Apply();
        baseBlock.mainRenderer.material.mainTexture = result;
    }
}
