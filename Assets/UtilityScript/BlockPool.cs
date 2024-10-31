using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class BlockPool : MonoBehaviour
{
    public static BlockPool Instance { get; private set; }
    GameObject poolObject;

    public ObjectPool<RootBlock> rootPool; // ルートブロックとpivotのプール
    public ObjectPool<BaseBlock> blockPool; // ブロックのプール

    GameObject cubeObject; // ブロックのプレハブ

    // 既にプールに入っている既存のアイテムを返そうとすると例外が発生します
    [SerializeField] private bool collectionCheck = true;
    // プール容量と最大サイズをコントロールする追加オプション
    [SerializeField] private int rootCapacity = 50;
    [SerializeField] private int rootMax = 500;

    [SerializeField] private int blockCapacity = 500;
    [SerializeField] private int blockMax = 10000;
    
    public void Awake()
    {
        Instance = this;
        poolObject = new GameObject("PoolObject");

        cubeObject = Resources.Load<GameObject>("Block");
        rootPool = new ObjectPool<RootBlock>(OnCreateRootBlock, OnTakeRootBlock, OnReleaseRootBlock, OnDestroyRootBlock,
        collectionCheck, rootCapacity, rootMax);

        blockPool = new ObjectPool<BaseBlock>(OnCreateBlock, OnTakeBlock, OnReleaseBlock, OnDestroyBlock, 
        collectionCheck, blockCapacity, blockMax);

        for(int i = 0; i < rootCapacity; i++) rootPool.Release(OnCreateRootBlock());
        for(int i = 0; i < blockCapacity; i++) blockPool.Release(OnCreateBlock());
    }

    private RootBlock OnCreateRootBlock()
    {
        RootBlock rootBlock = new GameObject("RootBlock").AddComponent<RootBlock>();
        GameObject pivot = new GameObject("Pivot");
        pivot.transform.SetParent(rootBlock.transform);
        rootBlock.pivot = pivot;
        return rootBlock;
    }

    private void OnTakeRootBlock(RootBlock rootBlock)
    {
        rootBlock.gameObject.SetActive(true);
    }

    public static void ReleaseNotRootBlock(RootBlock rootBlock) //RootBlock以外のクラスを受け取るときに使う
    {
        if(rootBlock.GetType() != typeof(RootBlock)) rootBlock = RootConvert<RootBlock>(rootBlock);
        Instance.rootPool.Release(rootBlock);
    }

    private void OnReleaseRootBlock(RootBlock rootBlock)
    {
        foreach(BaseBlock block in rootBlock.pivot.GetComponentsInChildren<BaseBlock>())
        {
            ReleaseNotBaseBlock(block);
            rootBlock.BlockListList.Clear();
        }
        rootBlock.transform.parent = poolObject.transform;
        rootBlock.gameObject.SetActive(false);
    }

    private void OnDestroyRootBlock(RootBlock rootBlock)
    {
        //Destroy(rootBlock.gameObject);
    }


    private BaseBlock OnCreateBlock()
    {
        BaseBlock block = Instantiate(cubeObject).AddComponent<BaseBlock>();
        block.name = "BaseBlock";
        return block;
    }

    private void OnTakeBlock(BaseBlock block)
    {
        block.gameObject.SetActive(true);
    }

    public static void ReleaseNotBaseBlock(BaseBlock baseBlock) //RootBlock以外のクラスを受け取るときに使う
    {
        if(baseBlock.GetType() != typeof(BaseBlock)) baseBlock = BlockConvert<BaseBlock>(baseBlock);
        Instance.blockPool.Release(baseBlock);
    }

    private void OnReleaseBlock(BaseBlock block)
    {
        block.frameIndex = Vector3Int.zero;
        block.shapeIndex = Vector3Int.zero;
        
        Color color = block.mainRenderer.material.color;
        color.a = 1;
        block.mainRenderer.material.color = color;
        
        block.transform.parent = poolObject.transform;
        block.mainRenderer.material.color = new Color(1, 1, 1, 1f);
        block.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        block.gameObject.SetActive(false);
    }

    private void OnDestroyBlock(BaseBlock block)
    {
        //Destroy(block.gameObject);
    }

    public static T BlockConvert<T>(BaseBlock oldBlock) where T : BaseBlock
    {
        BaseBlock newBlock = oldBlock.AddComponent<T>();
        newBlock.blockType = oldBlock.blockType;
        newBlock.frameIndex = oldBlock.frameIndex;
        oldBlock.RootBlock.AddBlock(newBlock, oldBlock.shapeIndex, false);

        DestroyImmediate(oldBlock);

        return newBlock as T;
    }

    public static T RootConvert<T>(RootBlock oldRootBlock) where T : RootBlock
    {
        RootBlock newRootBlock = oldRootBlock.gameObject.AddComponent<T>();
        newRootBlock.pivot = oldRootBlock.pivot;

        foreach(BaseBlock baseBlock in oldRootBlock.BlockList)
        {
            newRootBlock.AddBlock(baseBlock, baseBlock.shapeIndex);
        }

        DestroyImmediate(oldRootBlock);
        return newRootBlock as T;
    }
}
