using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;
using static CloudPrinter.TCPServer.TcpPrinter;

namespace CloudPrinter.TCPServer
{
    public class tcpServer
    {
        TcpListener tcplist;
        TcpClient newClient;
        public tcpServer()
        {
            if (tcplist != null)
            {
                return;
            }
            tcplist = new TcpListener(IPAddress.Parse("192.168.136.1"), 5678);
            tcplist.Start();
            new Thread(o =>
            {
                while (true)
                {
                    try
                    {
                        //停在此处监听连接，如果有新客户端连接就运行下去
                        newClient=tcplist.AcceptTcpClient();
                        if (newClient != null)
                        {
                            new TcpPrinter(newClient);
                        }
                    }
                    catch(Exception ex)
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