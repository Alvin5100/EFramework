using System;
using System.Collections.Generic;
using UnityEngine;
using UnityWebSocket;

public class HttpTest : MonoBehaviour
{
    int m_SocketIndex = -1;
    Dictionary<int, WebSocket> m_SocketDic;

    void Start()
    {
        m_SocketDic = new Dictionary<int, WebSocket>();
    }

    void OnDestroy()
    {
        m_SocketDic.Clear();
        m_SocketDic = null;
    }

    /// <summary>
    /// ����һ������
    /// </summary>
    /// <param name="address">Զ�˵�ַ</param>
    /// <param name="subProtocol"></param>
    /// <param name="onOpen"></param>
    /// <param name="onClose"></param>
    /// <param name="onError"></param>
    /// <param name="onMessage"></param>
    /// <returns>���ص�ǰ����ID��������������ʱ�贫��</returns>
    public int CreateOne(string address, string subProtocol = null,
        EventHandler<OpenEventArgs> onOpen = null,
        EventHandler<CloseEventArgs> onClose = null,
        EventHandler<ErrorEventArgs> onError = null,
        EventHandler<MessageEventArgs> onMessage = null
        )
    {
        WebSocket _ws;
        if (string.IsNullOrEmpty(subProtocol))
            _ws = new WebSocket(address);
        else
            _ws = new WebSocket(address, subProtocol);
        _ws.OnOpen += onOpen;
        _ws.OnClose += onClose;
        _ws.OnError += onError;
        _ws.OnMessage += onMessage;
        m_SocketDic[++m_SocketIndex] = _ws;
        return m_SocketIndex;
    }

    /// <summary>
    /// ����
    /// </summary>
    public void Connect(int wsId = 0)
    {
        m_SocketDic[wsId].ConnectAsync();
    }

    /// <summary>
    /// �Ͽ�����
    /// </summary>
    public void Disconnect(int wsId = 0)
    {
        m_SocketDic[wsId].CloseAsync();
    }

    /// <summary>
    /// �ͷ�����
    /// </summary>
    public void Dispose(int wsId = 0)
    {
        m_SocketDic.Remove(wsId);
    }

    /// <summary>
    /// ��ȡ���ӵ�ַ
    /// </summary>
    public string GetAddress(int wsId = 0) => m_SocketDic[wsId].Address;

    /// <summary>
    /// ��ȡ��Э���ַ
    /// </summary>
    public string[] GetSubProtocols(int wsId = 0) => m_SocketDic[wsId].SubProtocols;

    /// <summary>
    /// ��ȡ��ǰ״̬
    /// </summary>
    public WebSocketState GetReadyState(int wsId = 0) => m_SocketDic[wsId].ReadyState;

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="msg">��Ϣ����</param>
    /// <param name="wsId">����ID</param>
    public void Send(string msg, int wsId = 0)
    {
        m_SocketDic[wsId].SendAsync(msg);
    }
    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="msg">��Ϣ����</param>
    /// <param name="wsId">����ID</param>
    public void Send(byte[] msg, int wsId = 0)
    {
        m_SocketDic[wsId].SendAsync(msg);
    }
}
