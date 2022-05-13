namespace ProductShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ProductComposition { get; set; }
        public string Manufacturer { get; set; }
        public bool IsDeleted { get; set; }

    }
}
