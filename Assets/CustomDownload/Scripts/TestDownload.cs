using System.Collections.Generic;
using UnityEngine;
using XHTools;

public class TestDownload : MonoBehaviour
{
    string UrlHead = @"http://192.168.29.11:8080/xxmusic/L0/"; //��Դ���صĵ�ַͷ

    void Start()
    {
        D.Log("\n \n \n \n \n���ش洢·��Ϊ��" + Application.persistentDataPath);
        Application.targetFrameRate = 60;
        Download = new DownloadTools(UrlHead);
    }

    private void Update()
    {
        //if (Input.GetKeyDown( KeyCode.Space))
        {
            //D.Error($"��СΪ��{Download.CurrentDownloadProgress / 1024 / 1024f} mb");
        }
    } 

    List<string> AssetsName = new List<string>()
    {
        //"00001.mp4",
        //"00002.mp4",
        //"00003.mp4",
        //"00004.mp4",
        //"00005.mp4",
        //"00006.mp4",
        //"00007.mp4",
        //"00008.mp4",
        //"00009.mp4",
        //"00010.mp4",
        "0010101.mp4",
        //"00011.mp4",
        //"00012.mp4",
        //"00013.mp4",
        //"00014.mp4",
        //"00015.mp4",
        //"00016.mp4",
        //"00017.mp4",
        //"00018.mp4",
        //"00019.mp4",
        //"0010201.mp4",
        //"00020.mp4",
        //"00021.mp4",
        //"00022.mp4",
        //"00023.mp4",
        //"00024.mp4",
        //"00025.mp4",
        //"00026.mp4",
        //"00027.mp4",
        //"00028.mp4",
        //"00029.mp4",
        //"00030.mp4",
        //"00031.mp4",
        //"00032.mp4",
        //"0010301.mp4",
        //"0010303.mp4",
        //"0010401.mp4",
        //"0010403.mp4",
        //"0010405.mp4",
        //"0010501.mp4",
        //"0010503.mp4",
        //"0020101.mp4",
        //"0020103.mp4",
        //"0020201.mp4",
        //"0020203.mp4",
        //"0020301.mp4",
        //"0020401.mp4",
        //"0020403.mp4",
        //"0020405.mp4",
        //"0020501.mp4",
        //"0020503.mp4",
        //"0030101.mp4",
        //"0030201.mp4",
        //"0030301.mp4",
        //"0030303.mp4",
        //"0030401.mp4",
        //"0030403.mp4",
        //"0030405.mp4",
        //"0070101.mp4",
        //"0070201.mp4",
        //"0070301.mp4",
        //"0070303.mp4",
        //"0070401.mp4",
        //"0070403.mp4",
        //"0070405.mp4",
        //"100001.mp4",
        //"100101.mp4",
        //"100201.mp4",
        //"100301.mp4",
    };

    DownloadTools Download;
    public void StartDownload()
    {
        Download.DownloadAll(AssetsName, ()=>
        {
            D.Log("�����������");
        });

    }

    public void LookoverLoadSize()
    {
        Download.GetDownloadSizeAll(AssetsName, (size)=> 
        {
            D.Log($"����Ҫ���ش�СΪ��{size / 1024f / 1024f}");
        });
    }

    public void LookoverIndex()
    {
        D.Log($"��ǰ��������: {Download.CurrentIndexProgress}");
    }

    int manNumber = 2;
    public void MaxDownload()
    { 
        Download.DownloadConcurrently(AssetsName, manNumber, () =>
        {
            D.Log("������������أ�ȫ���������");
        });
    }
     
    public void CreateAsset()
    {
        Download.CreatAssetFolder("xxyy");
    }

    public void DeleteAsset()
    {
        Download.DeleteAssetFolder("xxyy");
    }
}
