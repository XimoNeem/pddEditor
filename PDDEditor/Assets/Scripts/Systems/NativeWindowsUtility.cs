using UnityEngine;
using System;
using System.IO; // ��� ������������� FileSystemInfo, FileInfo, DirectoryInfo
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
        // ��������� ���� ������ ����� ��� ����� � ��������� ������, ����� �� ����������� �������� ����� Unity
        System.Threading.Thread thread = new System.Threading.Thread(() =>
        {
            FileSystemInfo selectedItem = null;

            if (selectFolder)
            {
                // �������� ���� ������ �����
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
                // �������� ���� ������ �����
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = $"{fileType.ToUpper()} files|*.{fileType}|All files|*.*";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        selectedItem = new FileInfo(openFileDialog.FileName);
                    }
                }
            }

            // ������� �� �������� ����� Unity ��� ������ callback
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