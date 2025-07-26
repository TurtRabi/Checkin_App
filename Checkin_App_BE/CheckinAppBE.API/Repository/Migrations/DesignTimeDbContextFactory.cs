using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository.Migrations
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TravelCardsDBContext>
    {
        public TravelCardsDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TravelCardsDBContext>();
            optionsBuilder.UseSqlServer("Server=(local); uid=sa; pwd=Tranleminh305@; database=TravelCardsDB; TrustServerCertificate=True");

            return new TravelCardsDBContext(optionsBuilder.Options);
        }
    }
}
