namespace ProductShop.Interfaces
{
    public interface ISaleService
    {
        public decimal GetDiscount(decimal price, decimal discount);
    }
}
