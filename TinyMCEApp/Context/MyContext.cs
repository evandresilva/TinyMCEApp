using Microsoft.EntityFrameworkCore;
using TinyMCEApp.Models;

namespace TinyMCEApp.Context
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }

        public DbSet<TemplateModel> Templates { get; set; }
        public DbSet<DocumentModel> Documents { get; set; }
    }
}
