using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Linq;
using VInspector.Libs;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class AttackController : MonoBehaviour //RAttack
{
    AudioManager AudM;
    AttackManager AttM;
    AttackUI attackUI;
    GameObject centerObject;
    GameObject videoObj;
    Enemy targetEnemy; //攻撃対象の敵

    public int power = 0;
    public List<EBlockColor> attackColorList { private set; get; } = new List<EBlockColor>(); //攻撃ブロックの色のリスト
    List<Texture> attackTextureList = new List<Texture>();

    bool attacked = false; //攻撃指示が出ているか
    bool doAttacking = false; //攻撃中か

    public bool doUpdating = false; //攻撃ブロックを設定中か
    bool doChecking = false; //攻撃ブロックの設定中か

    int attackQueueNum = 0; //攻撃待機リストの数
    int playAddAttackQueueNum = 0; //AddAttackQueueの実行回数
    List<Block> attackBlocks = new List<Block>(); //攻撃中の攻撃ブロックのリスト
    List<Block> waitingBlocks = new List<Block>(); //待機中の攻撃ブロックのリスト
    int edge = 0; //現在の正方形の辺の長さ
    float attackTime = 0f; // 攻撃までの残り時間
    float rotateSpeed;

    public void Init(AttackManager AttM) //初期化
    {
        this.AttM = AttM;
        AudM = FindFirstObjectByType<StageManager>().AudM;

        attackUI = Addressables.InstantiateAsync("AttackCanvas").WaitForCompletion().GetComponent<AttackUI>();
        attackUI.GetComponent<Canvas>().worldCamera = Camera.main;
        attackUI.transform.SetParent(this.transform);
        attackUI.transform.position = this.transform.position + new Vector3(0, 0, -1f);

        centerObject = new GameObject("WaitingList");
        centerObject.transform.SetParent(this.transform);
    }

    public void Update()
    {
        attackTime -= Time.deltaTime;
        attackUI?.SetPower(attackTime.RoundToInt());
        if (!attacked && attackTime <= 0)
        {
            attacked = true;
            Attack().Forget();
        }
    }

    void CheckEnoughBlocks() //必要数を攻撃ブロックに、余りを待機ブロックに設定
    {
        if (doChecking) return;
        doChecking = true;

        while (CanSquire(waitingBlocks.Count))
        {
            int needNum = (edge + 1) * (edge + 1) - edge * edge;
            List<Block> blockList = waitingBlocks.GetRange(0, needNum);
            waitingBlocks.RemoveRange(0, needNum);
            SetSquire(blockList);
        }
        doChecking = false;
    }

    bool CanSquire(int blockNum)
    {
        int needBlockNum = (edge + 1) * (edge + 1) - edge * edge;
        return blockNum >= needBlockNum;
    }

    void SetSquire(List<Block> blockList)
    {
        AddPower(blockList.Count, edge);
        int x;
        for(int y = 0; y <= edge; y++)
        {
            if(y < edge) x = edge;
            else x = 0;
            for (; x <= edge; x++)
            {
                Block block = blockList.First();
                blockList.Remove(block);

                Vector3Int index = new Vector3Int(x, y, 0); //正方形に配置する座標

                block.transform.position = transform.position + index;
                block.transform.rotation = Quaternion.Euler(-90, 0, 0);

                attackTime += 0.25f;
            }
        }
        // AudM.PlayNormalSound(NormalSound.BlockStacking);
        edge++;
    }

    void AddPower(int blockNum, int lineNum)
    {
        this.power += Mathf.FloorToInt(blockNum * lineNum * (0.5f + 0.5f * lineNum));
    }

    public void SetTarget(Enemy enemy) //攻撃対象を設定
    {
        targetEnemy = enemy;
    }

    async UniTask Attack()
    {
        GameObject video = Resources.Load<GameObject>("video");
        videoObj = Instantiate(video, transform.position + new Vector3((edge * 0.8f) / 2.0f, (edge * 0.8f) / 2.0f, -0.2f), Quaternion.Euler(90, 180, 0));
        videoObj.transform.localScale = new Vector3(edge / 10.0f, edge / 10.0f, edge / 10.0f);
        videoObj.transform.parent = transform;

        await transform.DOMove(targetEnemy.transform.position + new Vector3(-5, 0, 0), 0.6f).SetEase(Ease.InBack, 3); //-5は攻撃ブロックの原点が一番左にあるからあくまで仮の値
        targetEnemy.Damage(this);
        Release();
    }

    void Release()
    {
        Destroy(attackUI.gameObject);
        Destroy(centerObject);
        attackUI = null;
        if (videoObj) Destroy(videoObj);
        foreach (Block block in attackBlocks) block.Destroy();
        DestroyImmediate(this.gameObject);
    }

    public void AddWaitingBlock(Block block) //待機ブロックを設定
    {
        block.name = "AttackBlock";
        block.transform.parent = this.transform;
        block.transform.position = this.transform.position + new Vector3Int(0, 7, 0);
        waitingBlocks.Add(block);
        attackBlocks.Add(block);
        CheckEnoughBlocks();
    }
}
