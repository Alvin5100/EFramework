/* 
 * ================================================
 * Describe:      This script is used to control the websocket managers.     Thanks to the author: psygames, can join his QQ group (1126457634) get the latest version.
 * Author:        psygames
 * CreationTime:  2016-06-25 00:00:00
 * ModifyAuthor:  Xiaohei.Wang(Wenhao)
 * ModifyTime:    2024-02-07 17:21:26
 * Version:       0.1 
 * ===============================================
 */
namespace EasyFramework.UnityWebSocket
{
    /// <summary>
    /// Reference html5 WebSocket ReadyState Properties
    /// Indicates the state of a WebSocket connection.
    /// </summary>
    /// <remarks>
    /// The values of this enumeration are defined in
    /// <see href="http://www.w3.org/TR/websockets/#dom-websocket-readystate">
    /// The WebSocket API</see>.
    /// </remarks>
    public enum WebSocketState : ushort
    {
        /// <summary>
        /// Indicates that the connection has not yet been established.
        /// <para>������δ����</para>
        /// </summary>
        Connecting = 0,
        /// <summary>
        /// Indicates that the connection has been established, and the communication is possible.
        /// <para>�����Ѿ����������Խ���ͨ��</para>
        /// </summary>
        Open = 1,
        /// <summary>
        /// Indicates that the connection is going through the closing handshake, or the close method has been invoked.
        /// <para>�������ڽ��йر����֣������ѵ���close����</para>
        /// </summary>
        Closing = 2,
        /// <summary>
        /// Indicates that the connection has been closed or could not be established.
        /// <para>�����ѹرջ��޷�����</para>
        /// </summary>
        Closed = 3,
        /// <summary>
        /// Indicates that the connection does not exist or has been released.
        /// <para>���Ӳ����ڻ����Ѿ��ͷ�</para>
        /// </summary>
        Inexistence = 4,
    }
}
