using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AddShield", menuName = "Effect/CreateAddShield")]
public class AddShield : BaseEffectData
{
    public int threshold;
    [Tooltip("trueなら閾値未満の時にシールドが発動する")]
    public bool isLower = false;
    public Color shieldColor;
    public Sprite shieldImage;

    public override void Init(Enemy enemy)
    {
        this.enemy = enemy;
        return;
    }

    public override UniTask Execute()
    {
        BlockNumShield shield = new BlockNumShield(threshold, isLower, shieldColor, shieldImage);
        enemy.GetComponent<Enemy>().SetShield(shield);
        return UniTask.CompletedTask;
    }
}

public abstract class Shield
{
    public Sprite shieldImage;
    public Color shieldColor;
    public abstract bool CanDestroy(int power);
    public abstract string GetShieldText();
}

public class BlockNumShield : Shield
{
    int threshold = 0; //シールドの閾値
    bool isLower = false;
    public bool IsLower { get => isLower;}

    public BlockNumShield(int threshold, bool isLower, Color color, Sprite sprite)
    {
        this.threshold = threshold;
        this.isLower = isLower;
        shieldColor = color;
        shieldImage = sprite;
    }

    public override bool CanDestroy(int power)
    {
        if(isLower == true)
        {
            if(power < threshold) return true;
        }
        else
        {
            if(power > threshold) return true;
        }
        return false;
    }    

    public override string GetShieldText()
    {
        if(isLower) return "<" + threshold.ToString();
        else return ">" + threshold.ToString();
    }
}