using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dissimilis.ConsoleApp.Database
{
    class DissimilisDbContextOptions : DbContextOptionsBuilder
	{
		public DissimilisDbContextOptions() {
            
            this.UseSqlServer("(localdb)\\mssqllocaldb; Database=DissimilisDB;Trusted_Connection=True;ConnectRetryCount=0");
        }

	}

}

