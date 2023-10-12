using BookCatalogWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCatalogWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) 
        {
                
        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(new Category[]
            {
                new () {Id = 1,DisplayOrder = 1, Name = "Action" },
                new () {Id = 2,DisplayOrder = 2, Name = "SciFi" },
                new () {Id = 4,DisplayOrder = 4, Name = "History"},
                new () {Id = 3, DisplayOrder = 3, Name = "Horror"}
            });
        }
    }
}
