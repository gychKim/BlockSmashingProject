using UnityEngine;

public class ContentSizeChanger : MonoBehaviour
{
    private Canvas canvas;
    private RectTransform rectTrans;
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTrans = GetComponent<RectTransform>();
    }
    void Start()
    {
        rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, canvas.GetComponent<RectTransform>().sizeDelta.y);
    }
}
