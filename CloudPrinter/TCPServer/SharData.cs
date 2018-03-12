using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace CloudPrinter.TCPServer
{
    public class SharData
    {
        /// <summary>
        /// 存储对应设备的数据信息
        /// </summary>
        public static ConcurrentDictionary<string, int> dicSharData = new ConcurrentDictionary<string, int>();
    }
}