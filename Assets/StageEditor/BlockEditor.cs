using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEditor;

#if UNITY_EDITOR
public class BlockEditor : MonoBehaviour
{
    EditorManager EdiM;
    public TMP_InputField nameInputField;
    public TMP_Dropdown colorTypeDropdown;
    public TMP_Dropdown blockDataDropdown;
    public GameObject textureContentsObj;
    BlockEditMode mode = BlockEditMode.generateBlock;
    Vector3 screenPos;
    public GameObject pivotObj;

    [Header("全てのBlockDataのリスト")]
    public BlockBagData blockShapeData;
    RootBlock rootBlock;
    Texture selectedTexture;
    RootBlockData selectedBlockData;

    void Start()
    {
        EdiM = FindFirstObjectByType<EditorManager>();
        //ColorTypeのドロップダウンリストを作成
        foreach (ColorType Value in Enum.GetValues(typeof(ColorType))) 
        colorTypeDropdown.options.Add(new TMP_Dropdown.OptionData(Enum.GetName(typeof(ColorType), Value)));
        colorTypeDropdown.value = 0;
        colorTypeDropdown.RefreshShownValue();

        blockDataDropdown.options.Add(new TMP_Dropdown.OptionData("選択"));
        foreach(RootBlockData blockData in blockShapeData.blockDataList)
        blockDataDropdown.options.Add(new TMP_Dropdown.OptionData(blockData.name));
        colorTypeDropdown.value = 0;
        blockDataDropdown.RefreshShownValue();

        //テクスチャのリストを作成
        List<Texture2D> texture2DList = EditorManager.GetAddressableAsset<Texture2D>("Texture2D");
        foreach (var texture2D in texture2DList)
        {
            RawImage image = new GameObject().AddComponent<RawImage>();
            image.name = texture2D.name;
            image.transform.SetParent(textureContentsObj.transform);
            image.texture = texture2D;
            image.rectTransform.localScale = new Vector3(1, 1, 1);

            EventTrigger eventTrigger = image.AddComponent<EventTrigger>();
            EventTrigger.Entry eventEntry = new EventTrigger.Entry();
            eventEntry.eventID = EventTriggerType.PointerDown;
            eventEntry.callback.AddListener((data) => OnTextureClick(data));
            eventTrigger.triggers.Add(eventEntry);
        }
        selectedTexture = texture2DList[0];

        //frameの生成
        Vector2Int vector2Int = new Vector2Int(12, 12);
        EdiM.FraM.EditFrame(vector2Int);
        EdiM.GeneratePanel(vector2Int);

        //rootBlockの生成
        rootBlock = EdiM.GamM.GenerateRBlock();
    }

    public void Update()
    {
        if(mode == BlockEditMode.movePivot)
        {
            screenPos = new Vector3(
                Mathf.Round(EdiM.mousePos.x * 2) / 2,
                Mathf.Round(EdiM.mousePos.y * 2) / 2,
                Mathf.Round(EdiM.mousePos.z * 2) / 2
            );
        }
        else if(mode == BlockEditMode.generateBlock) screenPos = Vector3Int.RoundToInt(EdiM.mousePos);
        else return;
        screenPos.z = 0;
        EdiM.pointerObj.transform.position = screenPos;

        //左クリック時の処理
        if(Input.GetMouseButton(0)) OnLeftClick();
        //右クリック時の処理
        if(Input.GetMouseButton(1)) OnRightClick();
    }

    void OnLeftClick()
    {
        switch(mode)
        {
            case BlockEditMode.generateBlock:
                GenerateBlock();
                break;
            case BlockEditMode.movePivot:
                SetPivot();
                break;
        }
    }
    void OnRightClick()
    {
        DeleteBlock();
    }

    void GenerateBlock()
    {
        Vector3Int pos = Vector3Int.RoundToInt(screenPos);
        if(EdiM.FraM.IsWithinBoard(pos) && !EdiM.FraM.FrameListList[pos.y][pos.x].IsContain())
        {
            BaseBlock block = EdiM.GamM.GenerateBlock(BlockType.Mino, (ColorType)colorTypeDropdown.value, selectedTexture);
            block.mainRenderer.material.mainTexture = selectedTexture; //2度手間、いつか直す
            block.frameIndex = pos;
            rootBlock.AddBlock(block, pos);
            EdiM.FraM.SetFrame(block); 
        }
    }

    void SetPivot()
    {
        Vector3Int pos = Vector3Int.RoundToInt(screenPos);
        if(EdiM.FraM.IsWithinBoard(pos))
        pivotObj.transform.position = screenPos;
    }

