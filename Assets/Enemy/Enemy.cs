using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class Enemy : MonoBehaviour
{
    new string name;
    public string Name { get { return name; } }
    int hp;
    public int HP { get { return hp; } }
    List<EnemySkill> enemySkillList;
    EnemySkill nextSkill;
    public List<InitSkill> InitSkillList;
    float attackTimer;
    public bool canAttack = false;
    public bool isAlive = true;
    EnemyUI enemyUI;
    EnemyManager EneM;
    List<Shield> shieldList = new List<Shield>();
    List<ColorType> weakColorList = new List<ColorType>();

    public void Awake()
    {
        EneM = FindFirstObjectByType<EnemyManager>();

        GameObject enemyCanvas = Addressables.InstantiateAsync("EnemyCanvas").WaitForCompletion();
        //ここはInit関数を作り、そこで生成するように変更する
        enemyCanvas.name = "EnemyCanvas";
        enemyCanvas.transform.SetParent(this.transform);
        enemyCanvas.transform.position = this.transform.position + new Vector3(0,0,0);
        enemyCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        //ここまで
        enemyUI = enemyCanvas.GetComponent<EnemyUI>();
    }

    public void Init(EnemyData enemyData) //初期化
    {
        hp = enemyData.hp;
        name = enemyData.name;
        enemySkillList = new List<EnemySkill>(enemyData.skillList);
        InitSkillList = enemyData.InitSkillList;
        weakColorList = enemyData.weakColorList;
        this.name = enemyData.name;

        canAttack = false;

        if(enemySkillList.Count > 0)
        {
            nextSkill = enemySkillList[UnityEngine.Random.Range(0, enemySkillList.Count)];
        }

        enemyUI.Generate(name, hp, attackTimer);
        this.gameObject.SetActive(false);
    }

    public void PlayEnemy()
    {
        foreach(InitSkill skill in InitSkillList) skill.boardEffect.Execute(this);

        foreach(EnemySkill skill in enemySkillList) skill.Init();
        nextSkill = GetNextSkill();
        if(nextSkill != null)
        {
            nextSkill.requirement.isSelected();
            enemyUI.SetInterval(nextSkill.requirement.GetAttackUIText());
        }
        canAttack = true;
    }

    public void AddShield(Shield shield)
    {
        shieldList.Add(shield);
        if(shieldList.Count == 1) enemyUI.EnableShield(shieldList[0].shieldImage, shieldList[0].shieldColor, shieldList[0].GetShieldText());
    }

    public async void Damage(AttackRBlock attackRBlock) //ダメージ処理
    {
        DamageUI damageUI = Addressables.InstantiateAsync("DamageCanvas").WaitForCompletion().GetComponent<DamageUI>();
        if (hp == 0) Kill();
        bool isWeak = false;
        if(shieldList.Count > 0) 
        {
            if(shieldList[0].CanDestroy(attackRBlock.Power)) 
            {
                shieldList.RemoveAt(0); //シールドが破壊される場合はシールドを削除
                if(shieldList.Count != 0) enemyUI.EnableShield(shieldList[0].shieldImage, shieldList[0].shieldColor, shieldList[0].GetShieldText());
                else enemyUI.DisableShield();
            }
            else 
            {
                damageUI.Active(this, 0, false);
                return;
            }
        }

        foreach(ColorType colorType in weakColorList)
        {
            if(attackRBlock.ColorType.Contains(colorType))
            {
                isWeak = true;
                break;
            }
        }

        if(isWeak) hp -= attackRBlock.Power * 2;
        else hp -= attackRBlock.Power;

        damageUI.Active(this, attackRBlock.Power, isWeak);

        hp = Mathf.Max(0, hp);
        await enemyUI.SetHP(hp);
        if (hp == 0) Kill();
    }

    public void Update()
    {
        //即時発動スキルの処理
        EnemySkill enemySkill = GetActivatableSkill();
        if(enemySkill != null)
        {
            nextSkill.requirement.isEnd();
            nextSkill = enemySkill;
        }

        if(nextSkill == null) return;
        if(nextSkill.IsAttack)
        {
            canAttack = false;
            Attack();
        }
        else enemyUI.SetInterval(nextSkill.requirement.GetAttackUIText());
    }

    public void Attack() //攻撃処理
    {
        canAttack = false;

        foreach(BaseEffectData boardEffect in nextSkill.boardEffectList) boardEffect.Execute(this);
        if(nextSkill.isOnce) enemySkillList.Remove(nextSkill);
        else nextSkill.requirement.isEnd();

        nextSkill = GetNextSkill();
        if(nextSkill != null)
        {
            nextSkill.requirement.isSelected();
            enemyUI.SetInterval(nextSkill.requirement.GetAttackUIText());
        }
        canAttack = true;
    }

    void Kill() //死亡時の処理
    {
        if(!isAlive) return;
        isAlive = false;

        //死亡時アニメーション
        this.gameObject.SetActive(false);
        EneM.OnKilled(this);
    }

    EnemySkill GetNextSkill()
    {
        int MaxProbability = 0;
        foreach(EnemySkill skill in enemySkillList) MaxProbability += skill.probability;
        int random = UnityEngine.Random.Range(0, MaxProbability);
        int probability = 0;
        foreach(EnemySkill skill in enemySkillList)
        {
            probability += skill.probability;
            if(random < probability) return skill;
        }
        return null;
    }

    EnemySkill GetActivatableSkill()
    {
        foreach(EnemySkill skill in enemySkillList)
        {
            if(skill.isImmediately && skill.IsAttack) return skill;
        }
        return null;
    }
}