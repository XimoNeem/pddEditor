using UnityEngine;
using System;
using System.IO; // Для использования FileSystemInfo, FileInfo, DirectoryInfo
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using System.Windows.Forms;
#endif

public class NativeWindowsUtility : MonoBehaviour
{
    public static void ShowFileBrowser(string fileType, bool selectFolder, Action<FileSystemInfo> callback)
    {
        Context.Instance.Logger.LogWarning(fileType);
        Context.Instance.Logger.LogWarning(selectFolder.ToString());
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        // Запускаем окно выбора файла или папки в отдельном потоке, чтобы не блокировать основной поток Unity
        System.Threading.Thread thread = new System.Threading.Thread(() =>
        {
            FileSystemInfo selectedItem = null;

            if (selectFolder)
            {
                // Открытие окна выбора папки
                using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
                {
                    if (folderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        selectedItem = new DirectoryInfo(folderBrowser.SelectedPath);
                    }
                }
            }
            else
            {
                // Открытие окна выбора файла
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = $"{fileType.ToUpper()} files|*.{fileType}|All files|*.*";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        selectedItem = new FileInfo(openFileDialog.FileName);
                    }
                }
            }

            // Возврат на основной поток Unity для вызова callback
            if (selectedItem != null)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => callback?.Invoke(selectedItem));
            }
        });

        thread.SetApartmentState(System.Threading.ApartmentState.STA);
        thread.Start();
#else
        Debug.LogError("File browser is only available on Windows Standalone build.");
#endif
    }
}