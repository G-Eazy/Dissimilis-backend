using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.WebAPI.Services;
using Xunit;

namespace Dissimilis.WebAPI.xUnit.Setup
{
    internal class TestAuthService : IAuthService, IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _testServerFixture;
        private readonly DissimilisDbContext _dbContext;

        public TestAuthService(TestServerFixture testServerFixture)
        {
            _testServerFixture = testServerFixture;
            _dbContext = (DissimilisDbContext)_testServerFixture.GetServiceProvider().GetService(typeof(DissimilisDbContext));
        }

        public int? GetCurrentUserId()
        {
            return TestServerFixture.CurrentUserId;
        }

        public User GetVerifiedCurrentUser()
        {
            return _dbContext.Users.FirstOrDefault(u => u.Id == TestServerFixture.CurrentUserId);
        }
    }
}
