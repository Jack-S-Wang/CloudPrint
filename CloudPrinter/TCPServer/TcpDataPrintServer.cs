using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;

namespace CloudPrinter.TCPServer
{
    public class TcpDataPrintServer
    {
        /// <summary>
        /// 创建一个数据通道
        /// </summary>

        static TcpListener tcplist;
        TcpClient newClient;
        public TcpDataPrintServer(string number)
        {
            if (tcplist != null)
            {
                return;
            }
            tcplist = new TcpListener(IPAddress.Parse("192.168.11.130"), 5679);
            tcplist.Start();
            new Thread(o =>
            {
                while (true)
                {
                    try
                    {
                            //停在此处监听连接，如果有新客户端连接就运行下去
                            newClient = tcplist.AcceptTcpClient();
                        if (newClient != null)
                        {
                            new TcpDataPrint(newClient,number);
                        }
                    }
                    catch (Exception ex)
                    {

                        var str = string.Format("异常信息{0}；追踪信息来源：{1}", ex, ex.StackTrace);
                        tcplist.Stop();
                        break;
                    }
                }
            }).Start();
        }
    }
}