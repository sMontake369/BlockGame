using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine;

public class Menu : MonoBehaviour
{
    MainInputAction mainInputAction;
    public GameObject worldObj;
    public GameObject player;
    WorldShape worldShape;
    // Start is called before the first frame update
    void Start()
    {
        mainInputAction = new MainInputAction();
        mainInputAction.UI.Move.started += Move;
        mainInputAction.Enable();
        worldShape = worldObj.GetComponent<WorldShape>();
    }

    async void Move(InputAction.CallbackContext context)
    {
        int input = (int)context.ReadValue<float>();
        mainInputAction.Disable();
        worldShape.Rotate(input);
        await player.transform.DOLocalJump(player.transform.localPosition, 0.5f, 1, 0.25f);
        mainInputAction.Enable();
    }
}
