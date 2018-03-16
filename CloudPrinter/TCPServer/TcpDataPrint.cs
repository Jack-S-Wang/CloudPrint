using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Web;

namespace CloudPrinter.TCPServer
{

    public class TcpDataPrint
    {
        TcpClient clientData;
        NetworkStream streamData;
        int readCount = 0;
        List<byte> recice = new List<byte>();
        List<byte> printRecide = new List<byte>();
        byte[] buffer = new byte[1000];
        static Random rd = new Random();
        PrinterWifeData pwd;
        string number = "";
        System.Timers.Timer ti = new System.Timers.Timer(30000);
        public static ConcurrentDictionary<string, TcpDataPrint> dic = new ConcurrentDictionary<string, TcpDataPrint>();
        public TcpDataPrint(TcpClient client,string number)
        {
            clientData = client;
            this.number = number;
            pwd = new PrinterWifeData();
            streamData = clientData.GetStream();
            if (streamData != null)
            {
                try
                {
                    parseNumber();
                    streamData.BeginRead(buffer, 0, buffer.Length, ReadCallback, this);
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
            try
            {
                clientData.Close();
                streamData.Close();
                streamData.Dispose();
                ti.Enabled = false;
                ti.Close();
                ti.Dispose();
                if (SharData.dicByteData.ContainsKey(number))
                {
                    List<byte> a;
                    SharData.dicByteData.TryRemove(number, out a);
                }
                SharData.dicHttp[number].printResult = true;
            }
            catch
            {
                return;
            }
           
        }
        int dd = 0;
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
            data[5] = (byte)(id >> 8);
            data[6] = (byte)(id >> 16);
            data[7] = (byte)(id >> 24);
            data[8] = (byte)len;
            data[9] = (byte)(len >> 8);
            data[10] = (byte)(len >> 16);
            data[11] = (byte)(len >> 24);
            data[12] = ConstNumber.PARSE_NUMBER;
            data[13] = 0;
            dd = 1;
            streamData.BeginWrite(data, 0, data.Length, WriteCallback, this);
        }
        /// <summary>
        /// 异步发送信息
        /// </summary>
        /// <param name="ar"></param>
        private void WriteCallback(IAsyncResult ar)
        {
            try
            {
                streamData.EndWrite(ar);
            }
            catch
            {
                return;
            }
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
                    SharData.dicHttp[number].message = "设备无响应！";
                    closeObject();
                    return;
                }
                byte[] newbuffer = new byte[readCount];
                Array.Copy(buffer, newbuffer, readCount);
                recice.AddRange(newbuffer);
            }
            catch(Exception ex)
            {
                SharData.dicHttp[number].message =ex.Message;
                closeObject();
                return;
            }
            try
            {
                parseData();
                
            }
            catch(Exception ex)
            {
                SharData.dicHttp[number].message = ex.Message;
                closeObject();
                return;
            }
        }
        /// <summary>
        /// 读取数据处理
        /// </summary>
        private void parseData()
        {
            int bodySize = 0;
            while (ReadMessagefull(ref bodySize))
            {
                bool falge = false;
                byte[] StateData = new byte[bodySize];
                Array.Copy(recice.ToArray(), ConstNumber.HEADER_LENGTH, StateData, 0, bodySize);
                switch (recice[12])
                {
                    case ConstNumber.PARSE_NUMBER:
                        string snumber = "";
                        for(int i = 0; i < StateData.Length; i++)
                        {
                            snumber += string.Format("{0:X2}", StateData[i]);
                        }
                        if (!snumber.Equals(number))
                        {
                            SharData.dicHttp[number].message = "设备认证的number不一致";
                            closeObject();
                        }
                        printRecide = SharData.dicByteData[number];
                        //开启定时检测
                        ti.Enabled = true;
                        ti.Elapsed += ((s,e)=>{
                            SharData.dicHttp[number].message = "打印已经超时，无法打印";
                            closeObject();
                        });
                        //发送打印数据
                        if (SharData.dicSharData.ContainsKey(number))
                        {
                            if (sendDataPrint())
                            {
                                falge= true;
                            }
                        }else
                        {
                            sendNullData();
                        }
                        break;
                    case ConstNumber.PRINT_INDEX:
                        if (pwd.wifeDevState(StateData))
                        {
                            if (sendDataPrint())
                            {
                                falge= true;
                            }
                        }else
                        {
                            sendNullData();
                        }
                        break;
                }
                recice.RemoveRange(0, ConstNumber.HEADER_LENGTH + bodySize);
                if (!falge)
                {
                    streamData.BeginRead(buffer, 0, buffer.Length, ReadCallback, this);
                }
               
            }
            
        }

        /// <summary>
        /// 发送打印数据
        /// </summary>
        private bool sendDataPrint()
        {
            if (printRecide.Count == 0)
            {
                SharData.dicHttp[number].message = "打印成功";
                closeObject();
                return true;
            }
            int len = SharData.dicSharData[number];
            int pLen = 0;
            if (printRecide.Count >= len)
            {
                pLen = len;
            }else
            {
                pLen = printRecide.Count;
            }
            byte[] dataP = new byte[pLen];
            Array.Copy(printRecide.ToArray(), dataP, pLen);
            byte[] data = new byte[pLen+ConstNumber.HEADER_LENGTH];
            Array.Copy(ConstNumber.signData, data, 4);
            int id = rd.Next(1000, 5000);
            data[4] = (byte)id;
            data[5] = (byte)(id >> 8);
            data[6] = (byte)(id >> 16);
            data[7] = (byte)(id >> 24);
            data[8] = (byte)pLen;
            data[9] = (byte)(pLen >> 8);
            data[10] = (byte)(pLen >> 16);
            data[11] = (byte)(pLen >> 24);
            data[12] = ConstNumber.PRINT_INDEX;
            data[13] = 0;
            Array.Copy(dataP, 0, data, ConstNumber.HEADER_LENGTH, pLen);
            dd = 2;
            streamData.BeginWrite(data, 0, data.Length, WriteCallback, this);
            printRecide.RemoveRange(0, pLen);
            return false;
        }

        /// <summary>
        /// 发送空包数据查询缓存是否可以执行了
        /// </summary>
        private void sendNullData()
        {
            byte[] data = new byte[ConstNumber.HEADER_LENGTH];
            Array.Copy(ConstNumber.signData, data, 4);
            int id = rd.Next(1000, 5000);
            data[4] = (byte)id;
            data[5] = (byte)(id >> 8);
            data[6] = (byte)(id >> 16);
            data[7] = (byte)(id >> 24);
            data[12] = ConstNumber.PRINT_INDEX;
            data[13] = 0;
            dd = 3;
            streamData.BeginWrite(data, 0, ConstNumber.HEADER_LENGTH, WriteCallback, this);
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