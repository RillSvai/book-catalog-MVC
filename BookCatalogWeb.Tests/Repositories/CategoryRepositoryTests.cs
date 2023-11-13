using BookCatalog.DataAccess.Data;
using BookCatalog.DataAccess.Repository;
using BookCatalog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogWeb.Tests.Repositories
{
    public class CategoryRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            ApplicationDbContext db = new ApplicationDbContext(options);
            db.Database.EnsureCreated();
            if (!await db.Categories.AnyAsync()) 
            {
                for (int i = 0; i < 10; i++)
                {

                    Category c1 = new Category()
                    {
                        Name = $"Category {i+1}",
                        DisplayOrder = i+1,
                    };
                    await db.Categories.AddAsync(c1);
                    await db.SaveChangesAsync();

                }
            }
            return db;
        }

        [Fact]
        public async void CategoryRepository_Add_ReturnsVoid() 
        {
            //Arrange
            Category c = new Category()
            {
                Name = $"Test",
                DisplayOrder = 121,
            };
            ApplicationDbContext db = await GetDbContextAsync();
            CategoryRepository categoryRepo = new CategoryRepository(db);
            //Act + Assert
            try
            {
                categoryRepo.Add(c);
                return;
            }
            catch (Exception ex) 
            {
                Assert.Fail(ex.Message);
            }

            

        }

        [Fact]
        public async void CategoryRepository_Get_ReturnsCategory() 
        {
            //Arrange
            int id = 3;
            ApplicationDbContext db = await GetDbContextAsync();
            CategoryRepository categoryRepo = new CategoryRepository(db);
            
            //Act

            Category? result = categoryRepo.Get(c => c.Id == id);

            //Assert

            result.Should().NotBeNull();
            result!.DisplayOrder.Should().Be(3);
            result.Name.Should().BeEquivalentTo("adventure");
        }
    }
}
