using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace VisualizeGradeSystem.Models.Puzzle
{
    public class PuzzleContext : DbContext
    {
        public PuzzleContext()
            : base("name=Puzzle-Context")
        { }

       public DbSet<Puzzle> PuzzleList { get; set; }
    }
}