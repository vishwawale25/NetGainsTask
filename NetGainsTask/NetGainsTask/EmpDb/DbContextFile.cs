using Microsoft.EntityFrameworkCore;
using NetGainsTask.Models;

namespace NetGainsTask.EmpDb
{
    public class DbContextFile : DbContext
    {
        public DbContextFile(DbContextOptions options) : base(options)
        {
        }

        protected DbContextFile()
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
