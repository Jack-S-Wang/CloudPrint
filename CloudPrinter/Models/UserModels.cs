using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudPrinter.Models
{
    public class UserModels
    {
        public virtual int UserModelsId { get; set; }

        [Required]
        [StringLength(64)]
        [Index(IsUnique =true)]
        [Display(Name ="账号")]
        public virtual string userName { get; set; }

        [Required]
        [StringLength(13)]
        [DataType(DataType.Password)]
        [Display(Name ="密码")]
        public virtual string password { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "注册时间")]
        public virtual DateTime registerDate { get; set; }
    }
}
