using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace CustomUwrDownload
{
    /// <summary>
    /// �����ļ�����
    /// </summary>
    internal class UwrFileHandling
    {
        #region �����ļ�
        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="path">�����ļ���·��</param>
        /// <param name="bytes">�ļ����ֽ�����</param>
        /// <param name="callback">������ɺ�Ļص�</param>
        /// <param name="md5">��ǰҪ�����ļ���MD5</param>
        internal static void CreateFile(string path, byte[] bytes, Action callback = null, string md5 = default)
        {
            bool exist = false;
            if (File.Exists(path))
            {
                exist = ComparisonMD5(path, bytes, md5);
                if (exist) return;
            }
            File.WriteAllBytes(path, bytes);
            callback?.Invoke();
        }
        #endregion

        #region �ȶ�MD5
        /// <summary>
        /// �ȶ�MD5���ж��Ƿ���Ҫ�滻�ļ�
        /// </summary>
        /// <param name="filePath">�Ѵ��ڵ��ļ�·��</param>
        /// <param name="newDate">��Ҫ�ȶԵ�����������</param>
        /// <param name="md5">���������Աȵ�MD5</param>
        /// <returns>�����ļ�MD5�Ƿ���ͬ</returns>
        internal static bool ComparisonMD5(string filePath, byte[] newDate = null, string md5 = null)
        {
            try
            {                
                FileStream fs = File.OpenRead(filePath);
                int length = (int)fs.Length;
                byte[] old = new byte[length];

                fs.Read(old, 0, length);
                fs.Close();

                MD5 mD5 = new MD5CryptoServiceProvider();
                byte[] oldMd5 = mD5.ComputeHash(old);
                string oldmD5 = "";
                foreach (var item in oldMd5)
                {
                    oldmD5 += Convert.ToString(item, 16);
                }
                if (!string.IsNullOrEmpty(md5)) return oldmD5 == md5;

                byte[] newMd5 = mD5.ComputeHash(newDate);

                return oldMd5 == newMd5;
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }
        #endregion
    }
}