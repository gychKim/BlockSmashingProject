using UnityEngine;

public class SafeArea : MonoBehaviour
{
    private RectTransform rectTrans;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();

        Vector2 min = Screen.safeArea.min;
        Vector2 max = Screen.safeArea.max;

        min.x /= Screen.width;
        min.y /= Screen.height;

        max.x /= Screen.width;
        max.y /= Screen.height;

        rectTrans.anchorMin = min;
        rectTrans.anchorMax = max;
    }

    //private void Update()
    //{
    //    //DebugX.Log($"현재 Width : {Screen.width}");
    //    //DebugX.Log($"현재 Height : {Screen.height}");

    //    //DebugX.Log($"현재  : {rectTrans.sizeDelta}");
    //}
}
