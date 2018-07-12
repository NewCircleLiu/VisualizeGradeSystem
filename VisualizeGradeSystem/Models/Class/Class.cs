using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace VisualizeGradeSystem.Models.Class
{
    public class Class 
    {
        [Key]
        public int class_id { get; set; }
        public string class_name { get; set; }
        public string class_depart { get; set; }

    }
}
