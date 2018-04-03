using CloudPrinter.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace CloudPrinter.TCPServer
{
    public class PrinterWifeData
    {
        public string number = "";
        public int printCache = 0;
        public Task<PrinterModels> printer;
        public ApplicationDbContext db = new ApplicationDbContext();
        //处理返回验证的信息码值，解析其他设备信息可以调用该方法获取
        public int wifeDevInfo(int count, byte[] parseData)
        {
            try
            {
                List<byte> recide = new List<byte>();
                recide.AddRange(parseData);
                for (int i = 0; i < count; i++)
                {
                    switch (recide[1])
                    {
                        case 1:
                            string str = Encoding.UTF8.GetString(recide.ToArray(), 3, recide[0] - 3);
                            string str1 = "";
                            for (int j1 = 0; j1 < str.Length; j1++)
                            {
                                if (str[j1] == recide[2])
                                {
                                    //处理每个获取到的数据
                                    //处理完毕再赋值为空
                                    str1 = "";
                                }
                                else
                                {
                                    str1 += str[j1];
                                }
                            }
                            break;
                        case 2:
                            int cache = (recide[2] << 24) + (recide[3] << 16) + (recide[4] << 8) + recide[5];
                            break;
                        case 3:
                            for (int j3 = 0; j3 < recide[2]; j3++)
                            {
                                number += string.Format("{0:X2}", recide[j3 + 3]);
                            }
                            break;
                    }
                    recide.RemoveRange(0, recide[0]);
                }
                if (number == "")
                {
                    return 2;//信息获取失败
                }

                //TransactionOptions transactionOption = new TransactionOptions();
                ////设置事务隔离级别
                //transactionOption.IsolationLevel = IsolationLevel.Snapshot;
                ////设置事务超时时间，这里设置为1分钟
                //transactionOption.Timeout = new TimeSpan(0, 1, 0);
                //using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                //{
                printer = db.PrinterModels.FindAsync(new System.Threading.CancellationToken(), number);
                if (printer != null)
                {
                    if (printer.Result.mState)
                    {
                        return 3;//已经登录
                    }
                    else
                    {
                        if (printer != null)
                        {
                            printer.Result.mState = true;
                            db.Entry(printer.Result).State = EntityState.Modified;
                            db.SaveChangesAsync();
                        }
                        return 0;//成功
                    }
                }
                else
                {
                    return 1;//未注册
                }
                //}

            }
            catch (Exception ex)
            {
                string s = string.Format("{0}", ex);
                return 4;//其他错误未定义
            }
        }
        /// <summary>
        /// 获取消息体内容
        /// </summary>
        /// <param name="data"></param>
        public bool wifeDevState(byte[] data)
        {
            try
            {
                bool flge = false;
                if (data[2] != data[4])
                {
                    if (data[20] == 1)
                    {


                        //接受缓存已满
                        if (printer != null)
                        {
                            printer.Result.cState = "error";
                            printer.Result.stateMessage = "缓存已满";
                            db.Entry(printer.Result).State = EntityState.Modified;
                            db.SaveChangesAsync();
                        }

                        if (SharData.dicSharData.ContainsKey(number))
                        {
                            int a;
                            SharData.dicSharData.TryRemove(number, out a);
                        }
                    }
                    else
                    {
                        printCache = data[21] + (data[22] << 8) + (data[23] << 16) + (data[24] << 24);
                        if (SharData.dicSharData.ContainsKey(number))
                        {
                            int a;
                            SharData.dicSharData.TryRemove(number, out a);
                        }
                        SharData.dicSharData.TryAdd(number, printCache);
                        flge = true;
                    }
                }
                else//说明只获取一个类型内容
                {
                    switch (data[5])
                    {
                        case 0x30:
                            //先不处理
                            flge = true;
                            break;
                        case 0x31:
                            //获取数据流状态信息
                            if (data[6] == 1)
                            {
                                //接受缓存已满
                                if (printer != null)
                                {
                                    printer.Result.cState = "error";
                                    printer.Result.stateMessage = "缓存已满";
                                    db.Entry(printer.Result).State = EntityState.Modified;
                                    db.SaveChangesAsync();
                                }

                                if (SharData.dicSharData.ContainsKey(number))
                                {
                                    int a;
                                    SharData.dicSharData.TryRemove(number, out a);
                                }
                                flge = false;
                            }
                            else
                            {
                                printCache = data[10] + (data[9] << 8) + (data[8] << 16) + (data[7] << 24);
                                if (SharData.dicSharData.ContainsKey(number))
                                {
                                    int a;
                                    SharData.dicSharData.TryRemove(number, out a);
                                }
                                SharData.dicSharData.TryAdd(number, printCache);
                                flge = true;
                            }
                            break;
                    }
                }
                return flge;
            }
            catch (Exception ex)
            {
                string s = string.Format("{0}", ex);
                return false;
            }
        }
    }
}