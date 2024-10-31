using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class Enemy : MonoBehaviour
{
    public new string name { get; private set; }
    public int maxHp { get; private set; } //HPの最大値;
    public int hp { get; private set; } //現在のHP
    List<EnemySkill> skillList; //スキルリスト
    List<InitSkill> InitSkillList; //初期スキルリスト
    EnemySkill nextSkill; //次に発動するスキル
    List<Shield> shieldList = new List<Shield>();
    List<ColorType> weakColorList = new List<ColorType>();

    public bool attackNow = false;
    public bool isAlive = true;

    EnemyUI enemyUI;
    EnemyManager EneM;

    public void Awake()
    {
        EneM = FindFirstObjectByType<EnemyManager>();
    }

    public void Init(EnemyData enemyData) //初期化
    {
        maxHp = enemyData.hp;
        hp = maxHp;
        name = enemyData.name;
        skillList = new List<EnemySkill>(enemyData.skillList);
        InitSkillList = enemyData.InitSkillList;
        weakColorList = enemyData.weakColorList;
        this.name = enemyData.name;

        if(skillList.Count > 0)
        {
            nextSkill = skillList[UnityEngine.Random.Range(0, skillList.Count)];
        }

        GameObject enemyCanvas = Addressables.InstantiateAsync("EnemyCanvas").WaitForCompletion();
        enemyCanvas.name = "EnemyCanvas";
        enemyCanvas.transform.SetParent(this.transform);
        enemyCanvas.transform.position = this.transform.position + new Vector3(0,0,0);
        enemyCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        enemyUI = enemyCanvas.GetComponent<EnemyUI>();
        enemyUI.Init(name, hp);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 敵の行動を開始
    /// </summary>
    public async void PlayEnemy()
    {
        foreach(EnemySkill skill in skillList) 
        {
            skill.Init(this);
            if(skill.isImmediately) skill.requirement.isSelected();
        }

        foreach(InitSkill skill in InitSkillList) await skill.boardEffect.Execute(this);

        nextSkill = GetRandomSkill();
        if(nextSkill != null)
        {
            nextSkill.requirement.isSelected();
            enemyUI.SetInterval(nextSkill.requirement.GetAttackUIText());
        }
    }

    public void AddShield(Shield shield)
    {
        shieldList.Add(shield);
        if(shieldList.Count == 1) enemyUI.SetShield(shieldList[0].shieldImage, shieldList[0].shieldColor, shieldList[0].GetShieldText());
    }

    public async void Damage(AttackRBlock attackRBlock) //ダメージ処理
    {
        DamageUI damageUI = Addressables.InstantiateAsync("DamageCanvas").WaitForCompletion().GetComponent<DamageUI>();
        if (hp == 0) OnKill();
        int weaknessMultiplier = 1;
        if(shieldList.Count > 0) 
        {
            if(shieldList[0].CanDestroy(attackRBlock.power)) 
            {
                shieldList.RemoveAt(0); //シールドが破壊される場合はシールドを削除
                if(shieldList.Count != 0) enemyUI.SetShield(shieldList[0].shieldImage, shieldList[0].shieldColor, shieldList[0].GetShieldText());
                else enemyUI.DisableShield();
            }
            else 
            {
                damageUI.Generate(this, 0, false);
                return;
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
        int damage = attackRBlock.power * weaknessMultiplier;

        damageUI.Generate(this, attackRBlock.power, weaknessMultiplier != 1);

        await enemyUI.SetHP(hp - damage);
        hp = Mathf.Max(0, hp - damage);
        if (hp == 0) OnKill();
    }

    public void Update()
    {
        //即時発動スキルの処理
        EnemySkill ImmediatelySkill = CheckImmediatelySkill();
        if(ImmediatelySkill != null) 
        {
            _ = Attack(ImmediatelySkill);
        }

        if(nextSkill == null) return;

        enemyUI.SetInterval(nextSkill.requirement.GetAttackUIText());
        if(nextSkill.IsAttack) _ = Attack(nextSkill);  
    }

    public async UniTask Attack(EnemySkill enemySkill) //攻撃処理
    {
        if(attackNow) return;
        attackNow = true;

        foreach(BaseEffectData boardEffect in enemySkill.boardEffectList) await boardEffect.Execute(this);
        enemySkill.requirement.isEnd();
        if(enemySkill.isOnce) skillList.Remove(enemySkill);

        nextSkill = GetRandomSkill();
        if(nextSkill != null)
        {
            nextSkill.requirement.isSelected();
            enemyUI.SetInterval(nextSkill.requirement.GetAttackUIText());
        }

        attackNow = false;
    }

    void OnKill() //死亡時の処理
    {
        if(!isAlive) return;
        isAlive = false;

        //死亡時アニメーション
        this.gameObject.SetActive(false);
        EneM.AlertKill(this);
    }

    EnemySkill GetRandomSkill()
    {
        int MaxProbability = 0; //スキルの発動確率の合計
        foreach(EnemySkill skill in skillList) MaxProbability += skill.probability;

        int random = UnityEngine.Random.Range(0, MaxProbability);
        int probability = 0;
        foreach(EnemySkill skill in skillList)
        {
            probability += skill.probability;
            if(random < probability) return skill;
        }
        return null;
    }

    EnemySkill CheckImmediatelySkill()
    {
        foreach(EnemySkill skill in skillList)
        {
            if(skill.isImmediately && skill.IsAttack) return skill;
        }
        return null;
    }
}