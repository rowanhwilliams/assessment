using System.Text;

namespace PureGym.Basket
{
    public class PurchasableItem
    {
        public PurchasableItem(string name, decimal price, string category, bool isDiscountable)
        {
            Name = name;
            Price = price;
            Category = category;
            IsDiscountable = isDiscountable;
        }

        public string Name { get; }
        public decimal Price { get; }
        public string Category { get; }
        public bool IsDiscountable { get; }
    }
}
