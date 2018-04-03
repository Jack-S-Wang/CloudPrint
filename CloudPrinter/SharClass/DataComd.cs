using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CloudPrinter.SharClass
{
    public class DataComd
    {
        public byte[] reComdData(byte[] data,int pageWidth)
        {
            string t = data.Length.ToString();
            string x = (pageWidth / 8).ToString();
            string hread = "~DGR:"+DateTime.Now.ToString("yyyyMMddHHmmss")+".jpeg," + t + "," + x + ",";
            var datah = Encoding.ASCII.GetBytes(hread);
            string foot = "^XA\r\n^F020,20^XGR:SAMPLE.GRF,1,1^FS\r\n^XZ\r\n";
            var dataf = Encoding.ASCII.GetBytes(foot);
            byte[] dataAll = new byte[datah.Length + data.Length + dataf.Length];
            Array.Copy(datah, 0, dataAll, 0, datah.Length);
            Array.Copy(data, 0, dataAll, datah.Length, data.Length);
            Array.Copy(dataf, 0, dataAll, datah.Length + data.Length, dataf.Length);
            return dataAll;
        }
    }
}