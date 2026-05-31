using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TFGBack._02_Domain.Infrastructure.Contracts.Models;
using TFGBack._03_Infrastructure.Models;

namespace TFGBack._03_Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
            public DbSet<UserRepository> Users { get; set; }
            public DbSet<BoardRepository> Boards { get; set; }
            public DbSet<TaskRepository> Tasks { get; set; }
            public DbSet<UserFollowRepository> UserFollows { get; set; }
            public DbSet<BoardCommentRepository> BoardComments { get; set; }
    }
}
