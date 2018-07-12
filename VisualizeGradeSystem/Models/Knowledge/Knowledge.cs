using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace VisualizeGradeSystem.Models.Knowledge
{
    public class Knowledge
    {
        [Key]
        public int knowledge_id {get;set;}
        public string subject { get; set; }
        public string puzzle_name {get;set;} //这个题目的名字
        public string knowledge_name {get;set;}  //知识点的名字
        public string uploadtime { get; set; } //上传时间——作为一次考试
        public string updatetime { get; set; }
        public string uploader { get; set; }  //上传者
        public string score_gradeid { get; set; } //学生的考号
        public string stu_name { get; set; } //学生的名字
        public string stu_id { get; set; } //学生的学号
        public double score { get; set; } //该知识点的得分
        public string stu_class{get;set;}
    }
}