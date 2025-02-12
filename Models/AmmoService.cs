using Npgsql;

namespace AmmoTracker.Models
{
    /// <summary>
    /// Provides implementation for ammunition data management operations
    /// </summary>
    public class AmmoService : IAmmoService
    {
        /// <inheritdoc/>
        public async Task InsertAmmoAsync(string connectionString, Ammo ammo)
        {
            const string insertQuery =
                @"
                INSERT INTO ammo (brand, caliber, quantity, price, purchase_date)
                VALUES (@brand, @caliber, @quantity, @price, @purchaseDate);
            ";

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@brand", ammo.Brand);
            command.Parameters.AddWithValue("@caliber", ammo.Caliber);
            command.Parameters.AddWithValue("@quantity", ammo.Quantity);
            command.Parameters.AddWithValue("@price", ammo.Price);
            command.Parameters.AddWithValue("@purchaseDate", ammo.PurchaseDate);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"{rowsAffected} row(s) inserted successfully!");
        }

        /// <inheritdoc/>
        public decimal CalculatePricePerCartridge(decimal price, int quantity)
        {
            if (quantity == 0)
            {
                throw new InvalidOperationException(
                    "Quantity cannot be zero when calculating price per cartridge."
                );
            }
            return price / quantity;
        }
    }
}
