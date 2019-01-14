using CloudPrinter.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace CloudPrinter.TCPServer
{
    public class TcpPrinter
    {
        public static ConcurrentDictionary<string, TcpPrinter> dicTcp = new ConcurrentDictionary<string, TcpPrinter>();
        TcpClient client;
        NetworkStream stream;
        int readCount = 0;
        List<byte> recice = new List<byte>();
        byte[] buffer = new byte[1000];
        PrinterWifeData pwd;
        System.Timers.Timer ti = new System.Timers.Timer(5000);
        static Random rd = new Random();
        volatile bool isBeatWife = true;
        bool isBeatDev = false;
        string errorStr = "";
        int concurrentWrites = 0;
        int concurrentReads = 0;

        void MyBeginRead()
        {
            try
            {
                int newConcurrentReads = Interlocked.Increment(ref concurrentReads);
                if (newConcurrentReads > 1)
                {
                    // TODO 记录一个醒目的日志
                    using (FileStream file = new FileStream("C:\\项目程序\\readlog.txt", FileMode.OpenOrCreate))
                    {
                        if (file.Length > 0)
                        {
                            file.SetLength(0);
                            file.Seek(0, 0);
                        }
                        var data = Encoding.UTF8.GetBytes("重复发送了");
                        file.Write(data, 0, data.Length);
                    }
                }

                stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, this);
            }
            catch
            {
                Interlocked.Decrement(ref concurrentReads);
            }
        }

        void MyBeginWrite(byte[] buff, int off, int len)
        {

            try
            {
                int newConcurrentWrites = Interlocked.Increment(ref concurrentWrites);
                if (newConcurrentWrites > 1)
                {
                    // TODO 记录一个醒目的日志
                }

                stream.BeginWrite(buff, off, len, WriteCallback, this);
            }
            catch
            {
                Interlocked.Decrement(ref concurrentWrites);
            }
        }


        public TcpPrinter(TcpClient client)
        {
            this.client = client;
            client.NoDelay = true;
            int size = client.ReceiveBufferSize;
            client.ReceiveBufferSize = size * 2;
            stream = client.GetStream();
            if (stream != null)
            {
                pwd = new PrinterWifeData();
                try
                {
                    //MyBeginRead();
                    stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, this);
                }
                catch (Exception ex)
                {
                    var str = string.Format("异常信息{0}；追踪信息来源：{1}", ex, ex.StackTrace);
                    errorStr += (str + "\r\n");
                }
            }
            else
            {
                closeObject();
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
                //Interlocked.Decrement(ref concurrentReads);
                readCount = stream.EndRead(ar);
                if (readCount == 0)
                {
                    closeObject();
                    return;
                }
                byte[] newbuffer = new byte[readCount];
                Array.Copy(buffer, newbuffer, readCount);
                recice.AddRange(newbuffer);
            }
            catch (Exception ex)
            {
                string s = string.Format("{0}", ex);
                errorStr += (s + "\r\n");
                closeObject();
            }
            try
            {
                parseData();
            }
            catch (Exception ex)
            {
                string s = string.Format("{0}", ex);
                errorStr += (s + "\r\n");

                closeObject();
            }
            try
            {
                //MyBeginRead();
                stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, this);
            }
            catch
            {

                closeObject();
                return;
            }
        }

        private void closeObject()
        {
            try
            {
                client.Close();
                stream.Close();
                stream.Dispose();
                ti.Enabled = false;
                ti.Close();
                ti.Dispose();
                if (pwd.printer != null)
                {
                    pwd.printer.Result.mState = false;
                    pwd.db.Entry(pwd.printer.Result).State = EntityState.Modified;
                    pwd.db.SaveChangesAsync();
                }
                TcpPrinter tp;
                dicTcp.TryRemove(pwd.number, out tp);
                int a;
                SharData.dicSharData.TryRemove(pwd.number, out a);
                using (FileStream file = new FileStream("C:\\项目程序\\readlog.txt", FileMode.OpenOrCreate))
                {
                    if (file.Length > 0)
                    {
                        file.SetLength(0);
                        file.Seek(0, 0);
                    }
                    var data = Encoding.UTF8.GetBytes(errorStr);
                    file.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                string s = string.Format("{0}", ex);
                return;
            }
        }

        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="ar"></param>
        private void WriteCallback(IAsyncResult ar)
        {
            try
            {
                //Interlocked.Decrement(ref concurrentWrites);
                stream.EndWrite(ar);
            }
            catch (Exception ex)
            {
                string s = string.Format("{0}", ex);
                errorStr += (s + "\r\n");
                return;
            }
        }
        /// <summary>
        /// 解析数据信息
        /// </summary>
        private void parseData()
        {
            int bodySize = 0;
            while (ReadMessagefull(ref bodySize))
            {
                switch (recice[12])
                {
                    case ConstNumber.SUER_INDEX:
                        approvePrinter(bodySize);
                        break;
                    case ConstNumber.HREAT_INDEX:
                        HreatPrinter(bodySize);
                        break;
                }
                recice.RemoveRange(0, ConstNumber.HEADER_LENGTH + bodySize);
            }
        }

        private void HreatPrinter(int bodySize)
        {
            byte[] StateData = new byte[bodySize];
            Array.Copy(recice.ToArray(), ConstNumber.HEADER_LENGTH, StateData, 0, bodySize);
            switch (recice[13])
            {
                case 0:
                    pwd.wifeDevState(StateData);
                    isBeatWife = true;
                    break;
                case 1:
                    //设备先不解析
                    isBeatWife = true;
                    break;
            }
        }
        /// <summary>
        /// 认证打印机信息
        /// </summary>
        /// <param name="bodySize">消息体长度</param>
        private void approvePrinter(int bodySize)
        {
            if (recice[13] != ConstNumber.SUER_HIGNINDEX)
            {
                //var str = "认证消息高字节值不符合！";
            }
            byte[] data = new byte[bodySize];
            Array.Copy(recice.ToArray(), ConstNumber.HEADER_LENGTH, data, 0, bodySize);
            int recode = 0;
            int bodys = bodySize;
            while (data[2] <= bodys)
            {
                bodys = bodys - (data[2] + 4);
                byte[] dataNew = new byte[data[2]];
                Array.Copy(data, 4, dataNew, 0, data[2]);
                int code = printerParse(dataNew);
                if (data[5] == 0x80)
                {
                    recode = code;//返回验证信息
                }

                if (bodys == 0)
                {
                    break;
                }
                byte[] dataR = new byte[bodys];
                Array.Copy(data, data[2] + 4, dataR, 0, bodys);
                data = dataR;
            }
            //将验证之后的信息内容返回
            byte[] dataWirte = new byte[ConstNumber.SUER_WRITE];
            int len = ConstNumber.SUER_WRITE - ConstNumber.HEADER_LENGTH;
            Array.Copy(ConstNumber.signData, dataWirte, 4);
            dataWirte[4] = recice[4];
            dataWirte[5] = recice[5];
            dataWirte[6] = recice[6];
            dataWirte[7] = recice[7];
            dataWirte[11] = (byte)(len >> 24);
            dataWirte[10] = (byte)(len >> 16);
            dataWirte[9] = (byte)(len >> 8);
            dataWirte[8] = (byte)len;
            Array.Copy(ConstNumber.suerData, 0, dataWirte, 12, 2);
            //后面结构上的字节都可为零
            dataWirte[20] = (byte)recode;
            dataWirte[21] = (byte)(recode >> 8);
            dataWirte[22] = (byte)(ti.Interval / 1000);
            dataWirte[23] = (byte)((int)(ti.Interval / 1000) >> 8);
            var het = hreatSend();
            byte[] dataAll = new byte[dataWirte.Length + het.Length];
            Array.Copy(dataWirte, 0, dataAll, 0, dataWirte.Length);
            Array.Copy(het, 0, dataAll, dataWirte.Length, het.Length);


            //MyBeginWrite(dataAll, 0, dataAll.Length);
            stream.BeginWrite(dataAll, 0, dataAll.Length, WriteCallback, this);

            if (recode == 0)//认证成功，开启心跳
            {
                ti.Enabled = true;
                ti.Elapsed += ((o, e) =>
                {

                    if (!isBeatWife)
                    {
                        closeObject();
                        return;
                    }
                    else
                    {
                        isBeatWife = false;
                        var hte = hreatSend();
                        // MyBeginWrite(hte, 0, hte.Length);
                        stream.BeginWrite(hte, 0, hte.Length, WriteCallback, this);
                        //stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, this);
                    }
                });
                //存储
                dicTcp.TryAdd(pwd.number, this);
            }
        }

        /// <summary>
        /// 心跳发送
        /// </summary>
        private byte[] hreatSend()
        {
            try
            {
                byte[] hd = new byte[ConstNumber.HREAT_WRITE * 2];
                for (int i = 0; i < 2; i++)
                {
                    byte[] hreatData = new byte[ConstNumber.HREAT_WRITE];
                    int id = rd.Next(1000, 5000);
                    int count = 5;
                    Array.Copy(ConstNumber.signData, hreatData, 4);
                    hreatData[4] = (byte)id;
                    hreatData[5] = (byte)(id >> 8);
                    hreatData[6] = (byte)(id >> 16);
                    hreatData[7] = (byte)(id >> 24);
                    hreatData[8] = (byte)count;
                    hreatData[9] = (byte)(count >> 8);
                    hreatData[10] = (byte)(count >> 16);
                    hreatData[11] = (byte)(count >> 24);
                    hreatData[12] = ConstNumber.HREAT_INDEX;
                    hreatData[13] = (byte)i;
                    hreatData[20] = 0x10;
                    hreatData[21] = 0x09;
                    hreatData[22] = 1;
                  
                    if (i == 0)
                    {
                        hreatData[23] = 0x31;
                        hreatData[24] = 0x31;
                        Array.Copy(hreatData, hd, ConstNumber.HREAT_WRITE);
                    }
                    else
                    {
                        hreatData[23] = 0x30;
                        hreatData[24] = 0x30;
                        Array.Copy(hreatData, 0, hd, ConstNumber.HREAT_WRITE, ConstNumber.HREAT_WRITE);
                    }
                }
                return hd;
            }
            catch (Exception ex)
            {
                string s = string.Format("{0}", ex);
                errorStr += (s + "\r\n");
                return new byte[0];
            }
        }

        //打印机信息处理
        private int printerParse(byte[] data)
        {
            int recode = -1;
            switch (data[1])
            {
                case 0x80:
                    //weife
                    int count = (data[3] >> 8) + data[2];
                    byte[] parseData = new byte[data.Length - 4];
                    Array.Copy(data, 4, parseData, 0, parseData.Length);
                    recode = pwd.wifeDevInfo(count, parseData);
                    break;
                case 0x01:
                    break;
                case 0x02:
                    break;
            }
            return recode;
        }

        /// <summary>
        /// 数据打印发送给设备
        /// </summary>
        public void dataPrint()
        {
            byte[] data = new byte[ConstNumber.HEADER_LENGTH];
            Array.Copy(ConstNumber.signData, data, 4);
            int id = rd.Next(1000, 5000);
            data[4] = (byte)id;
            data[5] = (byte)(id >> 8);
            data[6] = (byte)(id >> 16);
            data[7] = (byte)(id >> 24);
            data[12] = ConstNumber.PRINT_CREATE;
            data[13] = 0;
            stream.BeginWrite(data, 0, data.Length, WriteCallback, this);
            new TcpDataPrintServer(pwd.number);

        }

        /// <summary>
        /// 判断是否是一个完整的消息内容
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

    public class ConstNumber
    {
        /// <summary>
        /// 标头
        /// </summary>
        public const int HEADER_LENGTH = 20;
        /// <summary>
        /// 认证消息号
        /// </summary>
        public const byte SUER_INDEX = 1;
        /// <summary>
        /// 认证消息号前的高字节
        /// </summary>
        public const byte SUER_HIGNINDEX = 2;
        /// <summary>
        /// 认证发送数据总字节数
        /// </summary>
        public const int SUER_WRITE = 24;

        /// <summary>
        /// 标记数据，固定值
        /// </summary>
        public static byte[] signData = new byte[] { 0x40, 0x41, 0x2F, 0x3F };
        /// <summary>
        /// 认证数据的消息号
        /// </summary>
        public static byte[] suerData = new byte[] { 1, 2 };
        /// <summary>
        /// 发送心跳总数目
        /// </summary>
        public const int HREAT_WRITE = 25;
        /// <summary>
        /// 心跳消息号
        /// </summary>
        public const int HREAT_INDEX = 5;
        /// <summary>
        /// 打印创建数据通道
        /// </summary>
        public const int PRINT_CREATE = 3;
        /// <summary>
        /// 验证number值的消息号
        /// </summary>
        public const int PARSE_NUMBER = 4;
        /// <summary>
        /// 透传打印数据消息号
        /// </summary>
        public const int PRINT_INDEX = 2;
    }
}