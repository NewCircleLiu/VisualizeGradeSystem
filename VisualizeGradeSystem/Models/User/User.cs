using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace VisualizeGradeSystem.Models.User
{
    public class User
    {
        [Key]
        public int user_id { get; set; } 
        public string user_account { get; set; }//学号
        public string user_password { get; set; }//密码
        public string user_kind { get; set; } //用户种类：stu,teacher,admin
        public string user_name { get; set; } //姓名
        public string user_depart { get; set; } //系
        public string user_class { get; set; } //班


    }
}