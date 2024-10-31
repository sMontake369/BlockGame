using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class OccurRequirement : ScriptableObject //発生条件(条件を満たすとこのスキルを発動できる)
{
    protected Enemy enemy;
    public abstract void Init(Enemy enemy); //初期化
    public abstract bool IsOccur(); //発生条件を満たしているか
}