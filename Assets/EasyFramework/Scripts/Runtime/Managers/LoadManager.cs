/* 
 * ================================================
 * Describe:      This script is used to loaded assets in unity.
 * Author:        Xiaohei.Wang(Wenhao)
 * CreationTime:  2022-06-08 14:43:47
 * ModifyAuthor:  Xiaohei.Wang(Wenhao)
 * ModifyTime:    2022-06-08 14:43:47
 * Version:       0.1 
 * ===============================================
 */
using UnityEngine;

namespace EasyFramework.Managers
{
    /// <summary>
    /// Loaded assets.
    /// </summary>
    public class LoadManager : Singleton<LoadManager>, IManager
    {
        int IManager.ManagerLevel => EF.Projects.AppConst.ManagerLevels.IndexOf("LoadManager");

        void ISingleton.Init()
        {

        }

        void ISingleton.Quit()
        {

        }

        /// <summary>
        /// Load the object in resources folder.
        /// ������Դ�ļ����еĶ���
        /// </summary>
        /// <param name="pathName">The object path in resources folder.�������ļ����е�·��</param>
        /// <returns>Return the object typeof T. ����T���͵Ķ���</returns>
        public T LoadInResources<T>(string pathName) where T : Object
        {
            return Resources.Load<T>(pathName);
        }

        public T Download<T>() where T : Object
        {
            return Resources.Load<T>("");
        }
    }
}
