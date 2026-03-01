using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public interface ISeeder
    {
        /// <summary>
        /// Controls the order in which seeders execute. Lower numbers run first.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Seed data via HasData calls on the ModelBuilder.
        /// </summary>
        void Seed(ModelBuilder modelBuilder);
    }
}
