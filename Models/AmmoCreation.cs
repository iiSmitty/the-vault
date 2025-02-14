using AmmoTracker.Interfaces;
using AmmoTracker.Models;
using AmmoTracker.Services;

namespace AmmoTracker.Models
{
    /// <summary>
    /// Handles the creation of new ammunition records with user input.
    /// </summary>
    public class AmmoCreation
    {
        private readonly IAmmoService _ammoService;

        public AmmoCreation(IAmmoService ammoService)
        {
            _ammoService = ammoService ?? throw new ArgumentNullException(nameof(ammoService));
        }

        /// <summary>
        /// Collects user input and creates a new ammunition record.
        /// </summary>
        public async Task CreateNewAmmoAsync()
        {
            try
            {
                TypeEffect("\nEntering Ammunition Creation Interface...");
                await Task.Delay(1000); // Fallout-style delay

                var ammo = await CollectAmmoDetails();
                if (ammo != null)
                {
                    await SaveAmmoRecord(ammo);
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

            await WaitForKeyPress();
        }

        /// <summary>
        /// Collects and validates ammunition details from user input.
        /// </summary>
        private async Task<Ammo?> CollectAmmoDetails()
        {
            try
            {
                TypeEffect("\nPlease enter ammunition details:");

                var caliber = GetValidInput("\nCaliber: ", ValidateRequired);
                var brand = GetValidInput("\nBrand: ", ValidateRequired);
                var quantity = GetValidInput<int>("\nQuantity: ", ValidateQuantity);
                var price = GetValidInput<decimal>("\nPrice (R): ", ValidatePrice);
                var purchaseDate = GetValidInput<DateTime>(
                    "\nPurchase Date (yyyy-MM-dd): ",
                    ValidateDate
                );

                var ammo = new Ammo
                {
                    Caliber = caliber,
                    Brand = brand,
                    Quantity = quantity,
                    Price = price,
                    PurchaseDate = purchaseDate,
                };

                if (await ConfirmDetails(ammo))
                {
                    return ammo;
                }

                return null;
            }
            catch (OperationCanceledException)
            {
                TypeEffect("\nAmmunition creation cancelled.");
                return null;
            }
        }

        /// <summary>
        /// Validates and gets user input with retry capability.
        /// </summary>
        private T GetValidInput<T>(string prompt, Func<T, bool> validator)
        {
            while (true)
            {
                TypeEffect(prompt);
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input) || input.ToLower() == "cancel")
                {
                    throw new OperationCanceledException();
                }

                if (TryParseInput(input, out T result) && validator(result))
                {
                    return result;
                }

                TypeEffect("\nInvalid input. Please try again or type 'cancel' to abort.");
            }
        }

        private string GetValidInput(string prompt, Func<string, bool> validator)
        {
            while (true)
            {
                TypeEffect(prompt);
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input) || input.ToLower() == "cancel")
                {
                    throw new OperationCanceledException();
                }

                if (validator(input))
                {
                    return input;
                }

                TypeEffect("\nInvalid input. Please try again or type 'cancel' to abort.");
            }
        }

        /// <summary>
        /// Validates required string input.
        /// </summary>
        private bool ValidateRequired(string input) => !string.IsNullOrWhiteSpace(input);

        /// <summary>
        /// Validates quantity input.
        /// </summary>
        private bool ValidateQuantity(int quantity) => quantity > 0;

        /// <summary>
        /// Validates price input.
        /// </summary>
        private bool ValidatePrice(decimal price) => price >= 0;

        /// <summary>
        /// Validates date input.
        /// </summary>
        private bool ValidateDate(DateTime date) => date <= DateTime.Now;

        /// <summary>
        /// Displays ammunition details and asks for confirmation.
        /// </summary>
        private async Task<bool> ConfirmDetails(Ammo ammo)
        {
            TypeEffect("\nPlease confirm ammunition details:");
            var details = new[]
            {
                ("Caliber", ammo.Caliber),
                ("Brand", ammo.Brand),
                ("Quantity", ammo.Quantity.ToString()),
                ("Price", $"R{ammo.Price:F2}"),
                ("Price Per Round", $"R{ammo.PricePerRound:F2}"),
                ("Purchase Date", ammo.PurchaseDate.ToString("d")),
            };

            for (int i = 0; i < details.Length; i++)
            {
                var (label, value) = details[i];
                var prefix = new string(' ', i * 3) + "└─ ";
                TypeEffect($"\n{prefix}{label}: {value}");
            }

            TypeEffect("\n\nConfirm details? (Y/N): ");
            var key = Console.ReadKey(intercept: true);
            return key.Key == ConsoleKey.Y;
        }

        /// <summary>
        /// Saves the ammunition record to the database.
        /// </summary>
        private async Task SaveAmmoRecord(Ammo ammo)
        {
            string? connectionString =
                Environment.GetEnvironmentVariable("SUPABASE_CONNECTION_STRING")
                ?? throw new InvalidOperationException(
                    "SUPABASE_CONNECTION_STRING environment variable not found."
                );

            await _ammoService.InsertAmmoAsync(connectionString, ammo);
            TypeEffect("\nAmmunition record created successfully!");
        }

        private void HandleError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            TypeEffect($"\nError creating ammunition record: {ex.Message}");
            Console.ResetColor();
        }

        private async Task WaitForKeyPress()
        {
            TypeEffect("\nPress any key to return to the main menu...");
            await Task.Run(() => Console.ReadKey(intercept: true));
        }

        private bool TryParseInput<T>(string input, out T result)
        {
            try
            {
                result = (T)Convert.ChangeType(input, typeof(T));
                return true;
            }
            catch
            {
                result = default!;
                return false;
            }
        }

        private void TypeEffect(string text)
        {
            // You'll need to implement this or use your existing TypeEffect method
            Console.WriteLine(text);
        }
    }
}
