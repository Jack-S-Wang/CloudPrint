using CloudPrinter.Models;
using CloudPrinter.TCPServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace CloudPrinter.Controllers
{
    public class SendHttpController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public bool printResult = false;
        public string message = "";
        [HttpPost]
        public string printData(string printerNumber, string printDt)
        {
            if (TcpPrinter.dicTcp.ContainsKey(printerNumber))
            {
                if (printDt.Length == 0 || printDt.Length % 2 != 0)
                {
                    return "发送的数据不符合！";
                }
                List<byte> li = new List<byte>();
                for (int i = 0; i < printDt.Length; i++)
                {
                    li.Add(Convert.ToByte(printDt[i].ToString(),16));
                }
                if (SharData.dicByteData.ContainsKey(printerNumber))
                {
                    return "该设备正繁忙中！";
                }
                if (!db.PrinterModels.FindAsync(printerNumber).Result.mState)
                {
                    return "设备已离线!";
                }
                SharData.dicByteData.TryAdd(printerNumber,li);
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
        [HttpPost]
        public string gethh()
        {
            return "shouda";
        }
    }
}
