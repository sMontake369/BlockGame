// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// #if UNITY_EDITOR
// public class test : MonoBehaviour
// {
//     // Start is called before the first frame update
//     void Start()
//     {
//         string path = "Assets/" + "test" + ".asset";
//         ContainerBlockData rootBlockData = new ContainerBlockData();
//         rootBlockData.blockPosList = new List<Vector3Int>();

//         for(int y = 0; y < 20; y++)
//         for(int x = 0; x < 14; x++)
//         {
//             rootBlockData.blockPosList.Add(new Vector3Int(x, y, 0));
//         }


//         UnityEditor.AssetDatabase.CreateAsset(rootBlockData, path);
//         Debug.Log("Create BlockData");
//     }
// }
// #endif