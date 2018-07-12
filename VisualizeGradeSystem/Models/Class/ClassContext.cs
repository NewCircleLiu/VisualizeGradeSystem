using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace VisualizeGradeSystem.Models.Class
{
    public class ClassContext : DbContext
    {
        public ClassContext()
            : base("name=Class-Context")
        { }

       public DbSet<Class> ClassList { get; set; }
    }
}
