using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PowerBot.Data.Models;

namespace PowerBot.Data;

public partial class PowerDbContext : DbContext
{
    public PowerDbContext()
    {
    }

    public PowerDbContext(DbContextOptions<PowerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Meow> Meow { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<NWord> NWords { get; set; }    
    public virtual DbSet<BestBoi> BestBoi { get; set; }
    public virtual DbSet<BestGirl> BestGirl { get; set; }
    public virtual DbSet<UserAct> UserActs { get; set; }
    public virtual DbSet<McConnect> McConnects { get; set; }
    public virtual DbSet<TimeCard> TimeCards { get; set; }
    public virtual DbSet<shoGirls> ShoGirls { get; set; }
    public virtual DbSet<Selfie> Selfies { get; set; }  
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=OOJIND\\DISCORDBOTSERVER;Initial Catalog=TimerBot;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BestBoi>(entity =>
        {
            entity.Property(e => e.Number).ValueGeneratedOnAdd();
        });
        modelBuilder.Entity<BestGirl>(entity =>
        {
            entity.Property(e => e.Number).ValueGeneratedOnAdd();
        });
        modelBuilder.Entity<Selfie>(entity =>
        {
            entity.Property(e => e.Number).ValueGeneratedOnAdd();
        });
        modelBuilder.Entity<TimeCard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TimeCard");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
