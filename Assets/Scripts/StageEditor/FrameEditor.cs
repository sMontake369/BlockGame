// #if UNITY_EDITOR

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using System;
// using UnityEngine.EventSystems;
// using Unity.VisualScripting;
// using UnityEditor;

// public class FrameEditor : MonoBehaviour
// {
//     EditorManager EdiM;
//     public TMP_InputField nameInputField;
//     public TMP_InputField xInputField;
//     public TMP_InputField yInputField;
//     public TMP_Dropdown colorTypeDropdown;
//     public TMP_Dropdown blockTypeDropdown;
//     public TMP_Dropdown frameDataDropdown;
//     public GameObject textureContentsObj;
//     FrameEditMode mode = FrameEditMode.block;
//     Vector3 screenPos;

//     [Header("全てのFrameDataのリスト")]
//     public FrameDataList frameDataList;
//     ContainerBlock frameRBlock;
//     ContainerBlock backGroundRBlock;
//     ContainerBlock selectRBlock;
//     Texture selectedTexture;
//     FrameData selectedFrameData;
//     Vector2Int frameSize;

//     void Start()
//     {
//         EdiM = FindFirstObjectByType<EditorManager>();
//         //ColorTypeのドロップダウンリストを作成
//         foreach (EBlockColor Value in Enum.GetValues(typeof(EBlockColor))) 
//         colorTypeDropdown.options.Add(new TMP_Dropdown.OptionData(Enum.GetName(typeof(EBlockColor), Value)));
//         colorTypeDropdown.value = 0;
//         colorTypeDropdown.RefreshShownValue();

//         //blockTypeのドロップダウンリストを作成
//         foreach (EBlockType Value in Enum.GetValues(typeof(EBlockType)))
//         blockTypeDropdown.options.Add(new TMP_Dropdown.OptionData(Enum.GetName(typeof(EBlockType), Value)));
//         blockTypeDropdown.value = 1;
//         blockTypeDropdown.RefreshShownValue();

//         frameDataDropdown.options.Add(new TMP_Dropdown.OptionData("選択"));
//         foreach(FrameData frameData in frameDataList.blockDataList)
//         frameDataDropdown.options.Add(new TMP_Dropdown.OptionData(frameData.name));
//         colorTypeDropdown.value = 0;
//         frameDataDropdown.RefreshShownValue();

//         //テクスチャのリストを作成
//         List<Texture2D> texture2DList = EditorManager.GetAddressableAsset<Texture2D>("Texture2D");
//         foreach (var texture2D in texture2DList)
//         {
//             RawImage image = new GameObject().AddComponent<RawImage>();
//             image.name = texture2D.name;
//             image.transform.SetParent(textureContentsObj.transform);
//             image.texture = texture2D;
//             image.rectTransform.localScale = new Vector3(1, 1, 1);

//             EventTrigger eventTrigger = image.AddComponent<EventTrigger>();
//             EventTrigger.Entry eventEntry = new EventTrigger.Entry();
//             eventEntry.eventID = EventTriggerType.PointerDown;
//             eventEntry.callback.AddListener((data) => OnTextureClick(data));
//             eventTrigger.triggers.Add(eventEntry);
//         }
//         selectedTexture = texture2DList[0];

//         //frameの生成
//         frameSize = new Vector2Int(int.Parse(xInputField.text), int.Parse(yInputField.text));
//         EdiM.FraM.SetFrame(EdiM.MakeFrameData(frameSize));
//         EdiM.GeneratePanel(frameSize);

//         //rootBlockの生成
//         frameRBlock = EdiM.GamM.GenerateRBlock();
//         backGroundRBlock = EdiM.GamM.GenerateRBlock();

//         SetGenMode();
//     }

//     public void Update()
//     {
//         if(mode == FrameEditMode.None) return;
//         screenPos = new Vector3(
//             Mathf.Round(EdiM.mousePos.x * 2) / 2,
//             Mathf.Round(EdiM.mousePos.y * 2) / 2,
//             Mathf.Round(EdiM.mousePos.z * 2) / 2
//         );
//         screenPos.z = 0;
//         // EdiM.pointerObj.transform.position = screenPos;

//         //左クリック時の処理
//         if(Input.GetMouseButton(0)) OnLeftClick();
//         //右クリック時の処理
//         if(Input.GetMouseButton(1)) OnRightClick();
//     }

//     void OnLeftClick()
//     {
//         GenerateBlock();
//     }
//     void OnRightClick()
//     {
//         DeleteBlock();
//     }

//     void GenerateBlock()
//     {
//         Vector3Int pos = Vector3Int.RoundToInt(screenPos);
//         if(EdiM.FraM.GetBlock(pos) == null)
//         {
//             Block block = EdiM.GamM.GenerateBlock((EBlockType)blockTypeDropdown.value, (EBlockColor)colorTypeDropdown.value, selectedTexture);
//             block.frameIndex = pos;
//             selectRBlock.AddBlock(block, pos);
//             EdiM.FraM.SetBlock(block); 
//         }
//     }

//     void DeleteBlock()
//     {
//         Vector3Int pos = Vector3Int.RoundToInt(screenPos);
//         foreach(Block block in selectRBlock.BlockList)
//         if(block.shapeIndex == pos)
//         {
//             EdiM.FraM.DeleteBlock(block);
//             block.DestroyBlock(false, false);
//             break;
//         }
//     }

