using HR_Recruitment.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HR_Recruitment.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

    }
}