using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace VisualizeGradeSystem.Models.Score
{
    public class Score
    {
        [Key]
        public int score_id { get; set; }
        public string score_gradeid { get; set; }  //考号
        public string stu_name { get; set; }  //学生姓名
        public string stu_id { get; set; }
        public double score { get; set; }
        public string updatetime { get; set; }
        public string uploadtime { get; set; }  //上传该文件的时间，区分各次考试
        public string stu_depart { get; set; }
        public string stu_class { get; set; }
        public string subject { get; set; }
        public string uploader { get; set; } //上传者
    }
}