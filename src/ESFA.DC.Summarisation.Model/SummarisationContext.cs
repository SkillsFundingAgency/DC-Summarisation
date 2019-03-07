using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
        public virtual DbSet<SummarisedActual> SummarisedActuals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\;Database=ESFA.DC.Summarisation.Database;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<CollectionReturn>(entity =>
            {
                entity.ToTable("CollectionReturn");

                entity.Property(e => e.CollectionReturnCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CollectionType)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SummarisedActual>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

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

                entity.Property(e => e.UoPcode)
                    .HasColumnName("UoPCode")
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
