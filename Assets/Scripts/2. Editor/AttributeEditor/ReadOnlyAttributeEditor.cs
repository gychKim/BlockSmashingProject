using System;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 출처 : https://velog.io/@ymsection/Unity-읽기전용-인스펙터-프로퍼티
/// </summary>
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute), true)]
public class ReadOnlyAttributeEditor : PropertyDrawer
{
    // 적용되는 프로퍼티가 필요한 정확한 높이를 계산한다.
    // 리스트 같은 프로퍼티인 경우, 부숴질 수 있기 때문에, includeChildren를 true로 설정하면 전체가 보이게 되므로, 충분한 높이를 확보할 수 있다.
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    // OnGUI를 그린다.
    // 기본적으로는 비활성화된다.
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = !Application.isPlaying && ((ReadOnlyAttribute)attribute).runTimeOnly; // runTimeOnly가 true라면, 필드값은 게임 진행중이 아니라면 활성화된다.
        EditorGUI.PropertyField(position, property, label, true); // 프로퍼티를 그린다
        GUI.enabled = true; // GUI를 다시 활성화 시킨다. > 이건 전역 상태를 조절하는 변수라, true로 하지 않으면, 이 후에 그려지는 모든 GUI가 그려지지 않을 수 도 있기에 true로 설정한다.

        // Insepctor를 위에서부터 그리는데, ReadOnly인 Field를 그리기 시작하면 잠시 GUI를 끈다음에 Draw하고(이러면 비활성화가 된다), 다시 GUI.enable = true로 해준다.
    }
}
#endif

[AttributeUsage(AttributeTargets.Field)] // 필드값 한정 프로퍼티
public class ReadOnlyAttribute : PropertyAttribute
{
    public readonly bool runTimeOnly;
    public ReadOnlyAttribute(bool runTimeOnly = false)
    {
        this.runTimeOnly = runTimeOnly;
    }
}
