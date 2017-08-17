using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace ZQLib
{
    class ZQTcpClient : IDisposable
    {
        static public readonly int MAX_TCP_CLIENT_BUFF_SIZE = (8 * 1024);
        public ZQTcpClient()
        {
            m_WorkThread = new Thread(ThreadProcess);
            m_WorkThread.Start();

        }

        ~ZQTcpClient()
        {
            Dispose();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Release()
        {
            Dispose();
        }

        protected void Dispose(bool Disposing)
        {
            m_Disposed = true;
        }


        public bool Initialize()
        {

            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (m_socket == null)
            {
                return false;
            }

            m_socket.NoDelay = true;
            m_socket.SendTimeout = 0;
            m_socket.ReceiveTimeout = 0;



            return true;
        }

        public void ConnectToServer(string HostIP, int HostPort, int iTimeOut)
        {
            try
            {
                if (m_socket != null)
                {
                    IAsyncResult connResult = m_socket.BeginConnect(HostIP, HostPort, OnConnectCallback, m_socket);
                    //这里需要设定异步等待时间是多少
                    connResult.AsyncWaitHandle.WaitOne(1000, true);
                    if (connResult.IsCompleted)
                    {
                        //说明连接成功
                        m_HostIP = HostIP;
                        m_HostPort = HostPort;

                        this.m_IsConnected = true;

                        Debug.WriteLine("连接成功");
                    }
                    else
                    {
                        //处理连接不成功的一场
                        this.m_IsConnected = false;
                        Debug.WriteLine("连接不成功");
                    }
                }
            } catch (Exception e)
            {
                ProcessExecption(e);
            }
        }

        public void OnConnectCallback(IAsyncResult ar)
        {
            m_IsConnected = true;
            Debug.WriteLine("OnConnectCallBack");
            DoSendData();
        }

        public void ProcessExecption(Exception e)
        {
            this.m_IsConnected = false;
            Debug.WriteLine("ConnToServer have Exception ! " + e.Message);


        }

        public void DoReadData()
        {
            //可以读取的数据长度
            int ReadSize = m_ReviceBuffer.Length - m_RevicedLength;


            //如果是连接状态那么可以开始接收数据了
            if (m_IsConnected)
            {
               IAsyncResult ar =  m_socket.BeginReceive(m_ReviceBuffer,m_RevicedLength,ReadSize, SocketFlags.None, OnReviceCallBack, m_socket);
                //这里也需要设定一个超时等待时间 这样会阻塞线程一秒钟所以最好不要这样做。、、
                //ar.AsyncWaitHandle.WaitOne(1000, true);
                //if (ar.IsCompleted)
                //{
                    //数据接收完毕
                //}
                //else
                //{
                    //数据接收出现问题
                //}
            }
        }

        private void OnReviceCallBack(IAsyncResult ar)
        {
            //接下来这里是处理接收到的数据的函数

        }


        public byte[] StructToBytes(object obj)
        {
            // 得到结构体的大小
            int size = Marshal.SizeOf(obj);
            // 创建byte数组
            byte[] bytes = new byte[size];
            // 分配结构体大小的内存空间

            IntPtr structPtr = Marshal.AllocHGlobal(size);
            // 将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(obj, structPtr, false);
            // 从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            // 释放内存空间
            Marshal.FreeHGlobal(structPtr);
            // 返回byte数组
            return bytes;

        }


        public void DoSendData()
        {
           //发送数据是发送一个长度的

            //如果是连接状态那么可以开始接收数据了
            if (m_IsConnected)
            {
                ClientMessage cm = new ClientMessage();
                cm.Sign = 0xFFFFFFF;
                cm.Cmd = 1;
                cm.DataLength = 10;
                cm.ReservByte = 0;
                cm.DataLength = 1000;

                m_SendBuffer = StructToBytes(cm);
                m_SendingLength = Marshal.SizeOf(cm);

                IAsyncResult ar = m_socket.BeginSend(m_SendBuffer, 0, m_SendingLength, SocketFlags.None, OnSendCallBack, m_socket);
                //这里也需要设定一个超时等待时间
                //ar.AsyncWaitHandle.WaitOne(1000, true);
                //if (ar.IsCompleted)
                //{
                    //数据接收完毕
                //}
                //else
                //{
                    //数据接收出现问题
                //}
            }
        }

        private void OnSendCallBack(IAsyncResult ar)
        {
            //这里是处理发送数据的回调
            //DoSendData();
            Debug.WriteLine("OnSendCallback");
        }

        private void ThreadProcess()
        {

        }

        private Socket m_socket = null;
        private string m_HostIP { get; set; } = "";
        private int m_HostPort { get; set; } = 0;
        private bool m_IsConnected { get; set; } = false;

        private bool m_Disposed = false;

        private Thread m_WorkThread = null;//这个线程主要负责接收数据和发送数据


        private int m_RevicedLength = 0;//这个是指m_ReviceBuffer里的数据长度
        private int m_SendingLength = 0;

        private byte[] m_SendBuffer = new byte[MAX_TCP_CLIENT_BUFF_SIZE];
        private byte[] m_ReviceBuffer = new byte[MAX_TCP_CLIENT_BUFF_SIZE];
        IList<byte> BufferList = new List<byte>();


    }
}
