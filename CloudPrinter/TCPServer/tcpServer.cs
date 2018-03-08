using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;

namespace CloudPrinter.TCPServer
{
    public class tcpServer
    {
        TcpListener tcplist;
        TcpClient newClient;
        public tcpServer()
        {
            tcplist = new TcpListener(IPAddress.Parse("192.168.11.105"), 5678);
            tcplist.Start();
            new Thread(o =>
            {
                while (true)
                {
                    try
                    {
                        //停在此处监听连接，如果有新客户端连接就运行下去
                        tcplist.BeginAcceptTcpClient(connenctCallback, this);
                    }
                    catch
                    {
                        return;
                    }
                }
            }).Start();
        }
        private void connenctCallback(IAsyncResult ar)
        {
            newClient=tcplist.EndAcceptTcpClient(ar);
        }
    }
}