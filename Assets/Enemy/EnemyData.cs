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
    [Header("敵のイベントリスト")]
    public List<EnemyEvent> eventList;
}

[System.Serializable]
public class EnemySkill
{
    [JsonIgnore]
    public List<BaseEffectData> boardEffectList;
    public AttackRequirement AttackReq; //攻撃の発動条件(条件を満たすと攻撃を行う)
    public List<OccurRequirement> OccurReqList; //スキルの発生条件(条件を満たすとこのスキルを発動できる)
    public int probability = 1; //発動確率
    public bool isOnce; //一度だけ発動するか

    public void Init(Enemy enemy)
    {
        foreach(BaseEffectData effect in boardEffectList) effect.Init(enemy);
        foreach(OccurRequirement occurReq in OccurReqList) occurReq.Init(enemy);
        AttackReq.Init(enemy);
    }

    public bool IsOccur()
    {
        foreach(OccurRequirement req in OccurReqList) if(!req.IsOccur()) return false;
        return true;
    }
}

[System.Serializable]
public class EnemyEvent
{
    public List<BaseEffectData> boardEffectList;
    public List<OccurRequirement> occurReqList; //スキルの発生条件(条件を満たすとこのスキルを発動できる)

    public void Init(Enemy enemy)
    {
        foreach(BaseEffectData effect in boardEffectList) effect.Init(enemy);
        foreach(OccurRequirement req in occurReqList) req.Init(enemy);
        if(occurReqList.Count == 0) Debug.LogError("occurReqListが空のため開始時に発動します");
    }

    public bool isOccur()
    {
        foreach(OccurRequirement req in occurReqList) if(!req.IsOccur()) return false;
        return true;
    }
}