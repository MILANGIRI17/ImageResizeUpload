using ImageResizeUpload.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageResizeUpload.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Artist> Artists { get; set; }
       


   
    }

    
}
