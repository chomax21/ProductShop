using System.ComponentModel.DataAnnotations;

namespace ProductShop.ViewModel
{
    public class ViewModelProduct
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ProductComposition { get; set; }
        [Required]
        public string Manufacturer { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int Count { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
