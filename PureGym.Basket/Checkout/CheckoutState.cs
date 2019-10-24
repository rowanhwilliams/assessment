using System;
using System.Linq;
using PureGym.Basket.Discount;

namespace PureGym.Basket.Checkout
{
    public class CheckoutState
    {
        public BasketItem[] Items;
        
        public decimal CurrentTotal { get; }
        public DiscountResult[] DiscountResults;

        private CheckoutState(BasketItem[] items, decimal currentTotal, DiscountResult[] discountResults)
        {
            Items = items;
            CurrentTotal = currentTotal;
            DiscountResults = discountResults;
        }

        public static CheckoutState Initialise(BasketItem[] allBasketItems)
        {
            var total = allBasketItems.Sum(item => item.Price);
            return new CheckoutState(allBasketItems, total, new DiscountResult[0]);
        }

        public BasketItem[] DiscountableItems => Items.Where(basketItem => basketItem.Item.IsDiscountable).ToArray();
        public decimal DiscountableItemsTotal => DiscountableItems.Sum(item => item.Price);

        public CheckoutState ToNewState(decimal newTotal, DiscountResult discountResult)
        {
            return new CheckoutState(Items, newTotal, DiscountResults.Append(discountResult).ToArray());
        }

        public CheckoutResult ToCheckoutResult()
        {
            var failedDiscounts = DiscountResults
                .Where(result => !result.WasApplied)
                .Select(result => result.NotAppliedReason)
                .ToArray();

            var failureMessage = failedDiscounts.Any() ? string.Join(Environment.NewLine, failedDiscounts) : null;
            return new CheckoutResult(CurrentTotal, failureMessage);
        }
    }
}