using System.IO;
using AmmoTracker.Interfaces;
using AmmoTracker.Models;
using AmmoTracker.Services;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AmmoTracker
{
    class Program
    {
        /// <summary>
        /// Entry point for the Pip-Boy application
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task Main(string[] args)
        {
            try
            {
                DotNetEnv.Env.Load();
                Console.Title = "Pip-Boy 3000";
                SetupConsole();

                ShowLoadingScreen();
                await ShowMenuAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error: {ex.Message}");
                Thread.Sleep(2000); // Give user time to see error
            }
            finally
            {
                Console.ResetColor();
                Console.Clear();
            }
        }

        /// <summary>
        /// Configures initial console settings
        /// </summary>
        private static void SetupConsole()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            if (OperatingSystem.IsWindows())
            {
                try
                {
                    Console.SetWindowSize(120, 30);
                    Console.SetBufferSize(120, 30);
                }
                catch (Exception ex)
                    when (ex is ArgumentOutOfRangeException || ex is PlatformNotSupportedException)
                {
                    // Ignore if window size cannot be set or if running in an unsupported environment
                }
            }
        }

        /// <summary>
        /// Displays the Vault-Tec ASCII art logo
        /// </summary>
        private static void LoadLogo()
        {
            const string PIPBOY_LOGO =
                @"
___  ___         _   _                _  _   
|  \/  |        | | | |              | || |  
| .  . | _   _  | | | |  __ _  _   _ | || |_ 
| |\/| || | | | | | | | / _` || | | || || __|
| |  | || |_| | \ \_/ /| (_| || |_| || || |_ 
\_|  |_/ \__, |  \___/  \__,_| \__,_||_| \__|
          __/ |                              
         |___/                               
";

            try
            {
                Console.Clear(); // Clear screen before displaying logo
                Console.WriteLine(PIPBOY_LOGO);
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error displaying logo: " + ex.Message);
            }
        }

        /// <summary>
        /// Displays the Pip-Boy boot sequence with animations and system messages
        /// </summary>
        private static void ShowLoadingScreen()
        {
            const int SYSTEM_PAUSE = 2000;

            try
            {
                LoadLogo();

                // Display creator credits
                TypeEffect("Coded by André Smit", delay: 75);

                // Show system initialization
                LoadingAnimation("Initializing Pip-Boy", duration: 4000);

                // Display welcome messages
                TypeEffect("Welcome, Vault Dweller.", delay: 100);
                TypeEffect("System Diagnostics: All Systems Operational.", delay: 60);

                Thread.Sleep(SYSTEM_PAUSE);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during boot sequence: {ex.Message}");
                Thread.Sleep(1000); // Give user time to see error
            }
        }

        /// <summary>
        /// Displays and handles the main menu interaction
        /// </summary>
        private static async Task ShowMenuAsync()
        {
            while (true)
            {
                try
                {
                    DisplayMenu();
                    var choice = GetUserChoice();

                    if (await HandleMenuChoice(choice))
                        break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    await Task.Delay(2000);
                }
            }
        }

        /// <summary>
        /// Displays the main menu options
        /// </summary>
        private static void DisplayMenu()
        {
            Console.Clear();
            LoadLogo();
            TypeEffect("=== Main Menu ===");
            Console.WriteLine("[0] Create Test Ammo");
            Console.WriteLine("[1] Create New Ammo");
            Console.WriteLine("[2] View Ammo Details");
            Console.WriteLine("[3] Exit");
        }

        /// <summary>
        /// Gets and validates user input
        /// </summary>
        private static string GetUserChoice()
        {
            Console.Write("Enter your choice: ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Handles the user's menu selection
        /// </summary>
        /// <param name="choice">The user's menu choice</param>
        /// <returns>True if the application should exit, false otherwise</returns>
        private static async Task<bool> HandleMenuChoice(string choice)
        {
            switch (choice)
            {
                case "0":
                    await CreateTestAmmo();
                    return false;

                case "1":
                    await CreateNewAmmo();
                    return false;

                case "2":
                    await ViewAmmoDetails();
                    return false;

                case "3":
                    await ExitApplication();
                    return true;

                default:
                    DisplayInvalidOption();
                    return false;
            }
        }

        /// <summary>
        /// Handles the ammo creation process
        /// </summary>
        private static async Task CreateTestAmmo()
        {
            Console.Clear();
            TypeEffect("Creating test ammo...");
            await TestAmmoCreationAsync();
        }

        /// <summary>
        /// Handles the ammo viewing process
        /// </summary>
        private static async Task ViewAmmoDetails()
        {
            Console.Clear();
            TypeEffect("Enter Ammo ID to view: ");

            if (int.TryParse(Console.ReadLine(), out int ammoId))
            {
                await ViewAmmoDetailsAsync(ammoId);
            }
            else
            {
                TypeEffect("Invalid ID format. Press any key to continue...");
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Handles the application exit process
        /// </summary>
        private static async Task ExitApplication()
        {
            Console.Clear();
            TypeEffect("Exiting Pip-Boy. Stay safe, Vault Dweller!");
            await Task.Delay(2000);
        }

        /// <summary>
        /// Displays invalid option message
        /// </summary>
        private static void DisplayInvalidOption()
        {
            Console.WriteLine("Invalid option. Please try again.");
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Displays a loading animation with customizable message and duration
        /// </summary>
        /// <param name="message">The message to display during loading</param>
        /// <param name="duration">Total duration in milliseconds (default: 3000)</param>
        /// <param name="dotInterval">Interval between dots in milliseconds (default: 500)</param>
        public static void LoadingAnimation(
            string message,
            int duration = 3000,
            int dotInterval = 500
        )
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            if (duration <= 0)
                throw new ArgumentException("Duration must be positive", nameof(duration));

            if (dotInterval <= 0)
                throw new ArgumentException("Dot interval must be positive", nameof(dotInterval));

            Console.Write(message);

            using var cts = new CancellationTokenSource(duration);
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    Console.Write(".");
                    Thread.Sleep(dotInterval);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal termination, no action needed
            }

            Console.WriteLine(" Done!");
        }

        /// <summary>
        /// Creates a typewriter effect for the given message
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="delay">Delay between characters in milliseconds (default: 50)</param>
        /// <param name="newLine">Whether to add a new line at the end (default: true)</param>
        public static void TypeEffect(string message, int delay = 50, bool newLine = true)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            if (delay < 0)
                throw new ArgumentException("Delay must be non-negative", nameof(delay));

            foreach (char c in message)
            {
                Console.Write(c);
                if (delay > 0)
                {
                    Thread.Sleep(delay);
                }
            }

            if (newLine)
            {
                Console.WriteLine();
            }
        }

        public class AmmoService : IAmmoService
        {
            /// <inheritdoc/>
            public async Task InsertAmmoAsync(string connectionString, Ammo ammo)
            {
                if (string.IsNullOrEmpty(connectionString))
                    throw new ArgumentNullException(nameof(connectionString));

                if (ammo == null)
                    throw new ArgumentNullException(nameof(ammo));

                const string sql =
                    @"
            INSERT INTO ammo (brand, caliber, quantity, price, purchase_date)
            VALUES (@Brand, @Caliber, @Quantity, @Price, @PurchaseDate)";

                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Brand", ammo.Brand);
                command.Parameters.AddWithValue("@Caliber", ammo.Caliber);
                command.Parameters.AddWithValue("@Quantity", ammo.Quantity);
                command.Parameters.AddWithValue("@Price", ammo.Price);
                command.Parameters.AddWithValue("@PurchaseDate", ammo.PurchaseDate);

                await command.ExecuteNonQueryAsync();
            }

            /// <inheritdoc/>
            public decimal CalculatePricePerCartridge(decimal price, int quantity)
            {
                if (quantity <= 0)
                {
                    throw new ArgumentException(
                        "Quantity must be greater than zero when calculating price per cartridge.",
                        nameof(quantity)
                    );
                }

                if (price < 0)
                {
                    throw new ArgumentException("Price cannot be negative.", nameof(price));
                }

                return price / quantity;
            }
        }

        /// <summary>
        /// Creates a test ammunition entry using configuration settings
        /// </summary>
        private static async Task TestAmmoCreationAsync()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    optional: true
                )
                .AddEnvironmentVariables()
                .Build();

            var ammoService = new AmmoService();
            var ammoTest = new AmmoTest(configuration, ammoService);
            await ammoTest.CreateAmmoAsync();
        }

        /// <summary>
        /// Handles the creation and management of test ammunition entries.
        /// </summary>
        public class AmmoTest
        {
            private readonly IConfiguration _configuration;
            private readonly IAmmoService _ammoService;

            /// <summary>
            /// Initializes a new instance of the AmmoTest class.
            /// </summary>
            /// <param name="configuration">The configuration interface for accessing app settings.</param>
            /// <param name="ammoService">The service interface for ammunition operations.</param>
            /// <exception cref="ArgumentNullException">Thrown when configuration or ammoService is null.</exception>
            public AmmoTest(IConfiguration configuration, IAmmoService ammoService)
            {
                _configuration =
                    configuration ?? throw new ArgumentNullException(nameof(configuration));
                _ammoService = ammoService ?? throw new ArgumentNullException(nameof(ammoService));
            }

            /// <summary>
            /// Creates a test ammunition entry in the database.
            /// </summary>
            /// <returns>A task representing the asynchronous operation.</returns>
            /// <exception cref="InvalidOperationException">Thrown when the connection string is not found.</exception>
            public async Task CreateAmmoAsync()
            {
                string? connectionString =
                    Environment.GetEnvironmentVariable("SUPABASE_CONNECTION_STRING")
                    ?? throw new InvalidOperationException(
                        "SUPABASE_CONNECTION_STRING environment variable not found."
                    );

                var testAmmo = CreateTestAmmo();
                try
                {
                    await _ammoService.InsertAmmoAsync(connectionString, testAmmo);
                    PrintSuccess(testAmmo);
                }
                catch (Exception ex)
                {
                    PrintError(ex);
                }
                await WaitForKeyPress();
            }

            /// <summary>
            /// Creates a test ammunition object with predefined values.
            /// </summary>
            /// <returns>A new Ammo object with test data.</returns>
            private static Ammo CreateTestAmmo() =>
                new()
                {
                    Brand = "TestBrand",
                    Caliber = "9mm",
                    Quantity = 50,
                    Price = 300,
                    PurchaseDate = DateTime.UtcNow,
                };

            /// <summary>
            /// Prints the successful creation of test ammunition with formatted details.
            /// </summary>
            /// <param name="ammo">The ammunition object to display.</param>
            private static void PrintSuccess(Ammo ammo)
            {
                var details = new[]
                {
                    ("Brand", ammo.Brand),
                    ("Caliber", ammo.Caliber),
                    ("Quantity", ammo.Quantity.ToString()),
                    ("Price", ammo.Price.ToString()),
                    ("Date", ammo.PurchaseDate.ToString()),
                };

                Console.WriteLine("\nTest Ammo creation succeeded!");
                for (int i = 0; i < details.Length; i++)
                {
                    var (label, value) = details[i];
                    var prefix = new string(' ', i * 3) + "└─ ";
                    Console.WriteLine($"{prefix}{label}: {value}");
                }
            }

            /// <summary>
            /// Prints error details in red text to the console.
            /// </summary>
            /// <param name="ex">The exception containing the error details.</param>
            private static void PrintError(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError during test ammo creation: {ex.Message}");
                Console.ResetColor();
            }

            /// <summary>
            /// Waits for a key press before returning to the main menu.
            /// </summary>
            /// <returns>A task representing the asynchronous operation.</returns>
            private static async Task WaitForKeyPress()
            {
                Console.WriteLine("\nPress any key to return to the main menu...");
                await Task.Run(() => Console.ReadKey(intercept: true));
            }
        }

        /// <summary>
        /// Retrieves and displays ammunition details for a specific ID from the database.
        /// </summary>
        /// <param name="id">The ID of the ammunition record to retrieve.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the connection string is not found.</exception>
        private static async Task ViewAmmoDetailsAsync(int id)
        {
            try
            {
                var configuration = BuildConfiguration();
                string? connectionString = GetConnectionString();
                var ammo = await RetrieveAmmoDetails(id, connectionString);

                if (ammo != null)
                {
                    DisplayAmmoDetails(ammo);
                }
                else
                {
                    TypeEffect($"No ammo found with ID {id}");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

            await WaitForKeyPress();
        }

        /// <summary>
        /// Builds the configuration for the application.
        /// </summary>
        /// <returns>The configured IConfiguration instance.</returns>
        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    optional: true
                )
                .AddEnvironmentVariables()
                .Build();
        }

        /// <summary>
        /// Retrieves the connection string from environment variables.
        /// </summary>
        /// <returns>The database connection string.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the connection string is not found.</exception>
        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("SUPABASE_CONNECTION_STRING")
                ?? throw new InvalidOperationException(
                    "SUPABASE_CONNECTION_STRING environment variable not found."
                );
        }

        /// <summary>
        /// Retrieves ammunition details from the database.
        /// </summary>
        /// <param name="id">The ID of the ammunition to retrieve.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <returns>The ammunition details if found, null otherwise.</returns>
        private static async Task<Ammo?> RetrieveAmmoDetails(int id, string connectionString)
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            const string sql =
                "SELECT id, caliber, brand, price, purchase_date, quantity FROM ammo WHERE id = @Id";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Ammo
                {
                    Id = reader.GetInt32(0),
                    Caliber = reader.GetString(1),
                    Brand = reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    PurchaseDate = reader.GetDateTime(4),
                    Quantity = reader.GetInt32(5),
                };
            }

            return null;
        }

        /// <summary>
        /// Displays ammunition details in a formatted manner.
        /// </summary>
        /// <param name="ammo">The ammunition details to display.</param>
        private static void DisplayAmmoDetails(Ammo ammo)
        {
            Console.WriteLine("\nAmmo Details:");
            var details = new[]
            {
                ("ID", ammo.Id.ToString()),
                ("Brand", ammo.Brand),
                ("Caliber", ammo.Caliber),
                ("Quantity", ammo.Quantity.ToString()),
                ("Total Price", $"R{ammo.Price:F2}"),
                ("Price Per Round", $"{ammo.PricePerRound:F2}"),
                ("Purchase Date", ammo.PurchaseDate.ToString("d")),
            };

            for (int i = 0; i < details.Length; i++)
            {
                var (label, value) = details[i];
                var prefix = new string(' ', i * 3) + "└─ ";
                Console.WriteLine($"{prefix}{label}: {value}");
            }
        }

        /// <summary>
        /// Handles and displays error messages.
        /// </summary>
        /// <param name="ex">The exception to handle.</param>
        private static void HandleError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            TypeEffect($"Error retrieving ammo details: {ex.Message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Waits for a key press before continuing.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task WaitForKeyPress()
        {
            TypeEffect("\nPress any key to return to the main menu...");
            await Task.Run(() => Console.ReadKey());
        }

        /// <summary>
        /// Handles the creation of new ammunition with user input.
        /// </summary>
        private static async Task CreateNewAmmo()
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile(
                        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                        optional: true
                    )
                    .AddEnvironmentVariables()
                    .Build();

                var ammoService = new AmmoService(); // Or however you're getting your AmmoService
                var ammoCreation = new AmmoCreation(ammoService);
                await ammoCreation.CreateNewAmmoAsync();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                TypeEffect($"\nError in ammunition creation: {ex.Message}");
                Console.ResetColor();
                Console.ReadKey();
            }
        }
    }
}
