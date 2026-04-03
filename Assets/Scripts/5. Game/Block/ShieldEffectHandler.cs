using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldEffectHandler : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> crackSpriteList; // 파괴 스프라이트 리스트

    [SerializeField]
    private GameObject shieldUI;
    [SerializeField]
    private Image crackImage; // 파괴 이미지

    private Guid applyShieldEventKey, hitShieldEventKey;

    private void Start()
    {
        applyShieldEventKey = EventManager.Instance.Subscribe(EffectType.Shield, ApplyShield);
        hitShieldEventKey = EventManager.Instance.Subscribe<MainGameEventType, int>(MainGameEventType.HitShield, HitShield);

        shieldUI.SetActive(false);
    }

    /// <summary>
    /// 실드 적용
    /// </summary>
    private void ApplyShield()
    {
        if(!gameObject.activeSelf)
            shieldUI.SetActive(true);

        crackImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 실드로 점수감소 막았을 시
    /// </summary>
    /// <param name="count">남은 실드 개수</param>
    private void HitShield(int count)
    {
        if(count <= 0)
        {
            crackImage.sprite = null;
            shieldUI.SetActive(false);
            crackImage.gameObject.SetActive(false);
            return;
        }

        if(!crackImage.gameObject.activeSelf)
            crackImage.gameObject.SetActive(true);

        // IndexOutOfRangeException 배열의 유효 범위를 벗어난 경우
        // ArgumentOutOfRangeException 리스트, string 등에서 유효 범위르 벗어난 경우
        try
        {
            crackImage.sprite = crackSpriteList[count - 1];
        }
        catch(ArgumentOutOfRangeException e) 
        {
            crackImage.sprite = null;
            crackImage.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe(EffectType.Shield, applyShieldEventKey);
        EventManager.Instance.Unsubscribe(MainGameEventType.HitShield, hitShieldEventKey);
    }
}
