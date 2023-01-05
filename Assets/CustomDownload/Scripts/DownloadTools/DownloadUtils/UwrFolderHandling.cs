using System.IO;
using UnityEngine;

namespace CustomUwrDownload
{
    /// <summary>
    /// �ļ��д���
    /// </summary>
    internal class UwrFolderHandling
    {
        #region ����ר��Ŀ¼
        /// <summary>
        /// �����������ļ��У��������ļ��У����ػ���Ŀ¼��
        /// </summary>
        internal static void CreatDownloadAssetsFolder(string porjectName)
        {
            string path = Application.persistentDataPath + $"/{porjectName}";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            if (!Directory.Exists(path + "/video")) Directory.CreateDirectory(path + "/video");

            if (!Directory.Exists(path + "/audio")) Directory.CreateDirectory(path + "/audio");
        }
        #endregion

        #region ���������Դ
        /// <summary>
        /// ɾ���������ļ����µ��������ļ���
        /// </summary>
        /// <param name="_porjectName">��Ҫɾ�����ļ��еģ��ļ�������</param>
        internal static void DeleteDownloadAssets(string _porjectName)
        {
            string path = Application.persistentDataPath + $"/{_porjectName}";
            for (int i = Directory.GetDirectories(path).Length - 1; i >= 0; i--)
            {
                Directory.Delete(Directory.GetDirectories(path)[i], true);
            }
        }
        #endregion
    }
}