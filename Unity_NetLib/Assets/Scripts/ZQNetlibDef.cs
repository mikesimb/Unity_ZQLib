using System.Collections;


namespace  ZQNetLib
{
   public enum ZQTcpClientStatue
    {
        ZQ_STATUE_NONE  = 0,
        ZQ_STATUE_CONNECTING,
        ZQ_STATUE_CONNECTED,
        ZQ_STATUE_CONNECT_FAIL,
        ZQ_STATUE_ABORT,
        ZQ_STATUE_DISCONNECT,
    };
}
