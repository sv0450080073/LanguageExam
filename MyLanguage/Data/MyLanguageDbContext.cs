using Microsoft.EntityFrameworkCore;
using MyLanguage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Data
{
    public class MyLanguageDbContext :DbContext
    {
        public MyLanguageDbContext(DbContextOptions<MyLanguageDbContext> options)
          : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<KanJi> KanJis { get; set; }
        public DbSet<ExamForm> ExamForm { get; set; }
        public DbSet<ExamFormDetail> ExamFormDetail { get; set; }
        public DbSet<IncorrectAnswerForm> IncorrectAnswerForm { get; set; }
        public DbSet<IncorrectAnswerFormDetail> IncorrectAnswerFormDetail { get; set; }
        public DbSet<UserScores> UserScores { get; set; }
    }
}
