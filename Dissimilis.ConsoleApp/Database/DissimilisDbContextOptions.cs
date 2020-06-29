using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.ConsoleApp.Database
{
    class DissimilisDbContextOptions : DbContextOptionsBuilder
	{
		public DissimilisDbContextOptions()
		{
			var conn = "Server=(localdb)\\mssqllocaldb;Database=DissimilisDB;Trusted_Connection=True;ConnectRetryCount=0";

			this.UseSqlServer(conn);
		}
	}

}

