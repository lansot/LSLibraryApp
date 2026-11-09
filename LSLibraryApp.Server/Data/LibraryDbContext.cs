using LSLibraryApp.Server.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace LSLibraryApp.Server.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Topic> Topics { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookReference> BookReferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Topics
            modelBuilder.Entity<Topic>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(t => t.Body);
                entity.Property(t => t.CreatedAt)
                    .IsRequired();
                entity.Property(t => t.UpdatedAt);
                entity.Property(t => t.Version)
                    .IsConcurrencyToken();

                entity.OwnsMany(t => t.Tags);

                entity.HasMany(t => t.BookReferences)
                    .WithOne(br => br.Topic)
                    .HasForeignKey(br => br.TopicId)
                    // Probably not ideal for a topic deletion to massacre a bunch of book contents, but we can figure out how this should work later.
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Books
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(b => b.Description);
                entity.Property(b => b.CreatedAt)
                    .IsRequired();

                entity.HasMany(b => b.Contents)
                    .WithOne(br => br.Book)
                    .HasForeignKey(br => br.BookId)
                    .OnDelete(DeleteBehavior.Cascade); // Makes sense for a book deletion to also delete its contents
            });

            // Configure BookReferences
            modelBuilder.Entity<BookReference>(entity =>
            {
                entity.HasKey(br => br.Id);
                entity.Property(br => br.BookId)
                    .IsRequired();
                entity.Property(br => br.TopicId)
                    .IsRequired();
                entity.Property(br => br.Level)
                    .IsRequired();
                entity.Property(br => br.Position)
                    .IsRequired();
            });
        }
    }
}
