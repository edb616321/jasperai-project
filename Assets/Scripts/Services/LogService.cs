using UnityEngine;

public class LogService : ILogService
{
    public void Log(string message)
    {
        Debug.Log($"[LogService] {message}");
    }
    
    public void LogWarning(string message)
    {
        Debug.LogWarning($"[LogService] {message}");
    }
    
    public void LogError(string message)
    {
        Debug.LogError($"[LogService] {message}");
    }
}
