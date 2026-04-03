using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LabelColorAttribute))]
public class LabelColorAttributeEditor : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        LabelColorAttribute labelColorAttribute = (LabelColorAttribute)attribute;

        // 원본 색 캐싱
        Color originalColor = GUI.contentColor;

        // 원하는 색으로 바꾸기
        GUI.contentColor = labelColorAttribute.color;

        // Insepctor 너비 기반으로 라벨 문자열 생성
        string centeredLabel = GetCenteredLabel(labelColorAttribute.header, position.width);

        // y축 위치를 +5
        position.y += 5;

        // 글씨체 굵게
        EditorGUI.LabelField(position, centeredLabel, EditorStyles.boldLabel);

        // 다음 필드부터 다시 원본 색을 주기
        GUI.contentColor = originalColor;
    }

    public override float GetHeight()
    {
        return EditorGUIUtility.singleLineHeight + 20f;
    }

    /// <summary>
    /// Insepctor 너비 기반으로 라벨 문자열 생성
    /// </summary>
    /// <param name="text"></param>
    /// <param name="totalPixelWidth"></param>
    /// <param name="averageCharWidth"></param>
    /// <returns></returns>
    private string GetCenteredLabel(string text, float totalPixelWidth, float averageCharWidth = 5f)
    {
        int totalCharCount = Mathf.FloorToInt(totalPixelWidth / averageCharWidth);
        int textLength = text.Length;
        int dashCount = Mathf.Max(0, totalCharCount - textLength); // -2 for '|', '|'

        int leftDashes = dashCount / 2;
        int rightDashes = dashCount - leftDashes;

        return $"{new string('-', leftDashes)}{text}{new string('-', rightDashes)}";
    }
}
#endif

public class LabelColorAttribute : PropertyAttribute
{
    public string header;
    public Color color;
    public bool isBold;
    public LabelColorAttribute(string header, int red, int green, int blue)
    {
        this.header = header;
        color = new Color(red / 255f, green / 255f, blue / 255f);
    }
}
