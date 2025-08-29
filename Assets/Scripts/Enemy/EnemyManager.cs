using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : BaseManager
{
    BattleManager batM;
    List<Enemy> enemyList;
    public List<Enemy> EnemyList { get { return enemyList; } }

    public override void Init()
    {
        batM = this.transform.parent.GetComponent<BattleManager>();
        enemyList = new List<Enemy>();
    }

    public void Generate(List<EnemyData> enemyDataList)
    {
        if (enemyDataList == null || enemyDataList.Count == 0) return;
        foreach (EnemyData enemyData in enemyDataList)
        {
            Vector2 pos = batM.transform.position + enemyData.pos;
            Enemy enemy = Instantiate(enemyData.obj, pos, Quaternion.identity).AddComponent<Enemy>();
            enemy.transform.SetParent(this.transform);
            enemyList.Add(enemy);
            enemy.Init(this);
            enemy.Generate(enemyData);
            enemy.gameObject.SetActive(false);
        }
    }

    public async void StartBattle()
    {
        foreach(Enemy enemy in enemyList)
        {
            enemy.gameObject.SetActive(true);
            await enemy.StartBattle();
        }
    }

    public void AlertKill(Enemy enemy)
    {
        enemyList.Remove(enemy);
        if(enemyList.Count == 0) batM.ClearBattle(); 
    }
}
