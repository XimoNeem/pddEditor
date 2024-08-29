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

    /// <summary>
    /// returned True = just created
    /// </summary>
    /// <param name="path"></param>
    public static bool CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
                Debug.Log($"Created {path}");
                return true;
            }
            catch (Exception ex)
            {
                Context.Instance.Logger.LogError("Error creating directory: " + ex.Message);
                return false;
            }
        }
        return false;
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
        EnsureDirectoryExists(path);
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

    public static bool CopyFile(string sourcePath, string destinationPath)
    {
        EnsureDirectoryExists(destinationPath);

        try
        {
            File.Copy(sourcePath, destinationPath, true); // true to overwrite if it already exists
            Debug.Log($"File copied from {sourcePath} to {destinationPath}");
            return true;
        }
        catch (Exception ex)
        {
            Context.Instance.Logger.LogError("Error copying file: " + ex.Message);
            return false;
        }
    }

    public static bool DeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Context.Instance.Logger.Log($"Deleted file at {path}");
                return true;
            }
            else
            {
                Context.Instance.Logger.LogWarning("File does not exist: " + path);
                return false;
            }
        }
        catch (Exception ex)
        {
            Context.Instance.Logger.LogError("Error deleting file: " + ex.Message);
            return false;
        }
    }
}