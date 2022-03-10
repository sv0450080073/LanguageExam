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
    }
}
