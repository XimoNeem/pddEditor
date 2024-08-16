using System.Collections;
using UnityEngine;
using System.IO;
using System;

public static class PDDUtilities 
{
    public static void EnsureDirectoryExists(string path)
    {
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (Exception ex)
            {
                Context.Instance.Logger.LogError("Error creating directory: " + ex.Message);
            }
        }
    }

    public static void WriteTextToFile(string path, string text)
    {
        EnsureDirectoryExists(path);

        try
        {
            File.WriteAllText(path, text);
        }
        catch (Exception ex)
        {
            Context.Instance.Logger.LogError("Error writing to file: " + ex.Message);
        }
    }

    public static bool TryReadFromFile(string path, out string content)
    {
        content = null;
        try
        {
            if (File.Exists(path))
            {
                content = File.ReadAllText(path);
                return true;
            }
            else
            {
                Context.Instance.Logger.LogWarning("File does not exist: " + path);
                return false;
            }
        }
        catch (System.Exception ex)
        {
            Context.Instance.Logger.LogError("Error reading from file: " + ex.Message);
            return false;
        }
    }
}