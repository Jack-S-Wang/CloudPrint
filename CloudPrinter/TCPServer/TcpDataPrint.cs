using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;

namespace CloudPrinter.TCPServer
{

    public class TcpDataPrint
    {
        TcpClient clientData;
        NetworkStream streamData;
        int readCount = 0;
        List<byte> recice = new List<byte>();
        byte[] buffer = new byte[1000];
        static Random rd = new Random();
        PrinterWifeData pwd;
        string number = "";
        public TcpDataPrint(TcpClient client)
        {
            clientData = client;
            pwd = new PrinterWifeData();
            streamData = clientData.GetStream();
            if (streamData != null)
            {
                try
                {
                    parseNumber();
                }
                catch
                {
                    closeObject();
                }
            }
            else
            {
                closeObject();
            }
        }
        private void closeObject()
        {
            clientData.Close();
            streamData.Close();
            streamData.Dispose();
        }
        /// <summary>
        /// 发送验证number值
        /// </summary>
        private void parseNumber()
        {
            byte[] data = new byte[22];
            Array.Copy(ConstNumber.signData, data, 4);
            int id = rd.Next(1000, 5000);
            int len = 2;
            data[4] = (byte)id;
            data[5] = (byte)(id << 8);
            data[6] = (byte)(id << 16);
            data[7] = (byte)(id << 24);
            data[8] = (byte)len;
            data[9] = (byte)(len << 8);
            data[10] = (byte)(len << 16);
            data[11] = (byte)(len << 24);
            data[12] = ConstNumber.PARSE_NUMBER;
            data[13] = 0;
            streamData.BeginWrite(data, 0, data.Length, WriteCallback, this);
        }
        /// <summary>
        /// 异步发送信息
        /// </summary>
        /// <param name="ar"></param>
        private void WriteCallback(IAsyncResult ar)
        {
            streamData.EndWrite(ar);
            streamData.BeginRead(buffer, 0, buffer.Length, ReadCallback, this);
        }

        /// <summary>
        /// 异步读取数据
        /// </summary>
        /// <param name="ar"></param>
        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                readCount = streamData.EndRead(ar);
                if (readCount == 0)
                {
                    closeObject();
                }
                byte[] newbuffer = new byte[readCount];
                Array.Copy(buffer, newbuffer, readCount);
                recice.AddRange(newbuffer);
            }
            catch
            {
                closeObject();
            }
            try
            {
                parseData();
            }
            catch
            {

            }
        }

        private void parseData()
        {
            int bodySize = 0;
            while (ReadMessagefull(ref bodySize))
            {
                byte[] StateData = new byte[bodySize];
                Array.Copy(recice.ToArray(), ConstNumber.HEADER_LENGTH, StateData, 0, bodySize);
                switch (recice[12])
                {
                    case ConstNumber.PARSE_NUMBER:
                        for(int i = 0; i < StateData.Length; i++)
                        {
                            number += string.Format("{0:X2}", StateData[i]);
                        }
                        break;
                    case ConstNumber.PRINT_INDEX:
                        pwd.wifeDevState(StateData);
                        break;
                }
            }
        }
        /// <summary>
        /// 校验是否是为一个完整的消息内容
        /// </summary>
        /// <param name="bodySize"></param>
        /// <returns></returns>
        private bool ReadMessagefull(ref int bodySize)
        {
            if (recice.Count < ConstNumber.HEADER_LENGTH)
            {
                return false;
            }

            bodySize =
               recice[8] +
               recice[9] * 256 +
               recice[10] * 256 * 256 +
               recice[11] * 256 * 256 * 256;

            return recice.Count >= ConstNumber.HEADER_LENGTH + bodySize;
        }
    }
}