using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BJUTDUHelper.Model;

namespace BJUTDUHelper.Model
{
    public class BJUTDUHelperDbContext:DbContext
    {
        public DbSet<BJUTEduCenterUserinfo> EduUsers { get; set; }
        public DbSet<BJUTLibCenterUserinfo> LibUsers { get; set; }
        public DbSet<BJUTInfoCenterUserinfo>InfoUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=BJUTDUHelper.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserBase>().ToTable("Users");
        }


    }
}
