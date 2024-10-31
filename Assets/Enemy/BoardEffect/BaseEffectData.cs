using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseEffectData : ScriptableObject
{
    public new string name; //識別用

    public abstract void Init();
    public abstract UniTask Execute(Enemy enemy);
}
