using System;
using Dissimilis.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Dissimilis.DbContext
{
    public class DissimilisDbContextFactory : IDesignTimeDbContextFactory<DissimilisDbContext>
    {
        private readonly DbContextOptions<DissimilisDbContext> _options;
        private readonly string _connectionString;
        private const string ConnectionStringKey = "SQL_CONNECTION_STRING";

#if DEBUG
        public DissimilisDbContextFactory()
        {
            _connectionString = ConfigurationInfo.GetSqlConnectionString();
            _options = CreateOptions();
        }
#endif


        public DissimilisDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
            _options = CreateOptions();
        }


        public DissimilisDbContextFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection(ConnectionStringKey)?.Value;
            _options = CreateOptions();
        }

        private DbContextOptions<DissimilisDbContext> CreateOptions()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ApplicationException($"Connection string {ConnectionStringKey} not set.");
            }

            return new DbContextOptionsBuilder<DissimilisDbContext>()
                .UseSqlServer(_connectionString, optionsBuilder =>
                {
                    optionsBuilder.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    optionsBuilder.CommandTimeout(10000);
                }).Options;
        }

        public DissimilisDbContext CreateDbContext(params string[] args) =>
            new DissimilisDbContext(_options);
    }
}
