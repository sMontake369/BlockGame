using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    BattleManager BatM;
    List<Enemy> enemyList;
    public List<Enemy> EnemyList { get { return enemyList; } }

    public void Init(BattleManager BatM)
    {
        this.BatM = BatM;

        if (this.BatM == null)
        {
            Debug.Log("BattleManager is not found");
            return;
        }

        enemyList = new List<Enemy>();
    }

    public void Generate(List<EnemyData> enemyDataList) //敵を召喚
    {
        foreach(EnemyData enemyData in enemyDataList)
        {
            if(enemyData == null) continue;
            Enemy enemy = Instantiate(enemyData.obj, BatM.battlePos.lowerLeft + enemyData.pos, Quaternion.identity).AddComponent<Enemy>();
            enemy.transform.SetParent(this.transform);
            enemy.gameObject.SetActive(false);
            enemyList.Add(enemy); 

            enemy.Init(this);
            enemy.Generate(enemyData);
        }
    }

    public async void PlayEnemy()
    {
        foreach(Enemy enemy in enemyList)
        {
            enemy.gameObject.SetActive(true);
            await enemy.Play();
        }

    }

    public void AlertKill(Enemy enemy)
    {
        enemyList.Remove(enemy);
        if(enemyList.Count == 0) BatM.ClearBattle(); 
    }
}
