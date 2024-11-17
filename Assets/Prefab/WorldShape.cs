using System.Collections;
using System.Collections.Generic;
using ExternPropertyAttributes;
using UnityEngine;
using NaughtyAttributes;

public class WorldShape : MonoBehaviour
{
    public int StageNum = 1;
    int curRot = 0;
    List<GameObject> stages = new List<GameObject>();
    public Material material;
    void Start()
    {
        for(int i = 0; i < StageNum; i++)
        {
            stages.Add(new GameObject("Stage" + i));
            stages[i].transform.parent = transform;
            stages[i].AddComponent<MeshFilter>();
            stages[i].AddComponent<MeshRenderer>();
            stages[i].AddComponent<StageShape>().Create(10, i);
            stages[i].transform.position = new Vector3(0, 0, 0);
            stages[i].GetComponent<MeshRenderer>().material = material;

            if(i > 3) stages[i].gameObject.SetActive(false);
        }
    }

    [NaughtyAttributes.Button("Rotate")]
    public void Rotate(int rot = 1)
    {
        transform.Rotate(0, 0, 36 * rot);
        if(rot > 0)
        {
            if(curRot - 4 >= 0) stages[curRot - 4].gameObject.SetActive(false);
            if(curRot + 4 < stages.Count) stages[curRot + 4].gameObject.SetActive(true);
        }
        else
        {
            if(curRot - 4 >= 0) stages[curRot - 4].gameObject.SetActive(true);
            if(curRot + 4 < stages.Count) stages[curRot + 4].gameObject.SetActive(false);
        }
        curRot += rot;
    }
}
