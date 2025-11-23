using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;

namespace BlazorGame.Tests
{
    public class TestBase
    {
        protected ApplicationDbContext CreateInMemoryContext([CallerMemberName] string testName = "")
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{testName}_{Guid.NewGuid()}")
                .Options;
            return new ApplicationDbContext(options);
        }
    }
}