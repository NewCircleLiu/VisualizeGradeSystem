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
        public string user_account { get; set; }//学号/教工号
        public string user_password { get; set; }//密码
        public string user_kind { get; set; } //用户种类：stu,teacher,admin,head（课题组负责人，附上上传excel的那个人）
        public string user_name { get; set; } //姓名
        public string user_depart { get; set; } //系
        public string user_class { get; set; } //班:对于学生表示他属于哪个班，对于老师表示他交哪个班：Class1,Class2,Class3
        public string user_subject { get; set; } //只针对教师，表示属于哪个课题组

    }
}