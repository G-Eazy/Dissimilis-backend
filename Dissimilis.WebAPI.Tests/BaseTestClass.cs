using System;
using System.Collections.Generic;
using System.Text;
using Dissimilis.DbContext;
using Dissimilis.WebAPI.xUnit.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dissimilis.WebAPI.xUnit
{
    public class BaseTestClass : IClassFixture<TestServerFixture>
    {
        internal readonly TestServerFixture _testServerFixture;

        public BaseTestClass(TestServerFixture testServerFixture)
        {
            _testServerFixture = testServerFixture;
        }
    }
}
