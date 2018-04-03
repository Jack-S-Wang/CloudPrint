using CloudPrinter.Models;
using CloudPrinter.SharClass;
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
            string printerNumber = "";
            try
            {
                PDFToImage pdf = new PDFToImage();
                DataComd dc = new DataComd();
                var httpRequest = HttpContext.Current.Request;
                printerNumber = httpRequest.Form["printerNumber"] as string;  // 获取 FormData的键值 
                var file = httpRequest.Files[0];
                var printDt = new byte[file.InputStream.Length];
                file.InputStream.Read(printDt, 0, printDt.Length);
                //将pdf进行转化为图片
                byte[] imgData=pdf.getBitmap(printDt);
                int width = pdf.getBitmapWidth();
                byte[] dataComd = dc.reComdData(imgData,width);
                if (TcpPrinter.dicTcp.ContainsKey(printerNumber))
                {
                    List<byte> li = new List<byte>();
                    li.AddRange(dataComd);
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
            catch(Exception ex)
            {
                if (SharData.dicByteData.ContainsKey(printerNumber))
                {
                    List<byte> b;
                    SharData.dicByteData.TryRemove(printerNumber, out b);
                }
                if (SharData.dicHttp.ContainsKey(printerNumber))
                {
                    SendHttpController s;
                    SharData.dicHttp.TryRemove(printerNumber, out s);
                }
                return "请求协议出现异常:"+string.Format("{0}",ex);
            }
           
        }

    }
}
