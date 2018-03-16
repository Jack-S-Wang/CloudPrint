using CloudPrinter.Models;
using CloudPrinter.TCPServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace CloudPrinter.Controllers
{
    public class SendHttpController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public bool printResult = false;
        public string message = "";
        [HttpPost]
        public string printData()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string printerNumber = httpRequest.Form["printerNumber"] as string;  // 获取 FormData的键值 
                var file = httpRequest.Files[0];
                var printDt = new byte[file.InputStream.Length];
                file.InputStream.Read(printDt, 0, printDt.Length);

                if (TcpPrinter.dicTcp.ContainsKey(printerNumber))
                {
                    List<byte> li = new List<byte>();
                    li.AddRange(printDt);
                    if (SharData.dicByteData.ContainsKey(printerNumber))
                    {
                        return "该设备正繁忙中！";
                    }
                    if (!db.PrinterModels.FindAsync(printerNumber).Result.mState)
                    {
                        return "设备已离线!";
                    }
                    SharData.dicByteData.TryAdd(printerNumber, li);
                    SharData.dicHttp.TryAdd(printerNumber, this);
                    TcpPrinter.dicTcp[printerNumber].dataPrint();
                    while (!printResult)
                    {
                        //一直等待
                        Thread.Sleep(1000);
                    }
                    SendHttpController a;
                    SharData.dicHttp.TryRemove(printerNumber, out a);
                    return message;
                }
                else
                {
                    return "设备已离线！";
                }
            }
            catch
            {
                //if (SharData.dicByteData.ContainsKey(printerNumber))
                //{
                //    List<byte> b;
                //    SharData.dicByteData.TryRemove(printerNumber, out b);
                //}
                //if (SharData.dicHttp.ContainsKey(printerNumber))
                //{
                //    SendHttpController s;
                //    SharData.dicHttp.TryRemove(printerNumber, out s);
                //}
                return "请求协议出现异常";
            }
        }

    }
}
