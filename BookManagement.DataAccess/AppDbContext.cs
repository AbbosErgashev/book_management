using BookManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
}
