using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SampleCRUDApi.Controllers;
using SampleCRUDApi.Data;

namespace Tests.Controllers
{
    public class CompaniesControllersTests
    {
        private CompanyEntity DefaultEntry => new CompanyEntity
        {
            Id = 1,
            Name = "Default",
            CreatedBy = 1
        };
        
        private AppDbContext _dbContext;
        
        [SetUp]
        public void Setup()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
 
            var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
            _dbContext = new AppDbContext(options);
            _dbContext.Database.EnsureDeleted();  
            _dbContext.Database.EnsureCreated();
        }

        [Test]
        public async Task Put_CreatedByDoesNotMatch_ReturnsForbid()
        {
            // Arrange
            FillTestEntry(createdBy: DefaultEntry.CreatedBy + 1);
            var testingObject = new CompaniesController(_dbContext);
            
            // Act
            var result = await testingObject.Put(DefaultEntry.Id, new CompaniesController.PutBindModel());
            
            // Assert
            var forbidResult = result as ForbidResult;
            Assert.That(forbidResult, Is.Not.Null);
        }
        
        [Test]
        public async Task Put_InputCorrect_UpdatesEntry()
        {
            // Arrange
            var id = 1;
            var bindModel = new CompaniesController.PutBindModel
            {
                TenantId = 1,
                Name = "Updated"
            };
            FillTestEntry(id);
            var testingObject = new CompaniesController(_dbContext);
            
            // Act
            var result = await testingObject.Put(id, bindModel);
            
            // Assert
            var jsonResult = result as JsonResult;
            Assert.That(jsonResult, Is.Not.Null);
            
            var companyResult = jsonResult.Value as CompanyEntity;
            Assert.That(companyResult, Is.Not.Null);
            Assert.That(companyResult.Name, Is.EqualTo(bindModel.Name));
        }

        private void FillTestEntry(int? id = null, int? createdBy = null)
        {
            var entry = DefaultEntry;
            if (id != null) entry.Id = (int)id;
            if (createdBy != null) entry.CreatedBy = (int)createdBy;
            _dbContext.Add(entry);
            _dbContext.SaveChanges();
        }
    }
}