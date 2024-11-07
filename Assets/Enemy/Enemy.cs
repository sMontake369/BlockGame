using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class Enemy : MonoBehaviour
{
    public new string name { get; private set; }
    public int maxHp { get; private set; } //HPの最大値;
    public int hp { get; private set; } //現在のHP
    public List<EnemySkill> skillList { get; private set; }  //スキルリスト
    List<EnemyEvent> eventList; //イベントリスト
    public EnemySkill nextSkill { get; private set; } //次に発動するスキル
    Shield shield;
    List<ColorType> weakColorList = new List<ColorType>();

    bool canAttack = false;
    bool attackNow = false;
    bool isAlive = true;

    EnemyUI enemyUI;
    EnemyText enemyText;
    EnemyManager eneM;

    public void Init(EnemyManager eneM)
    {
        this.eneM = eneM;
    }

    public void Generate(EnemyData enemyData) //初期化
    {
        maxHp = enemyData.hp;
        hp = maxHp;
        name = enemyData.name;
        this.name = enemyData.name;
        weakColorList = enemyData.weakColorList;

        if(enemyData.skillList != null) 
        {
            skillList = new List<EnemySkill>(enemyData.skillList);
            foreach(EnemySkill enemy in skillList) enemy.Init(this);
        }
        if(enemyData.eventList != null)
        {
            eventList = new List<EnemyEvent>(enemyData.eventList);
            foreach(EnemyEvent enemy in eventList) enemy.Init(this);
        }

        GameObject enemyCanvas = Addressables.InstantiateAsync("EnemyCanvas").WaitForCompletion();
        enemyCanvas.transform.SetParent(this.transform);
        enemyCanvas.transform.position = this.transform.position + new Vector3(0,0,0);
        enemyCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

        enemyUI = enemyCanvas.GetComponent<EnemyUI>();
        enemyUI.Init(name, hp);

        enemyText = Addressables.InstantiateAsync("EnemyText").WaitForCompletion().GetComponent<EnemyText>();
        enemyText.name = name + "text";
        enemyText.transform.SetParent(this.transform);
        enemyText.transform.localPosition = new Vector3(-4,-1,-1);
        enemyText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 敵の行動を開始
    /// </summary>
    public async UniTask Play()
    {
        canAttack = true;
        EnemyEvent enemyEvent = CheckEvent();
        while(enemyEvent != null)
        {
            await Attack(enemyEvent.boardEffectList);
            eventList.Remove(enemyEvent);
            enemyEvent = CheckEvent();
        }
    }

    public void SetShield(Shield shield)
    {
        if(this.shield != null)
        {
            shield = null;
            enemyUI.DisableShield();
        }
        this.shield = shield;
        enemyUI.SetShield(this.shield.shieldImage, this.shield.shieldColor, this.shield.GetShieldText());
    }

    public async void Damage(AttackRBlock attackRBlock) //ダメージ処理
    {
        DamageUI damageUI = Addressables.InstantiateAsync("DamageCanvas").WaitForCompletion().GetComponent<DamageUI>();
        int damage = attackRBlock.power;
        if (hp == 0) OnKill();
        int weaknessMultiplier = 1;
        if(shield != null) 
        {
            if(shield.CanDestroy(damage)) 
            {
                shield = null; //シールドが破壊される場合はシールドを削除
                enemyUI.DisableShield();
            }
            else 
            {
                damage = 0;
            }
        }

        foreach(ColorType colorType in weakColorList)
        {
            if(attackRBlock.colorTypeList.Contains(colorType))
            {
                weaknessMultiplier *= 2;
                break;
            }
        }
        damage *= weaknessMultiplier;

        damageUI.Generate(this, damage, weaknessMultiplier != 1);

        await enemyUI.SetHP(hp - damage);
        hp = Mathf.Max(0, hp - damage);
        if (hp == 0) OnKill();
    }

    public async void Update()
    {
        if(!canAttack || attackNow) return;

        //即時発動スキルの処理
        if(eventList != null)
        {
            EnemyEvent enemyEvent = CheckEvent();
            while(enemyEvent != null)
            {
                await Attack(enemyEvent.boardEffectList);
                eventList.Remove(enemyEvent);
                enemyEvent = CheckEvent();
            }
        }

        if(nextSkill == null) 
        {
            nextSkill = GetRandomSkill(); //途中で条件を満たすスキルできるかもしれない
            if(nextSkill != null) nextSkill.AttackReq.isSelected();
            else return;
        }

        enemyUI.SetInterval(nextSkill.AttackReq.GetAttackUIText());
        if(nextSkill.AttackReq.isAttack()) 
        {
            enemyUI.SetInterval(nextSkill.AttackReq.GetAttackUIText());
            await Attack(nextSkill.boardEffectList);  
            nextSkill.AttackReq.isEnd();
            if(nextSkill.isOnce) skillList.Remove(nextSkill);
            nextSkill = GetRandomSkill();
            if(nextSkill != null)
            {
                nextSkill.AttackReq.isSelected();
                enemyUI.SetInterval(nextSkill.AttackReq.GetAttackUIText());
            }
        }
    }


    public async UniTask Attack(List<BaseEffectData> boardEffectList) //攻撃処理
    {
        if(attackNow) return;
        attackNow = true;

        foreach(BaseEffectData boardEffect in boardEffectList) 
        {
            if(boardEffect.isWait) await boardEffect.Execute();
            else _ = boardEffect.Execute();
        }

        attackNow = false;
    }

    void OnKill() //死亡時の処理
    {
        if(!isAlive) return;
        isAlive = false;

        //死亡時アニメーション
        this.gameObject.SetActive(false);
        eneM.AlertKill(this);
    }

    EnemySkill GetRandomSkill()
    {
        int MaxProbability = 0; //スキルの発動確率の合計
        List<EnemySkill> enableSkillList = new List<EnemySkill>();
        foreach(EnemySkill skill in skillList) 
        {
            if(skill.OccurReqList == null || !skill.IsOccur()) continue;
            enableSkillList.Add(skill);
        }

        foreach(EnemySkill skill in enableSkillList) MaxProbability += skill.probability;

        int random = UnityEngine.Random.Range(0, MaxProbability);
        int probability = 0;
        foreach(EnemySkill skill in enableSkillList)
        {
            probability += skill.probability;
            if(random < probability) return skill;
        }
        return null;
    }

    EnemyEvent CheckEvent()
    {
        foreach(EnemyEvent enemyEvent in eventList)
        {
            if(!enemyEvent.isOccur()) continue;
            return enemyEvent; //条件を満たしている場合
        }
        return null;
    }

    public void SetNextSkill(EnemySkill enemySkill, bool isNow) //次に発動するスキルをセット
    {
        enemySkill.Init(this);
        skillList.Add(enemySkill);
        if(isNow)
        {
            if(nextSkill != null) nextSkill.AttackReq.isEnd();
            nextSkill = enemySkill;
            nextSkill.AttackReq.isSelected();
            enemyUI.SetInterval(nextSkill.AttackReq.GetAttackUIText());
        }
    }

    public void DeleteSkill(EnemySkill enemySkill) //スキルを削除
    {
        skillList.Remove(enemySkill);
    }

    public void DeleteNextSkill() //スキルを削除
    {
        nextSkill.AttackReq.isEnd();
        nextSkill = null;
    }

    public async UniTask Talk(string str, int interval = 2000)
    {
        await enemyText.SetText(str, interval);
    }
}