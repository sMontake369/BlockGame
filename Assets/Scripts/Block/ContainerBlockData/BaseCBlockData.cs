using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class ContainerBlockData : ScriptableObject
{
    public Vector2 pivotPos;
    [SerializeField]
    public SerializeDictionary<Vector2Int, BaseBlockData> blockDataList;

    public ContainerBlock Generate()
    {
        ContainerBlock containerBlock = new GameObject("ContainerBlock").AddComponent<ContainerBlock>();
        containerBlock.Create(this);
        containerBlock.SetPivotPos(pivotPos);
        return containerBlock;
    }
}

[System.Serializable]
public class SerializeDictionary<TKey, TValue>
{
    [SerializeField] private List<DicElement> _dicElements = new List<DicElement>();
    private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

    [System.Serializable]
    public class DicElement
    {
        public TKey Key;
        public TValue Value;

        public DicElement(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    public Dictionary<TKey, TValue> GetDictionary
    {
        get
        {
            if (_dicElements != null)
            {
                _dictionary.Clear();
                foreach (var element in _dicElements)
                {
                    _dictionary.Add(element.Key, element.Value);
                }
                return _dictionary;
            }
            return null;
        }
    }
}