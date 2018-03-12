﻿using CloudPrinter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CloudPrinter.TCPServer
{
    public class PrinterWifeData
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        public string number = "";
        public int printCache = 0;
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
                            int cache = (recide[2] >> 24) + (recide[3] >> 16) + (recide[4] >> 8) + recide[5];
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
                var printer = db.PrinterModels.Find(number);
                if (printer != null)
                {
                    if (printer.mState)
                    {
                        return 3;//已经登录
                    }
                    else
                    {
                        return 0;//成功
                    }
                }
                else
                {
                    return 1;//未注册
                }

            }
            catch
            {
                return 4;//其他错误未定义
            }
        }
        /// <summary>
        /// 获取消息体内容
        /// </summary>
        /// <param name="data"></param>
        public void wifeDevState(byte[] data)
        {
            if (data[2] != data[4])
            {
                if (data[20] == 1)
                {
                    //接受缓存已满
                    var printer = db.PrinterModels.Find(number);
                    if (printer != null)
                    {
                        printer.cState = "error";
                        printer.stateMessage = "缓存已满";
                    }
                }
                else
                {
                    
                    printCache = data[21] + (data[22] >> 8) + (data[23] >> 16) + (data[24] >> 24);
                }
            }
            else//说明只获取一个类型内容
            {
                switch (data[5])
                {
                    case 0x30:
                        //先不处理
                        break;
                    case 0x31:
                        //获取数据流状态信息
                        if (data[6] == 1)
                        {
                            //接受缓存已满
                            var printer=db.PrinterModels.Find(number);
                            if (printer != null)
                            {
                                printer.cState = "error";
                                printer.stateMessage = "缓存已满";
                            }
                        }else
                        {
                            printCache = data[7] + (data[8] >> 8) + (data[9] >> 16) + (data[10] >> 24);
                        }
                        break;
                }
            }
        }
    }
}