    void DeleteBlock()
    {
        Vector3Int pos = Vector3Int.RoundToInt(screenPos);
        foreach(BaseBlock block in rootBlock.BlockList)
        if(block.shapeIndex == pos)
        {
            EdiM.FraM.DeleteBlock(block);
            rootBlock.BlockListList[block.shapeIndex.y][block.shapeIndex.x] = null;
            BlockPool.ReleaseNotBaseBlock(block);
            block.DeleteRootBlock();
            break;
        }
    }

    public void OnTextureClick(BaseEventData data)
    {
        GameObject pointerObject = (data as PointerEventData).pointerEnter;
        Debug.Log(pointerObject);
        selectedTexture = pointerObject.GetComponent<RawImage>().texture;
    }

    public void OnClickLoad()
    {
        rootBlock.Destroy();
        selectedBlockData = null;
        pivotObj.transform.position = new Vector3(0, 0, 0);
        foreach(RootBlockData blockData in blockShapeData.blockDataList)
        if(blockData.name == blockDataDropdown.options[blockDataDropdown.value].text)
        {
            Debug.Log(blockData.name);
            selectedBlockData = blockData;
            nameInputField.text = blockData.name;
            colorTypeDropdown.value = (int)blockData.colorType;
            colorTypeDropdown.RefreshShownValue();
            break;
        }

        if(selectedBlockData == null) return;
        rootBlock = EdiM.GamM.GenerateRBlock(selectedBlockData);
        pivotObj.transform.position = selectedBlockData.pivotPos;
        EdiM.FraM.SetRFrame(rootBlock);
    }

    public void OnClickDelete()
    {
        if(selectedBlockData == null) return;
        rootBlock.Destroy();
        UnityEditor.AssetDatabase.DeleteAsset("Assets/ContentsData/BlockShape/" + selectedBlockData.name + ".asset");
        UnityEditor.AssetDatabase.SaveAssets();
        blockShapeData.blockDataList.Remove(selectedBlockData);
        blockDataDropdown.options.RemoveAt(blockDataDropdown.value);
        blockDataDropdown.value = 0;
        blockDataDropdown.RefreshShownValue();
        selectedBlockData = null;

        rootBlock = EdiM.GamM.GenerateRBlock();
        pivotObj.transform.position = new Vector3(0, 0, 0);
        EdiM.FraM.SetRFrame(rootBlock);
    }

    public void OnClickSave()
    {
        string name = nameInputField.text;

        if(string.IsNullOrEmpty(name) || rootBlock.GetBlockNum() == 0)
        {
            Debug.Log("BlockData is empty or Name is empty!");
            return;
        }

        bool isNew = false;
        if(selectedBlockData == null) 
        {
            isNew = true;
            selectedBlockData = ScriptableObject.CreateInstance<RootBlockData>();
        }
        selectedBlockData.blockPosList = new List<Vector3Int>();

        //BlockDataの設定
        selectedBlockData.name = name;
        selectedBlockData.colorType = (ColorType)colorTypeDropdown.value;
        selectedBlockData.blockType = BlockType.Mino;
        selectedBlockData.pivotPos = pivotObj.transform.position;
        foreach(BaseBlock baseBlock in rootBlock.BlockList) selectedBlockData.blockPosList.Add(baseBlock.shapeIndex);

        //BlockDataの保存
        string path = "Assets/ContentsData/RootBlock/" + name + ".asset";
        if(isNew == true)
        {
            UnityEditor.AssetDatabase.CreateAsset(selectedBlockData, path);
            blockShapeData.blockDataList.Add(selectedBlockData);
            blockDataDropdown.options.Add(new TMP_Dropdown.OptionData(selectedBlockData.name));
            blockDataDropdown.value = blockShapeData.blockDataList.Count;
            blockDataDropdown.RefreshShownValue();
            Debug.Log("Create BlockData");
        }
        else 
        {
            EditorUtility.SetDirty(selectedBlockData);
            UnityEditor.AssetDatabase.SaveAssets();
            Debug.Log("Save BlockData");
        }
    }

    public void SetGenMode() 
    {
        mode = BlockEditMode.generateBlock;
        Debug.Log("SetGenMode");
    }
    public void SetPivMode() 
    {
        mode = BlockEditMode.movePivot;
        Debug.Log("SetPivMode");
    }

}

enum BlockEditMode
{
    generateBlock,
    movePivot,
    None
}
#endif