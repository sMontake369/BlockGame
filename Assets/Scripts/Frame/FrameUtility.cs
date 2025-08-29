using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FrameUtility
{
    public static List<List<ContainerBlock>> blockList;

    public static List<List<T>> Generate<T>(int X, int Y)
    {
        List<List<T>> listList = new List<List<T>>();
        for(int i = 0; i < Y; i++)
        {
            listList.Add(new List<T>());
            for(int j = 0; j < X; j++) listList[i].Add(default);
        }
        return listList;
    }

    /// <summary>
    /// 2次元リストの指定した位置に要素を挿入
    /// listList = 2次元リスト
    /// elementListList = 挿入する要素
    /// insertColumn = 挿入する列
    /// insertRow = 挿入する行
    /// </summary>
    public static void InsertListList<T>(List<List<T>> listList, List<List<T>> elementListList, int insertY, int insertX)
    {
        if(insertY < 0) insertY = 0;//リストの先頭に挿入
        if(insertY > listList.Count) insertY = listList.Count - 1;//リストの最後に挿入
        
        for(int y = elementListList.Count - 1; y >= 0 ; y--) //マイナスからやるの気持ち悪い
        {
            listList.Insert(insertY, new List<T>(elementListList[y].Count));
            
            for(int x = elementListList[y].Count - 1; x >= 0; x--) //マイナスからやるの気持ち悪い
            {
                listList[insertY].Insert(insertX, elementListList[y][x]);
            }
        }
    }

    /// <summary>
    /// 2次元リストの指定した位置に要素を挿入
    /// listList = 2次元リスト
    /// insertNum = 挿入する要素の数
    /// index = 挿入する位置
    /// </summary>
    public static void InsertListList<T>(List<List<T>> listList, Vector3Int index, Vector3Int insertNum)
    {   
        int column = listList[0].Count;
        for(int y = index.y; y < insertNum.y; y++)
        {
            listList.Insert(index.y, new List<T>(listList[0].Count)); //y軸にリストを追加
            for(int x = 0; x < column; x++) listList[index.y].Add(default); //x軸に要素を追加
        }

        for(int x = 0; x < insertNum.x; x++)
        {
            for(int y = 0; y < listList.Count; y++) listList[y].Insert(index.x, default); //x軸に要素を追加
        }
    }

//足りない要素
//配列をn行又は、n列追加する
//配列の間に要素を追加するときに、今ある要素をどうずらすか
//不足している要素を探索する

    public static List<List<T>> DeepCopy<T>(List<List<T>> listList)
    {
        List<List<T>> newListList = new List<List<T>>();
        for(int i = 0; i < listList.Count; i++) newListList.Add(new List<T>(listList[i]));
        return newListList;
    }
}
