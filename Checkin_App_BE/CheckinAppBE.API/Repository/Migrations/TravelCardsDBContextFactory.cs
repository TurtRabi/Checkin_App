using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository.Models;
using System;

namespace Repository.Migrations
{
    public class TravelCardsDBContextFactory : IDesignTimeDbContextFactory<TravelCardsDBContext>
    {
        public TravelCardsDBContext CreateDbContext(string[] args)
        {
            // Chuỗi kết nối cứng, bạn có thể chỉnh sửa trực tiếp ở đây
            var connectionString = @"Server=(local);Database=TravelCardsDB;User Id=sa;Password=Tranleminh305@;TrustServerCertificate=True;";

            // In ra hash code chuỗi kết nối để debug (không in ra chuỗi thật tránh lộ mật khẩu)
            Console.WriteLine($"Connection string (hash): {connectionString.GetHashCode()}");

            var optionsBuilder = new DbContextOptionsBuilder<TravelCardsDBContext>();

            // Cấu hình DbContext sử dụng SQL Server với connection string trên
            optionsBuilder.UseSqlServer(connectionString);

            // Trả về instance DbContext mới
            return new TravelCardsDBContext(optionsBuilder.Options);
        }
    }
}
