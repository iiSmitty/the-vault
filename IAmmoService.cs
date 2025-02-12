namespace AmmoTracker.Models
{
    /// <summary>
    /// Defines operations for managing ammunition data
    /// </summary>
    public interface IAmmoService
    {
        /// <summary>
        /// Inserts a new ammunition record into the database
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="ammo">Ammunition data to insert</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task InsertAmmoAsync(string connectionString, Ammo ammo);

        /// <summary>
        /// Calculates the price per individual cartridge
        /// </summary>
        /// <param name="price">Total price of ammunition</param>
        /// <param name="quantity">Number of cartridges</param>
        /// <returns>Price per cartridge</returns>
        /// <exception cref="InvalidOperationException">Thrown when quantity is zero</exception>
        decimal CalculatePricePerCartridge(decimal price, int quantity);
    }
}
