using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public static FrameManager FM;
    [SerializeField]
    public static TextMeshProUGUI Map;
    private static List<List<BaseBlock>> blockList = new List<List<BaseBlock>>();
    //int count = 0;

    public void Awake()
    {
        Map = GetComponent<TextMeshProUGUI>();
    }

    public void Update()
    {
        //count++;
        //if(count % 100 == 0) UpdateText();
    }

    // Update is called once per frame
    public static void UpdateText() //スコアは足していくから逆になる
    {
        if(FM == null)
        {
            FM = FindFirstObjectByType<FrameManager>();
            if(!FM) return;
        }
        blockList = FM.FrameListList;
        Map.text = "";
        for(int y = blockList.Count - 1; 0 <= y ; y--) //下に改行するから逆になる
        {
            for(int x = 0; x < blockList[y].Count; x++)
            {
                if(blockList[y][x] == null)
                {
                    Map.text += "＿";
                    continue;
                    //if(y < frameManager.ValidFrameArray.GetLength(0) && x < frameManager.ValidFrameArray.GetLength(1) && frameManager.ValidFrameArray[y, x] == 1) Map.text += "■";
                    //else Map.text += "？";
                }
                else if (blockList[y][x].blockType == BlockType.Air)
                {
                    Map.text += "　";
                }
                else if (blockList[y][x].blockType == BlockType.Mino)
                {
                    Map.text += "□";
                }
                else if (blockList[y][x].blockType == BlockType.Wall)
                {
                    Map.text += "■";
                }
                else {
                    Map.text += "＊";
                }
            }
            Map.text += "\n";
        }
    }
}
