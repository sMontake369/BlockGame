using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    BattleManager BatM;
    List<Enemy> enemyList = new List<Enemy>();
    public List<Enemy> EnemyList { get { return enemyList; } }

    public void Init()
    {
        BatM = FindFirstObjectByType<BattleManager>();
    }

    public void Generate(List<EnemyData> enemyDataList) //敵を召喚
    {
        foreach(EnemyData enemyData in enemyDataList)
        {
            Enemy enemy = Instantiate(enemyData.obj, BatM.battlePos.lowerLeft + enemyData.pos, Quaternion.identity).AddComponent<Enemy>();
            enemy.transform.SetParent(this.transform);
            enemy.Init(enemyData);
            enemyList.Add(enemy); 
        }
    }

    public void PlayEnemy()
    {
        foreach(Enemy enemy in enemyList)
        {
            enemy.gameObject.SetActive(true);
            enemy.PlayEnemy();
        }

    }

    public void AlertKill(Enemy enemy)
    {
        enemyList.Remove(enemy);
        if(enemyList.Count == 0) BatM.ClearBattle(); 
    }
}
