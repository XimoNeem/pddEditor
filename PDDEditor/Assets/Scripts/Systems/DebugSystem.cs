using System.IO;
using UnityEngine;

public class DebugSystem
{
    private readonly string _logFilePath;

    public DebugSystem()
    {
        _logFilePath = Path.Combine(Application.persistentDataPath, "PDDlogs.txt");
    }

    private void WriteToFile(string message)
    {
        try
        {
            // Добавление сообщения в файл
            File.AppendAllText(_logFilePath, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + message + "\n");
        }
        catch (IOException ex)
        {
            Debug.LogError($"Failed to write to log file: {ex.Message}");
        }
    }

    public void Log(string message)
    {
        string formattedMessage = $"[Log]: {message}";
        WriteToFile(formattedMessage);

        if (Context.Instance.UIDrawer != null)
        {
            DebugLayer_Window logger = Context.Instance.UIDrawer.GetLoadedWindow<DebugLayer_Window>();
            if (logger != null) { logger.LogMessage(message, Color.green); }
        }
        Debug.Log($"Logger : {message}");
    }

    public void LogError(string message)
    {
        string formattedMessage = $"[Error]: {message}";
        WriteToFile(formattedMessage);

        if (Context.Instance.UIDrawer != null)
        {
            DebugLayer_Window logger = Context.Instance.UIDrawer.GetLoadedWindow<DebugLayer_Window>();
            if (logger != null) { logger.LogMessage(message, Color.red); }
        }
        Debug.LogError($"Logger : {message}");
    }

    public void LogWarning(string message)
    {
        string formattedMessage = $"[Warning]: {message}";
        WriteToFile(formattedMessage);

        if (Context.Instance.UIDrawer != null)
        {
            DebugLayer_Window logger = Context.Instance.UIDrawer.GetLoadedWindow<DebugLayer_Window>();
            if (logger != null) { logger.LogMessage(message, Color.yellow); }
        }
        Debug.LogWarning($"Logger : {message}");
    }
}
