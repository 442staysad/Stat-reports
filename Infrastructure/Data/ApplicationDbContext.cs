using Microsoft.EntityFrameworkCore;
using Core.Entities;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<Employee> Employees { get; set; }


    }
}
