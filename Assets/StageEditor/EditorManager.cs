#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets;
using Unity.Mathematics;

public class EditorManager : MonoBehaviour
{

    [HideInInspector]
    public MainGameManager GamM;
    [HideInInspector]
    public FrameManager FraM;
    public Vector3 mousePos;
    Camera mainCamera;
    Vector3 tmpMousePos = new Vector3(0, 0, 0);
    public GameObject pointerObj;
    List<GameObject> panelList = new List<GameObject>();

    [Tooltip("stageDataをJsonファイルとして保存する")]
    public StageData stageData;

    // Start is called before the first frame update
    void Awake()
    {
        GamM = FindFirstObjectByType<MainGameManager>();
        FraM = FindFirstObjectByType<FrameManager>();
        GamM.Init();
        FraM.Init();
        mainCamera = Camera.main;
        GamM.SetEditorMode();
    }

    public void Start()
    {
        if(stageData) ReadWrite.Write("StageData.json", stageData);
    }

    void Update()
    {
        //カーソルの位置を取得
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z += 1;

        //カメラの移動範囲
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
        if(topRight.x - (0.3f * mainCamera.orthographicSize + 4) < mousePos.x) 
        {
            mousePos.x = topRight.x - (0.3f * mainCamera.orthographicSize + 4);
            return;
        }

        //カメラのズーム
        var scroll = Input.mouseScrollDelta.y;
        mainCamera.orthographicSize = math.clamp(mainCamera.orthographicSize - scroll, 1, 30);
        
        //カメラの移動
        if(Input.GetMouseButtonDown(2)) tmpMousePos = mousePos;
        if(Input.GetMouseButton(2)) mainCamera.transform.position += tmpMousePos - mousePos;

    }

    public void GeneratePanel(Vector2Int size)
    {
        foreach (var panel in panelList) Destroy(panel);
        panelList.Clear();

        for(int i = 0; i < size.x; i += 2)
        for(int j = 0; j < size.y; j += 2)
        {
            GameObject panel = Instantiate(Resources.Load<GameObject>("Panel"), new Vector3(0.5f + i, 0.5f + j, 1f), Quaternion.identity);
            panel.transform.parent = transform;
            panel.name = "Panel";
            panel.transform.Rotate(-270, 0, 180);
            panelList.Add(panel);
        }
    }

    public static List<T> GetAddressableAsset<T>(string address)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var group = settings.FindGroup(address);
        List<T> assetList = new List<T>();
        foreach (var entry in group.entries) assetList.Add(Addressables.LoadAssetAsync<T>(entry.address).WaitForCompletion());
        return assetList;
    }
}
#endif