//     public void OnTextureClick(BaseEventData data)
//     {
//         GameObject pointerObject = (data as PointerEventData).pointerEnter;
//         Debug.Log(pointerObject);
//         selectedTexture = pointerObject.GetComponent<RawImage>().texture;
//     }

//     // public void OnClickLoad()
//     // {
//     //     frameRBlock.Destroy();
//     //     backGroundRBlock.Destroy();
//     //     selectedFrameData = null;
//     //     foreach(FrameData frameData in frameDataList.blockDataList)
//     //     if(frameData.name == frameDataDropdown.options[frameDataDropdown.value].text)
//     //     {
//     //         selectedFrameData = frameData;
//     //         nameInputField.text = frameData.name;
//     //         break;
//     //     }

//     //     if(selectedFrameData == null) frameSize = new Vector2Int(int.Parse(xInputField.text), int.Parse(yInputField.text));
//     //     else frameSize = new Vector2Int(selectedFrameData.frameSize.x, selectedFrameData.frameSize.y);
//     //     EdiM.FraM.EditFrame(frameSize);
//     //     EdiM.GeneratePanel(frameSize);

//     //     if(selectedFrameData == null) return;

//     //     xInputField.text = frameSize.x.ToString();
//     //     yInputField.text = frameSize.y.ToString();
//     //     frameRBlock = EdiM.GamM.GenerateRBlock();
//     //     foreach(BlockData posTextureType in selectedFrameData.framePosList)
//     //     {
//     //         BaseBlock block = EdiM.GamM.GenerateBlock(posTextureType.blockType, (ColorType)colorTypeDropdown.value, posTextureType.texture);
//     //         frameRBlock.AddBlock(block, posTextureType.blockPos);
//     //     }
//     //     backGroundRBlock = EdiM.GamM.GenerateRBlock();
        
//     //     SetGenMode();
//     // }

//     public void OnClickDelete()
//     {
//         if(selectedFrameData == null) return;
//         frameRBlock.ExitFrame();
//         UnityEditor.AssetDatabase.DeleteAsset("Assets/ContentsData/FrameData/" + selectedFrameData.name + ".asset");
//         UnityEditor.AssetDatabase.SaveAssets();
//         frameDataList.blockDataList.Remove(selectedFrameData);
//         frameDataDropdown.options.RemoveAt(frameDataDropdown.value);
//         frameDataDropdown.value = 0;
//         frameDataDropdown.RefreshShownValue();
//         selectedFrameData = null;

//         frameRBlock = EdiM.GamM.GenerateRBlock();
//         EdiM.FraM.SetRBlock(frameRBlock);
//     }

//     // public void OnClickSave()
//     // {
//     //     string name = nameInputField.text;

//     //     if(string.IsNullOrEmpty(name) || frameRBlock.GetBlockNum() == 0)
//     //     {
//     //         Debug.Log("FrameData is empty or Name is empty!");
//     //         return;
//     //     }

//     //     bool isNew = false;
//     //     if(selectedFrameData == null) 
//     //     {
//     //         isNew = true;
//     //         selectedFrameData = ScriptableObject.CreateInstance<FrameData>();
//     //     }

//     //     //FrameDataの設定
//     //     selectedFrameData.name = name;
//     //     selectedFrameData.framePosList = new List<BlockData>();
//     //     foreach(BaseBlock block in frameRBlock.BlockList) selectedFrameData.framePosList.Add(
//     //         new BlockData { blockPos = block.shapeIndex, texture = block.mainRenderer.material.mainTexture, blockType = block.blockType });

//     //     selectedFrameData.frameSize = new Vector3Int(frameSize.x, frameSize.y, 0);
//     //     selectedFrameData.moveSize = new BorderInt(new Vector3Int(0, 0, 0), new Vector3Int(frameSize.x, frameSize.y, 0));

//     //     //BlockDataの保存
//     //     string path = "Assets/ContentsData/FrameData/" + name + ".asset";
//     //     if(isNew == true)
//     //     {
//     //         UnityEditor.AssetDatabase.CreateAsset(selectedFrameData, path);
//     //         frameDataList.blockDataList.Add(selectedFrameData);
//     //         frameDataDropdown.options.Add(new TMP_Dropdown.OptionData(selectedFrameData.name));
//     //         frameDataDropdown.value = frameDataList.blockDataList.Count;
//     //         frameDataDropdown.RefreshShownValue();
//     //         Debug.Log("Save CreateData");
//     //     }
//     //     else 
//     //     {
//     //         EditorUtility.SetDirty(selectedFrameData);
//     //         UnityEditor.AssetDatabase.SaveAssets();
//     //         Debug.Log("Save FrameData");
//     //     }
//     // }

//     public void SetGenMode() {
//         Debug.Log("SetGenMode");
//         mode = FrameEditMode.block;
//         EdiM.FraM.DeleteRBlock(backGroundRBlock);
//         EdiM.FraM.SetRBlock(frameRBlock);
//         backGroundRBlock.gameObject.SetActive(false);
//         frameRBlock.gameObject.SetActive(true);
//         selectRBlock = frameRBlock;
//     }
//     public void SetBackGround() {
//         Debug.Log("SetBackGround");
//         mode = FrameEditMode.backGround;
//         EdiM.FraM.DeleteRBlock(frameRBlock);
//         EdiM.FraM.SetRBlock(backGroundRBlock);
//         frameRBlock.gameObject.SetActive(false);
//         backGroundRBlock.gameObject.SetActive(true);
//         selectRBlock = backGroundRBlock;
//     }

// }

// enum FrameEditMode
// {
//     block,
//     backGround,
//     None
// }
// #endif