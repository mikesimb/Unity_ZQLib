using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ZQLib
{
    public class CZQTcpClient : IDisposable
    {
        static public readonly int MAX_TCP_CLIENT_BUFF_SIZE = (64 * 1024);

        public CZQTcpClient()
        {
            
        }

       ~CZQTcpClient()
        {

        }


        public void Release()
        {
            Dispose();
        }

        

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool Disposing)
        {
            if (!m_isDispose)
            {
                if(m_Thread != null)
                {
                    LocalThreadState = ThreadState.AbortRequested;
                    m_Thread.Join();
                }
                if(m_waiting != null)
                {
                    m_waiting.Set();
                }

                if(Disposing)
                {
                    CloseSocket();
                    m_WaitSendSize = 0;
                    m_HashReadSize = 0;
                    m_waiting = null;
                    m_Thread = null;
                    m_Mutex = null;
                    m_SendBuffer = null;
                    m_ReadBuffer = null;
                    //m_QueueReq = null;
                }
                m_isDispose = true;
            }
        }


        public bool Connect(string HostIP,int HostPort ,int mTimeOut = -1)
        {
            if((m_statue == ZQNetLib.ZQTcpClientStatue.ZQ_STATUE_CONNECTING)||
               (m_statue == ZQNetLib.ZQTcpClientStatue.ZQ_STATUE_CONNECTED))
            {
                return false;
            }

            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (m_socket == null)
                return false;

            m_socket.NoDelay = true;
            m_socket.SendTimeout = 0;
            m_socket.ReceiveTimeout = 0;

            bool non = mTimeOut >= 0;

            if(non)
            {
                m_waiting.Reset();
                AsyncCallback callback = new AsyncCallback(OnConnectCallback);
                try
                {
                    m_socket.BeginConnect(HostIP, HostPort, OnConnectCallback, m_socket);

                }catch(Exception e)
                {
                    //ProcessException(e,ZQLib.ZQTcpClientStatue.)
                    return false;
                }
            }
            return true;
        }


        private void OnConnectCallback(IAsyncResult  result )
        {
            try
            {
                Socket socket = (Socket)result.AsyncState;
                if(socket != null)
                {
                    if(socket.Connected && socket.Poll(0,SelectMode.SelectWrite))
                    {
                        socket.EndConnect(result);

                    }
                }
            }finally
            {
                if (m_waiting != null)
                    m_waiting.Set();
            }
        }


        //下边是pirvate的变量的定义部分
        private object m_Mutex = new object();
        private Socket m_socket = null;
        private ZQNetLib.ZQTcpClientStatue m_statue = ZQNetLib.ZQTcpClientStatue.ZQ_STATUE_NONE;
        private int m_WaitSendSize = 0;
        private int m_HashReadSize = 0;
        private ManualResetEvent m_waiting = new ManualResetEvent(false);
        //这个线程是用来做连接和发送数据用的  难道异步操作会有什么问题吗？

        private Thread m_Thread = null;
        private ThreadState m_ThreadState = ThreadState.Unstarted;
        private byte[] m_SendBuffer = new byte[MAX_TCP_CLIENT_BUFF_SIZE];
        private byte[] m_ReadBuffer = new byte[MAX_TCP_CLIENT_BUFF_SIZE];
        private bool m_isDispose = false;

    }
}