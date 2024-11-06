using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseEffectData : ScriptableObject
{
    public bool isWait = true; //終了まで待機するか
    protected Enemy enemy;

    public abstract void Init(Enemy enemy);
    public abstract UniTask Execute();
}
