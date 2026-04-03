
public class DebugX
{
    public static void RedLog(string message)
    {
        UnityEngine.Debug.Log($"<color=red>{message}</color>");
    }
    public static void BlueLog(string message)
    {
        UnityEngine.Debug.Log($"<color=blue>{message}</color>");
    }
    public static void GreenLog(string message)
    {
        UnityEngine.Debug.Log($"<color=00FF00>{message}</color>");
    }
    public static void YellowLog(string message)
    {
        UnityEngine.Debug.Log($"<color=#FFFF00>{message}</color>");
    }
    public static void OrangeLog(string message)
    {
        UnityEngine.Debug.Log($"<color=#FFA500>{message}</color>");
    }
    public static void SkyBlueLog(string message)
    {
        UnityEngine.Debug.Log($"<color=#87CEFA>{message}</color>");
    }
    public static void CyanLog(string message)
    {
        UnityEngine.Debug.Log($"<color=#00FFFF>{message}</color>");
    }
    public static void PinkLog(string message)
    {
        UnityEngine.Debug.Log($"<color=#FFC0CB>{message}</color>");
    }
    public static void GoldLog(string message)
    {
        UnityEngine.Debug.Log($"<color=#FFD700>{message}</color>");
    }
    public static void MagentaLog(string message)
    {
        UnityEngine.Debug.Log($"<color=#FF00FF>{message}</color>");
    }
    public static void PurpleLog(string message)
    {
        UnityEngine.Debug.Log($"<color=#DA70D6>{message}</color>");
    }
    public static void Log(string message)
    {
        UnityEngine.Debug.Log(message);
    }
    public static void LogWarning(string message)
    {
        UnityEngine.Debug.LogWarning(message);
    }
    public static void LogError(string message)
    {
        UnityEngine.Debug.LogError(message);
    }
}
