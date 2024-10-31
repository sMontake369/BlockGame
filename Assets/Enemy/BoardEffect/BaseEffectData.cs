using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseEffectData : ScriptableObject
{
    public new string name; //識別用
    protected Enemy enemy;

    public abstract void Init(Enemy enemy);
    public abstract UniTask Execute();
}
