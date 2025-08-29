using UnityEngine.Pool;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
    public static BlockPool Instance { get; private set; }
    GameObject poolObject;

    public ObjectPool<Block> blockPool; // ブロックのプール
    [SerializeField] private int blockCapacity = 500;
    [SerializeField] private int blockMax = 10000;
    
    public void Awake()
    {
        Instance = this;
        poolObject = new GameObject("PoolObject");

        blockPool = new ObjectPool<Block>(CreateBlock, TakeBlock, ReleaseBlock, DestroyBlock, 
        true, blockCapacity, blockMax);

        for(int i = 0; i < blockCapacity; i++) blockPool.Release(CreateBlock());
    }


    private Block CreateBlock()
    {
        return Instantiate(Resources.Load<GameObject>("Block")).AddComponent<Block>();
    }

    private void TakeBlock(Block block)
    {
        block.transform.SetParent(null);
    }

    private void ReleaseBlock(Block block)
    {
        block.transform.SetParent(null);
    }

    private void DestroyBlock(Block block)
    {
        block.transform.SetParent(null);
        Object.Destroy(block.gameObject);
    }
}
