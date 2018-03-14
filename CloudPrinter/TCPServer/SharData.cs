using CloudPrinter.Controllers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        
        /// <summary>
        /// 存储打印数据
        /// </summary>
        public static ConcurrentDictionary<string, List<byte>> dicByteData = new ConcurrentDictionary<string, List<byte>>();

        /// <summary>
        /// 存储http请求对象
        /// </summary>
        public static ConcurrentDictionary<string, SendHttpController> dicHttp = new ConcurrentDictionary<string, SendHttpController>();
    }
}