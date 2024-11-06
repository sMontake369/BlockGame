using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class AttackRBlock : RootBlock //RAttack
{   
    public int power { private set; get; } = 0;
    public List<ColorType> colorTypeList { private set; get; } = new List<ColorType>(); //攻撃ブロックの色のリスト
    List<Texture> textureList = new List<Texture>();
    AudioManager AudM;

    public void Start()
    {
        AudM = FindFirstObjectByType<StageManager>().AudM;
    }

    public int edge { private set; get; } = 0; //現在の正方形の辺の長さ
    
    public bool CanSquire(int blockNum)
    {
        int needBlockNum = (edge + 1) * (edge + 1) - edge * edge;
        return blockNum >= needBlockNum;
    }

    public async UniTask SetSquire(List<BaseBlock> blockList)
    {
        int x;
        for(int y = 0; y <= edge; y++)
        {
            if(y < edge) x = edge;
            else x = 0;
            for(; x <= edge; x++)
            {
                BaseBlock block = blockList[0];
                blockList.RemoveAt(0);
                block.transform.DOKill();

                Vector3Int index = new Vector3Int(x, y, 0); //正方形に配置する座標
                AddBlock(block, index, false);

                //アニメーション
                _ = block.transform.DOJump(transform.position + index, 10, 1, 1f).SetEase(Ease.OutQuint);
                _ = block.transform.DORotate(new Vector3(-90,0,0), 0.5f).SetEase(Ease.InExpo).OnComplete(() =>
                {
                    AudM.PlayNormalSound(NormalSound.BlockStacking);
                });
                await UniTask.Delay(50);
            }
        }
        edge++;
    }

    public void AddPower(int blockNum, int lineNum) 
    {
        this.power += Mathf.FloorToInt(blockNum * lineNum * (0.5f + 0.5f * lineNum));
    }

    public async void Attack(Enemy enemy)
    {
        int edge = Mathf.CeilToInt(Mathf.Sqrt(GetBlockNum()));
        GameObject video = Resources.Load<GameObject>("video");
        GameObject obj = Instantiate(video, transform.position + new Vector3((edge * 0.8f) / 2.0f, (edge * 0.8f) / 2.0f, -0.2f), Quaternion.Euler(90,180,0));
        obj.transform.localScale = new Vector3(edge / 10.0f, edge / 10.0f, edge / 10.0f);
        obj.transform.parent = transform;
        for(int i = 0; i < 8; i++)
        {
            AudM.PlayNormalSound(NormalSound.Hold);
            await UniTask.Delay(200);
        }
        

        _ = transform.DOMove(enemy.transform.position + new Vector3(-5,0,0), 0.6f).SetEase(Ease.InBack,3); //-5は攻撃ブロックの原点が一番左にあるからあくまで仮の値
        await UniTask.Delay(270);
        AudM.PlayNormalSound(NormalSound.ThrowBlock);
        await UniTask.Delay(330);
        if(enemy) 
        {
            enemy.Damage(this);
            AudM.PlayNormalSound(NormalSound.Attack);
        }

        Destroy(this.gameObject);
    }

    void ToOneBlock()
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
