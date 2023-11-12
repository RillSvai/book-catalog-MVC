using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using BookCatalogWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogWeb.Tests.Controller
{
    public class CategoryControllerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CategoryController _categoryController;

        public CategoryControllerTests()
        {
            //Dependencies
            _unitOfWork = A.Fake<IUnitOfWork>();

            //SUT
            _categoryController = new CategoryController(_unitOfWork);
        }

        [Fact]
        public void CategoryController_Index_ReturnsSuccess() 
        {
            //Arrange
            List<Category> categories = A.Fake<List<Category>>();
            categories.Add(new Category { Id = 1, Name = "Test", DisplayOrder = 1 });
            A.CallTo(() => _unitOfWork.CategoryRepo!.GetAll()).Returns(categories);

            //Act
            var result = _categoryController.Index();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public void CategoryController_Create_ReturnsSuccess() 
        {
            //Arrange
            Category category = A.Fake<Category>();
            category.Name = "Test";
            category.Id = 0;
            category.DisplayOrder = 1;
            A.CallTo(() => _unitOfWork.CategoryRepo!.Add(category));
            _categoryController.TempData = new TempDataDictionary(A.Fake<HttpContext>(), A.Fake<ITempDataProvider>());
            //Act
            var result = _categoryController.Create(category);
            
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToActionResult>();
        }

    }
}
