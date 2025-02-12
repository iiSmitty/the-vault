namespace AmmoTracker.Models
{
    public class Ammo
    {
        public int Id { get; set; }
        public required string Caliber { get; set; }
        public required string Brand { get; set; }
        public required decimal Price { get; set; }
        public required DateTime PurchaseDate { get; set; }
        public required int Quantity { get; set; }

        // Computed property - not stored in database
        public decimal PricePerRound => Quantity > 0 ? Price / Quantity : 0;
    }
}
