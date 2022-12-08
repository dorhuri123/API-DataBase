using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using APi_DataBase.modals;

namespace APi_DataBase.Data
{
    public class APi_DataBaseContext : DbContext
    {
        public APi_DataBaseContext (DbContextOptions<APi_DataBaseContext> options)
            : base(options)
        {
        }

        public DbSet<APi_DataBase.modals.projects> projects { get; set; } = default!;
    }
}
