using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudPrinter.Models
{
    public class PrinterModels
    {
        [Required()]
        [Display(Name ="用户ID")]
        public virtual string userAccount { get; set; }

        [Required]
        [StringLength(20)]
        [Key]
        [Display(Name ="设备注册号")]
        public virtual string printerNumber { get; set; }
        [StringLength(50)]
        [Display(Name ="打印机名称")]
        public virtual string printerName { get; set; }
        [Required]
        [Display(Name ="是否在线")]
        public virtual bool mState { get; set; }
        [Required]
        [StringLength(10)]
        [Display(Name ="状态")]
        public virtual string cState { get; set; }
        [StringLength(50)]
        [Display(Name ="状态信息")]
        public virtual string stateMessage { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "注册时间")]
        public virtual DateTime registerTime { get; set; }
        [Display(Name ="用户名称")]
        public virtual string userName { get; set; }
       
    }
}