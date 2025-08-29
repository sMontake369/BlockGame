using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyText : MonoBehaviour
{
    public GameObject enemyTextObj;
    public TextMeshProUGUI text;
    public float interval { get; private set; } = 0;

    public async UniTask SetText(string str, int interval = 2000)
    {
        enemyTextObj.gameObject.SetActive(true);
        text.text = str;
        await UniTask.Delay(interval);
        enemyTextObj.gameObject.SetActive(false);
    }
}
