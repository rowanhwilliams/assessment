namespace PureGym.Basket
{
    public class BasketItem
    {
        public int Quantity { get; }
        public PurchasableItem Item { get; }

        public BasketItem(int quantity, PurchasableItem item)
        {
            Quantity = quantity;
            Item = item;
        }

        public decimal Price => Quantity * Item.Price;
    }
}