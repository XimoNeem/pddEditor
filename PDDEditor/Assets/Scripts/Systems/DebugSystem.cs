using System.Data.Common;
using UnityEngine;

public class DebugSystem
{
    public DebugSystem()
    {
        
    }

    public void Log(string message)
    {
        DebugLayer_Window logger = Context.Instance.UIDrawer.GetLoadedWindow<DebugLayer_Window>();
        Debug.Log($"Logger : {message}");
        if (logger != null) { logger.LogMessage(message, Color.green); }
    }

    public void LogError(string message)
    {
        DebugLayer_Window logger = Context.Instance.UIDrawer.GetLoadedWindow<DebugLayer_Window>();
        Debug.Log($"Logger : {message}");
        if (logger != null) { logger.LogMessage(message, Color.red); }
    }

    public void LogWarning(string message)
    {
        DebugLayer_Window logger = Context.Instance.UIDrawer.GetLoadedWindow<DebugLayer_Window>();
        Debug.Log($"Logger : {message}");
        if (logger != null) { logger.LogMessage(message, Color.yellow); }
    }
}