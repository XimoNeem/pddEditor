using System.Data.Common;
using UnityEngine;

public class DebugSystem
{
    public DebugSystem()
    {
        
    }

    public void Log(string message)
    {
        if (Context.Instance.UIDrawer != null)
        { 
            DebugLayer_Window logger = Context.Instance.UIDrawer.GetLoadedWindow<DebugLayer_Window>();
            if (logger != null) { logger.LogMessage(message, Color.green); }
        }
        Debug.Log($"Logger : {message}");
    }

    public void LogError(string message)
    {
        if (Context.Instance.UIDrawer != null)
        {
            DebugLayer_Window logger = Context.Instance.UIDrawer.GetLoadedWindow<DebugLayer_Window>();
            if (logger != null) { logger.LogMessage(message, Color.red); }
        }
        Debug.Log($"Logger : {message}");
    }

    public void LogWarning(string message)
    {
        if (Context.Instance.UIDrawer != null)
        {
            DebugLayer_Window logger = Context.Instance.UIDrawer.GetLoadedWindow<DebugLayer_Window>();
            if (logger != null) { logger.LogMessage(message, Color.yellow); }
        }
        Debug.Log($"Logger : {message}");
    }
}