using UnityEditor.SceneManagement;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    StageManager staM;
    MainGameManager gamM;
    MainInputAction mainInputAction;

    float holdInterval = 0.05f;
    float lastInputTime = 0;
    float holdMoveTime = 0;
    bool isMoveHold = false;
    Vector2Int holdShiftOffset = Vector2Int.zero;

    public void Init()
    {
        staM = StageManager.Instance;
        mainInputAction = new MainInputAction();
        mainInputAction.Player.Move.started += context => Shift(context.ReadValue<Vector2>());
        mainInputAction.Player.Move.performed += context => HoldShift(context.ReadValue<Vector2>(), true);
        mainInputAction.Player.Move.canceled += context => HoldShift(Vector2.zero, false);

        mainInputAction.Player.Rotate.performed += context => Rotate(context.ReadValue<float>());

        mainInputAction.Player.Drop.started += context => Drop();

        mainInputAction.Player.Hold.performed += context => Hold();
        mainInputAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // lastInputTime += Time.deltaTime; // TODO これのせいで自動落下しない
        if(lastInputTime >= GameStatus.fallTime) gamM.Shift(Vector2Int.down);

        if(isMoveHold)
        {
            holdMoveTime += Time.deltaTime;
            if(holdMoveTime >= holdInterval)
            {
                Shift(holdShiftOffset);
                holdMoveTime = 0;
            }
        }
    }

    public void NextBattle()
    {
        gamM = staM.GetCurBattle().gamM;
    }

    void Shift(Vector2 offset) //左右移動
    {   
        if (gamM.playerCBlock == null) return;
        gamM.Shift(Vector2Int.RoundToInt(offset));
    }

    void HoldShift(Vector2 offset, bool performed) //ホールド移動
    {   
        if (gamM.playerCBlock == null) return;
        isMoveHold = performed;
        holdShiftOffset = Vector2Int.RoundToInt(offset);
    }

    void Rotate(float value) //回転
    {
        if (gamM.playerCBlock == null) return;
        gamM.Rotate(value * -90);
        lastInputTime = 0;
    }

    void Drop() //落下
    {
        if (gamM.playerCBlock == null) return;
        lastInputTime = 0;
        gamM.Drop();
    }

    void Hold() //ホールド
    {

    }
}