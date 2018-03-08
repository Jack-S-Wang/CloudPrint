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
        [Required]
        //[StringLength(64)]
        [Key]
        [DataType(DataType.EmailAddress)]
        //[Index(IsUnique =true)]
        [Display(Name ="账号")]
        public virtual string userAccount { get; set; }

        [StringLength(64)]
        [Display(Name ="用户名")]
        public virtual string Name { get; set; }

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
