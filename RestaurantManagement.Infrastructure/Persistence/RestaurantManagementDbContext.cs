using Microsoft.EntityFrameworkCore;

namespace RestaurantManagement.Infrastructure.Persistence;

public class RestaurantManagementDbContext : DbContext
{
    public RestaurantManagementDbContext(DbContextOptions<RestaurantManagementDbContext> options) : base(options) { }



    //Cấu hình fluent api
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
