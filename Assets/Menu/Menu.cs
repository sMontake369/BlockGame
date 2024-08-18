using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine;

public class Menu : MonoBehaviour
{
    MainInputAction mainInputAction;
    public GameObject circle;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        mainInputAction = new MainInputAction();
        mainInputAction.UI.Move.started += Move;
        mainInputAction.Enable();
    }

    async void Move(InputAction.CallbackContext context)
    {
        int input = (int)context.ReadValue<float>();
        mainInputAction.Disable();
        await DOTween.Sequence()
            .Join(circle.transform.DOLocalRotate(circle.transform.localRotation.eulerAngles + new Vector3(0, 36, 0) * input, 0.25f))
            .Join(circle.transform.DOLocalMoveY(circle.transform.localPosition.y + 0.1f * -input, 0.25f))
            .Join(player.transform.DOLocalJump(player.transform.localPosition, 0.3f, 1, 0.25f));
        mainInputAction.Enable();
    }
}
