using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using BJUTDUHelper.Model;

namespace BJUTDUHelper.Migrations
{
    [DbContext(typeof(BJUTDUHelperDbContext))]
    partial class BJUTDUHelperDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("BJUTDUHelper.Model.UserBase", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Password");

                    b.Property<string>("Username");

                    b.HasKey("ID");

                    b.ToTable("UserBase");

                    b.HasDiscriminator<string>("Discriminator").HasValue("UserBase");
                });

            modelBuilder.Entity("BJUTDUHelper.Model.BJUTEduCenterUserinfo", b =>
                {
                    b.HasBaseType("BJUTDUHelper.Model.UserBase");

                    b.Property<int>("EduSystemType");

                    b.ToTable("BJUTEduCenterUserinfo");

                    b.HasDiscriminator().HasValue("BJUTEduCenterUserinfo");
                });

            modelBuilder.Entity("BJUTDUHelper.Model.BJUTInfoCenterUserinfo", b =>
                {
                    b.HasBaseType("BJUTDUHelper.Model.UserBase");


                    b.ToTable("BJUTInfoCenterUserinfo");

                    b.HasDiscriminator().HasValue("BJUTInfoCenterUserinfo");
                });

            modelBuilder.Entity("BJUTDUHelper.Model.BJUTLibCenterUserinfo", b =>
                {
                    b.HasBaseType("BJUTDUHelper.Model.UserBase");


                    b.ToTable("BJUTLibCenterUserinfo");

                    b.HasDiscriminator().HasValue("BJUTLibCenterUserinfo");
                });
        }
    }
}
