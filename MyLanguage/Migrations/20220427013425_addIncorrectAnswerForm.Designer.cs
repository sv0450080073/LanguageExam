﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyLanguage.Data;

namespace MyLanguage.Migrations
{
    [DbContext(typeof(MyLanguageDbContext))]
    [Migration("20220427013425_addIncorrectAnswerForm")]
    partial class addIncorrectAnswerForm
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.14")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MyLanguage.Models.ExamForm", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ExamDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExamName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("ExamForm");
                });

            modelBuilder.Entity("MyLanguage.Models.ExamFormDetail", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AmHanViet")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AmKun")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AmOn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ExamForm_ID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Hiragana")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("KanJiID")
                        .HasColumnType("int");

                    b.Property<string>("KanJiWord")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VietNamMean")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("ExamFormDetail");
                });

            modelBuilder.Entity("MyLanguage.Models.IncorrectAnswerForm", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ExamDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ExamForm_Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("IncorrectAnswerForm");
                });

            modelBuilder.Entity("MyLanguage.Models.IncorrectAnswerFormDetail", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ExamFormDetail_Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("IncorrectAmHanViet")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IncorrectAmKun")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IncorrectAmOn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("IncorrectAnswerForm_Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("IncorrectHiragana")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IncorrectKanJiWord")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IncorrectVietNamMean")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("KanJiID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("IncorrectAnswerFormDetail");
                });

            modelBuilder.Entity("MyLanguage.Models.KanJi", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("HanViet")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("KanJiWord")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("VNMean")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("KanJis");
                });

            modelBuilder.Entity("MyLanguage.Models.UserScores", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ExamDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ExamForm_ID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Score")
                        .HasColumnType("float");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("UserScores");
                });
#pragma warning restore 612, 618
        }
    }
}
