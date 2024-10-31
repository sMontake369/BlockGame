using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "EnemyData", menuName = "CreateEnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("敵データの名前")]
    public new string name;
    [Header("敵のオブジェクト")]
    public GameObject obj;
    [Header("敵の位置")]
    public Vector3 pos;
    [Header("敵のHP")]
    public int hp;
    [Header("敵の弱点カラー")]
    public List<ColorType> weakColorList; 
    [Header("敵のスキルリスト")]
    public List<EnemySkill> skillList;
    [Header("敵の初期スキルリスト")]
    public List<InitSkill> InitSkillList;
    //public List<GameEvent> eventList;
}

[System.Serializable]
public class EnemySkill
{
    [JsonIgnore]
    public List<BaseEffectData> boardEffectList;
    [JsonIgnore]
    public EffectRequirement requirement;
    public int probability = 1; //発動確率
    public bool isOnce; //一度だけ発動するか
    public bool isImmediately; //即時発動するか

    [JsonIgnore]
    public bool IsAttack { get { return requirement.isAttack(); } }

    public void Init(Enemy enemy)
    {
        foreach(BaseEffectData effect in boardEffectList) effect.Init();
        requirement.Init(enemy);
    }
}

[System.Serializable]
public class InitSkill
{
    [JsonIgnore]
    public BaseEffectData boardEffect;
    //requirementをいれてもいいかも
}