using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 출처 : https://mintchocomacaroon.tistory.com/80
/// </summary>
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HeaderColorAttribute))]
public class HeaderColorAttributeEditor : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        HeaderColorAttribute headerColorAttribute = (HeaderColorAttribute)attribute;

        // 원본 색 캐싱
        Color originalColor = GUI.contentColor;

        // 원하는 색으로 바꾸기
        GUI.contentColor = headerColorAttribute.color;

        // x축 위치를 +5
        position.x += 15;

        // y축 위치를 +5
        position.y += 5;

        // isBold 여부에 따라 굵거나 얇은 글씨체로 라벨 필드 그려주기
        if(headerColorAttribute.isBold)
            EditorGUI.LabelField(position, headerColorAttribute.header, EditorStyles.boldLabel);
        else
            EditorGUI.LabelField(position, headerColorAttribute.header, EditorStyles.label);

        // 다음 필드부터 다시 원본 색을 주기
        GUI.contentColor = originalColor;
    }

    public override float GetHeight()
    {
        return EditorGUIUtility.singleLineHeight + 10f;
    }
}
#endif

public class HeaderColorAttribute : PropertyAttribute
{
    public string header;
    public Color color;
    public bool isBold;
    public HeaderColorAttribute(string header, int red, int green, int blue, bool isBold = false)
    {
        this.header = header;
        color = new Color(red / 255f, green / 255f, blue / 255f);
        this.isBold = isBold;
    }
}
