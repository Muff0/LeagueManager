using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class QueueContext : DbContext
    {

        public QueueContext(DbContextOptions<QueueContext> options) : base(options) { }

        public DbSet<CommandMessage> CommandQueue { get; set; }
        public DbSet<DomainEvent> EventQueue { get; set; }
        public DbSet<OutgoingMessage> MessageQueue { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CommandMessage>()
                .HasIndex(cm => cm.Status);

            modelBuilder.Entity<DomainEvent>()
                .HasIndex(de => de.Status);

            modelBuilder.Entity<OutgoingMessage>()
                .HasIndex(om => om.Status);
        }
    }
}
