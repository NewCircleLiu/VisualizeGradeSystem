using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace VisualizeGradeSystem.Models.Knowledge
{
    public class KnowledgeContext :DbContext
    {
        public KnowledgeContext()
            : base("name=Knowledge-Context")
        { }

       public DbSet<Knowledge> KnowledgeList { get; set; }
    }
}