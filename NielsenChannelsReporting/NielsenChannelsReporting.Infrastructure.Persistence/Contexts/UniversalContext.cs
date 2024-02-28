using Microsoft.EntityFrameworkCore;
using NielsenChannelsReporting.Application.Exceptions;
using NielsenChannelsReporting.Infrastructure.Persistence.Models;

namespace NielsenChannelsReporting.Infrastructure.Persistence.Contexts
{
    public partial class UniversalContext : DbContext
    {
        public UniversalContext()
        {
        }

        public UniversalContext(DbContextOptions<UniversalContext> options)
            : base(options)
        {
        }


        public virtual DbSet<ChannelSet> ChannelSets { get; set; } = null!;

        public virtual DbSet<NielsenChannelsReportLog> NielsenChannelsReportLogs { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                throw new PersistenceException("OptionsBuilder is not configured or the connection string is unsupported");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");


            modelBuilder.Entity<ChannelSet>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ChannelSet", "Source");

                entity.Property(e => e.Channels).HasColumnName("channels");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasDefaultValueSql("('8/13/2021 12:00:00')");

                entity.Property(e => e.Environment)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("environment");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Locked).HasColumnName("locked");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Publisheddate).HasColumnName("publisheddate");
            });


            modelBuilder.Entity<NielsenChannelsReportLog>(entity =>
            {
                entity.ToTable("NielsenChannelsReportLog", "dbo");

                entity.Property(e => e.Detail).HasColumnType("text");

                entity.Property(e => e.ProcessDate).HasColumnType("datetime");

                entity.Property(e => e.Environment).HasColumnType("nvarchar(20)");
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
