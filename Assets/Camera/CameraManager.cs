using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    BattleManager BatM;

    CinemachineVirtualCamera cinemaCamera;

    public void Init()
    {
        // return;
        // cameraObj = new GameObject("Camera");
        // cameraObj.transform.parent = this.transform;
        cinemaCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
        // cinemaCamera.Follow = cameraObj.transform;
    }

    public void SetPosAndOrtho(Vector3 pos, float orthoSize)
    {
        this.transform.position = pos;
        // this.transform.position += new Vector3(0, -18, 0);
        //DOVirtual.Float(cinemaCamera.m_Lens.OrthographicSize, orthoSize, 3f, value => { cinemaCamera.m_Lens.OrthographicSize = value; });
        DOVirtual.Float(cinemaCamera.m_Lens.FieldOfView, orthoSize, 3f, value => { cinemaCamera.m_Lens.FieldOfView = value; });
    }
}