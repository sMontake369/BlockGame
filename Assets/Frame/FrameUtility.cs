using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FrameUtility
{
    public static List<List<BaseBlock>> blockList;

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
    /// 2次元リストの指定した位置に値を挿入
    /// listList = 2次元リスト
    /// elementListList = 挿入する要素
    /// insertColumn = 挿入する列
    /// insertRow = 挿入する行
    /// </summary>
    public static void InsertListList<T>(List<List<T>> listList, List<List<T>> elementListList, int insertY, int insertX)
    {
        if(insertY < 0) insertY = 0;//リストの先頭に挿入
        if(insertY > listList.Count) insertY = listList.Count - 1;//リストの先頭に挿入
        
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
    /// 1次元リストの指定した位置に値を挿入
    /// list = 1次元リスト
    /// element = 挿入する要素
    /// insertIndex = 挿入するインデックス
    /// </summary>
    public static void InsertList<T>(List<T> list, T element, int insertColumn)
    {
        list.Insert(insertColumn, element);
    }

    public static List<List<T>> DeepCopy<T>(List<List<T>> listList)
    {
        List<List<T>> newListList = new List<List<T>>();
        for(int i = 0; i < listList.Count; i++) newListList.Add(new List<T>(listList[i]));
        return newListList;
    }
}
