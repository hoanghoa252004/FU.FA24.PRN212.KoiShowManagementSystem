using System;
using System.Collections.Generic;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer;

public partial class Prn212ProjectKoiShowManagementContext : DbContext
{
    public Prn212ProjectKoiShowManagementContext()
    {
    }

    public Prn212ProjectKoiShowManagementContext(DbContextOptions<Prn212ProjectKoiShowManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Criterion> Criteria { get; set; }

    public virtual DbSet<Koi> Kois { get; set; }

    public virtual DbSet<RefereeDetail> RefereeDetails { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<Show> Shows { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Variety> Varieties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(GetConnectionString());

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnectionStringDB"];
        return strConn;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Criterion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Criteria__3214EC0710F6C804");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ShowId).HasColumnName("Show_id");

            entity.HasOne(d => d.Show).WithMany(p => p.Criteria)
                .HasForeignKey(d => d.ShowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Criteria__Show_i__5070F446");
        });

        modelBuilder.Entity<Koi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Koi__3214EC07628442F5");

            entity.ToTable("Koi");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Size).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UserId).HasColumnName("User_Id");
            entity.Property(e => e.VarietyId).HasColumnName("Variety_Id");

            entity.HasOne(d => d.User).WithMany(p => p.Kois)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Koi__User_Id__412EB0B6");

            entity.HasOne(d => d.Variety).WithMany(p => p.Kois)
                .HasForeignKey(d => d.VarietyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Koi__Variety_Id__4222D4EF");
        });

        modelBuilder.Entity<RefereeDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefereeD__3214EC0768D73FC4");

            entity.ToTable("RefereeDetail");

            entity.Property(e => e.ShowId).HasColumnName("Show_id");
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.Show).WithMany(p => p.RefereeDetails)
                .HasForeignKey(d => d.ShowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefereeDe__Show___4CA06362");

            entity.HasOne(d => d.User).WithMany(p => p.RefereeDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefereeDe__User___4BAC3F29");
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Registra__3214EC07F0992DD5");

            entity.ToTable("Registration");

            entity.Property(e => e.CreateDate).HasColumnName("Create_date");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.KoiId).HasColumnName("Koi_id");
            entity.Property(e => e.Note).HasMaxLength(300);
            entity.Property(e => e.ShowId).HasColumnName("Show_Id");
            entity.Property(e => e.Size).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalScore)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("Total_score");

            entity.HasOne(d => d.Koi).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.KoiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__Koi_i__5629CD9C");

            entity.HasOne(d => d.Show).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.ShowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__Show___5535A963");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC0707138B16");

            entity.ToTable("Role");

            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Score__3214EC075BEF711E");

            entity.ToTable("Score");

            entity.Property(e => e.CriteriaId).HasColumnName("Criteria_id");
            entity.Property(e => e.RefereeDetailId).HasColumnName("Referee_detail_id");
            entity.Property(e => e.RegistrationId).HasColumnName("Registration_Id");
            entity.Property(e => e.Score1)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("Score");

            entity.HasOne(d => d.Criteria).WithMany(p => p.Scores)
                .HasForeignKey(d => d.CriteriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Score__Criteria___5BE2A6F2");

            entity.HasOne(d => d.RefereeDetail).WithMany(p => p.Scores)
                .HasForeignKey(d => d.RefereeDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Score__Referee_d__5AEE82B9");

            entity.HasOne(d => d.Registration).WithMany(p => p.Scores)
                .HasForeignKey(d => d.RegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Score__Registrat__59FA5E80");
        });

        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Show__3214EC075D3F9B0C");

            entity.ToTable("Show");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.EntranceFee).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.RegisterEndDate).HasColumnName("Register_end_date");
            entity.Property(e => e.RegisterStartDate).HasColumnName("Register_start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Incoming");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasMany(d => d.Varieties).WithMany(p => p.Shows)
                .UsingEntity<Dictionary<string, object>>(
                    "ShowDetail",
                    r => r.HasOne<Variety>().WithMany()
                        .HasForeignKey("VarietyId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ShowDetai__Varie__47DBAE45"),
                    l => l.HasOne<Show>().WithMany()
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ShowDetai__Show___48CFD27E"),
                    j =>
                    {
                        j.HasKey("ShowId", "VarietyId");
                        j.ToTable("ShowDetail");
                        j.IndexerProperty<int>("ShowId").HasColumnName("Show_id");
                        j.IndexerProperty<int>("VarietyId").HasColumnName("Variety_Id");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0740EE1023");

            entity.ToTable("User");

            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Password).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.RoleId).HasColumnName("Role_Id");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__Role_Id__3A81B327");
        });

        modelBuilder.Entity<Variety>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Variety__3214EC079F3DA6EF");

            entity.ToTable("Variety");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
