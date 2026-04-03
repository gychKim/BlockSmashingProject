using UnityEngine;

public class Random
{
    /// <summary>
    /// min 부터 max까지 수 중 랜덤한 수를 리턴한다.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Range(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    /// <summary>
    /// min 부터 max까지 수 중 랜덤한 수를 리턴한다.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float Range(float min, float max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    /// <summary>
    /// True혹은 False를 리턴한다.
    /// </summary>
    /// <returns></returns>
    public static bool Bool()
    {
        return Range(0,1) == 1 ? true : false;
    }

    /// <summary>
    /// 받은 확률에 따라 true 혹은 false를 리턴한다.
    /// </summary>
    /// <param name="percentage"></param>
    /// <returns></returns>
    public static bool Gacha(float percentage)
    {
        if (percentage >= 100f)
        {
            return true;
        }
        if(percentage <= 0f)
        {
            return false;
        }
        if(Range(0f, 100f) <= percentage)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 0 ~ 1.0 사이의 랜덤한 값을 가져온다.
    /// </summary>
    public static float Value { get => UnityEngine.Random.value; }
}
