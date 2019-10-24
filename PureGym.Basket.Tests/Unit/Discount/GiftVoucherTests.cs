using PureGym.Basket.Checkout;
using PureGym.Basket.Discount;
using Xunit;

namespace PureGym.Basket.Tests.Unit.Discount
{
    public class GiftVoucherTests
    {
        private readonly GiftVoucher _giftVoucher;

        private const decimal GiftVoucherValue = 5;

        public GiftVoucherTests()
        {
            _giftVoucher = new GiftVoucher("id", GiftVoucherValue);
        }

        [Fact]
        public void Apply_DoesNotApplyWhenNoBasketItems()
        {
            var emptyStartingCheckoutState = CheckoutState.Initialise(new BasketItem[0]);
            var resultingCheckoutState = _giftVoucher.Apply(emptyStartingCheckoutState);

            AssertDiscountWasNotApplied(resultingCheckoutState);
        }

        [Fact]
        public void Apply_DoesNotApplyWhenNoDiscountableBasketItems()
        {
            var nonDiscountableItem1 = new BasketItem(1, new PurchasableItem("something not discountable", 10M, "category", false));
            var nonDiscountableItem2 = new BasketItem(1, new PurchasableItem("something not discountable", 10M, "category", false));

            var startingCheckoutState = CheckoutStateWith(nonDiscountableItem1, nonDiscountableItem2);

            var resultingCheckoutState = _giftVoucher.Apply(startingCheckoutState);

            AssertDiscountWasNotApplied(resultingCheckoutState);
        }

        [Fact]
        public void Apply_AppliesWhenDiscountableBasketItemsArePresent()
        {
            var discountableItem = new BasketItem(1, new PurchasableItem("something discountable", 10M, "category", true));
            var nonDiscountableItem = new BasketItem(1, new PurchasableItem("something not discountable", 10M, "category", false));

            var startingCheckoutState = CheckoutStateWith(discountableItem, nonDiscountableItem);

            var resultingCheckoutState = _giftVoucher.Apply(startingCheckoutState);

            var expectedTotal = discountableItem.Price + nonDiscountableItem.Price - GiftVoucherValue;
            AssertDiscountWasApplied(resultingCheckoutState, expectedTotal);
        }

        [Fact]
        public void Apply_WillNotTakeTotalBelowZero_WhenVoucherIsMoreThanBasketTotal()
        {
            var discountableItemLessThanVoucherValue = new BasketItem(1, new PurchasableItem("something discountable", 2M, "category", true));
            
            var startingCheckoutState = CheckoutStateWith(discountableItemLessThanVoucherValue);

            var resultingCheckoutState = _giftVoucher.Apply(startingCheckoutState);

            AssertDiscountWasApplied(resultingCheckoutState, 0);
        }

        private static CheckoutState CheckoutStateWith(params BasketItem[] basketItems)
        {
            return CheckoutState.Initialise(basketItems);
        }

        private static void AssertDiscountWasNotApplied(CheckoutState resultingState)
        {
            var discountResult = Assert.Single(resultingState.DiscountResults);
            Assert.NotNull(discountResult);
            Assert.False(discountResult.WasApplied);
            Assert.NotNull(discountResult.NotAppliedReason);
        }

        private static void AssertDiscountWasApplied(CheckoutState resultingState, decimal expectedTotal)
        {
            var discountResult = Assert.Single(resultingState.DiscountResults);
            Assert.NotNull(discountResult);
            Assert.True(discountResult.WasApplied);

            Assert.Equal(expectedTotal, resultingState.CurrentTotal);
        }
    }
}
