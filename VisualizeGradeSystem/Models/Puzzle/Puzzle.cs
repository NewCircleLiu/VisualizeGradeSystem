using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace VisualizeGradeSystem.Models.Puzzle
{
    public class Puzzle
    {
        [Key]
        public int puzzle_id { get; set; }
        public string subject { get; set; }
        public string puzzle_name { get; set; }
        public string puzzle_knowledge { get; set; }
        public string uploadtime { get; set; }
        public string updatetime { get; set; }
        public string uploader { get; set; }
        public double puzzle_fullscore { get; set; }
    }
}