using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    [SerializeField]
    float holdInterval = 0.05f;
    float previousTime = 0;
    MainInputAction mainInputAction;
    MainGameManager GamM;

    bool isMoveHold = false;
    float moveValue = 0;
    float holdMoveTime = 0;

    bool isDropHold = false;
    float dropValue = 0;
    float holdDropTime = 0;


    // Start is called before the first frame update
    public void Start()
    {
        mainInputAction = new MainInputAction();
        mainInputAction.Player.Move.started += context => Move(context.ReadValue<float>());
        mainInputAction.Player.Move.performed += context => HoldMove(context.ReadValue<float>(), true);
        mainInputAction.Player.Move.canceled += context => HoldMove(0, false);

        mainInputAction.Player.Rotate.performed += context => OnRotate(context.ReadValue<float>());

        mainInputAction.Player.Drop.started += context => Drop(context.ReadValue<float>());
        mainInputAction.Player.Drop.performed += context => HoldDrop(context.ReadValue<float>(), true);
        mainInputAction.Player.Drop.canceled += context => HoldDrop(0, false);

        mainInputAction.Player.Hold.performed += context => OnHold();
        mainInputAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if(GamM == null) return;
        previousTime += Time.deltaTime;
        if(GamM.playerBlock != null && previousTime >= GameStatus.fallTime) Drop(-1);

        if(isMoveHold)
        {
            holdMoveTime += Time.deltaTime;
            if(holdMoveTime >= holdInterval)
            {
                Move(moveValue);
                holdMoveTime = 0;
            }
        }
        if(isDropHold)
        {
            holdDropTime += Time.deltaTime;
            if(holdDropTime >= holdInterval)
            {
                Drop(dropValue);
                holdDropTime = 0;
            }
        }
    }

    void Move(float value) //左右移動
    {   
        if(GamM == null || GamM.playerBlock == null) return;
        GamM.playerBlock.Transform(new Vector3Int((int)value, 0, 0));
        previousTime = 0;
    }

    void HoldMove(float value, bool performed) //ホールド移動
    {   
        isMoveHold = performed;
        moveValue = value;
    }

    void OnRotate(float value) //回転
    {
        if(GamM == null || GamM.playerBlock == null) return;
        if(value == 1) GamM.playerBlock.Rotation(Vector3.back);
        else if(value == -1) GamM.playerBlock.Rotation(Vector3.forward);
        previousTime = 0;
    }

    void Drop(float value) //落下
    {
        if(GamM == null || GamM.playerBlock == null) return;
        previousTime = 0;
        if(value == -1) 
        {
            if(GamM.playerBlock.Transform(Vector3Int.down)) 
            {
                OnGround();
            }
        }
        else if(value == 1) 
        {
            int downNum = 0;
            while(!GamM.playerBlock.Transform(Vector3Int.down))
            {
                if(downNum > 20)
                {
                    Debug.Log("落下しすぎ");
                    break;
                }
            }
            OnGround();
        }
    }

    void OnGround() //プレイヤーブロックが地面に着地した時
    {
        holdDropTime = 0;
        GamM.TurnEnd();
    }

    void HoldDrop(float value, bool performed)
    {
        if(value == 1) return;
        isDropHold = performed;
        dropValue = value;
    }

    void OnHold() //ホールド
    {
        if(GamM == null || GamM.playerBlock == null) return;
        previousTime = 0;
        GamM.playerBlock.Destroy();
        OnGround();

        //GamM.SetHoldBlock();
    }

    public void SetGameManager(MainGameManager gamM)
    {
        GamM = gamM;
    }
}