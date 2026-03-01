using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public abstract class BaseSeeder : ISeeder
    {
        public abstract int Order { get; }

        public abstract void Seed(ModelBuilder modelBuilder);
    }
}
