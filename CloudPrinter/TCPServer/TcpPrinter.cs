using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;

namespace CloudPrinter.TCPServer
{
    public class TcpPrinter
    {
        TcpClient client;
        NetworkStream stream;
        int readCount = 0;
        List<byte> recice = new List<byte>();
        byte[] buffer = new byte[1000];
        public TcpPrinter(TcpClient client)
        {
            this.client = client;
            stream = client.GetStream();
            ProcessData();
        }
        private void ProcessData()
        {
            try
            {
                stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, this);
            }
            catch(Exception ex)
            {
                var str=string.Format("异常信息{0}；追踪信息来源：{1}", ex, ex.StackTrace);
            }
            try
            {
                parseData();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 异步读取数据
        /// </summary>
        /// <param name="ar"></param>
        private void ReadCallback(IAsyncResult ar)
        {
            readCount = stream.EndRead(ar);
            if (readCount == 0)
            {
                client.Close();
                stream.Close();
                stream.Dispose();
            }
            byte[] newbuffer = new byte[readCount];
            buffer.CopyTo(newbuffer, readCount);
            recice.AddRange(newbuffer);
        }

        /// <summary>
        /// 解析数据信息
        /// </summary>
        private void parseData()
        {

        }
    }
}