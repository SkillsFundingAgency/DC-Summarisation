﻿using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Model
{
    public partial class SummarisationContext : DbContext
    {
        public SummarisationContext()
        {
        }

        public SummarisationContext(DbContextOptions<SummarisationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CollectionReturn> CollectionReturns { get; set; }
        public virtual DbSet<ESF_FundingData> ESF_FundingDatas { get; set; }
        public virtual DbSet<SummarisedActual> SummarisedActuals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=(local);Database=SummarisedActuals;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<CollectionReturn>(entity =>
            {
                entity.ToTable("CollectionReturn");

                entity.HasIndex(e => new { e.CollectionType, e.CollectionReturnCode })
                    .HasName("UK_CollectionReturn")
                    .IsUnique();

                entity.Property(e => e.CollectionReturnCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CollectionType)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<ESF_FundingData>(entity =>
            {
                entity.HasKey(e => new { e.UKPRN, e.ConRefNumber, e.DeliverableCode, e.AttributeName, e.CollectionYear, e.CollectionPeriod })
                    .HasName("PK__ESF_Fund__F8AAD8A3185C2C42");

                entity.ToTable("ESF_FundingData");

                entity.Property(e => e.ConRefNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DeliverableCode)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.AttributeName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Period_1).HasColumnType("decimal(15, 5)");

                entity.Property(e => e.Period_10).HasColumnType("decimal(15, 5)");

                entity.Property(e => e.Period_11).HasColumnType("decimal(15, 5)");

                entity.Property(e => e.Period_12).HasColumnType("decimal(15, 5)");

                entity.Property(e => e.Period_2).HasColumnType("decimal(15, 5)");

                entity.Property(e => e.Period_3).HasColumnType("decimal(15, 5)");

                entity.Property(e => e.Period_4).HasColumnType("decimal(15, 5)");

                entity.Property(e => e.Period_5).HasColumnType("decimal(15, 5)");

                entity.Property(e => e.Period_6).HasColumnType("decimal(15, 5)");

                entity.Property(e => e.Period_7).HasColumnType("decimal(15, 5)");

                entity.Property(e => e.Period_8).HasColumnType("decimal(15, 5)");

                entity.Property(e => e.Period_9).HasColumnType("decimal(15, 5)");
            });

            modelBuilder.Entity<SummarisedActual>(entity =>
            {
                entity.Property(e => e.ActualValue).HasColumnType("decimal(13, 2)");

                entity.Property(e => e.ContractAllocationNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FundingStreamPeriodCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.OrganisationId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PeriodTypeCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.UoPCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.CollectionReturn)
                    .WithMany(p => p.SummarisedActuals)
                    .HasForeignKey(d => d.CollectionReturnId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SummarisedActuals_CollectionReturn");
            });
        }
    }
}
