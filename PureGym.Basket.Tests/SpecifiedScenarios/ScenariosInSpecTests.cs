using PureGym.Basket.Checkout;
using PureGym.Basket.Discount;
using PureGym.Basket.Discount.Validation;
using Xunit;

namespace PureGym.Basket.Tests.SpecifiedScenarios
{
    public class ScenariosInSpecTests
    {
        [Fact]
        public void Basket1()
        {
            var basket = NewBasket();

            basket.AddItem(1, new PurchasableItem("Hat", 10.50M, "Clothing", true));
            basket.AddItem(1, new PurchasableItem("Jumper", 54.65M, "Clothing", true));

            basket.AddVoucher(new GiftVoucher("XXX-XXX", 5M));

            var checkoutResult = PerformCheckout(basket);

            Assert.Equal(60.15M, checkoutResult.Total);
            Assert.Null(checkoutResult.FailureMessage);
        }

        [Fact]
        public void Basket2()
        {
            var basket = NewBasket();

            basket.AddItem(1, new PurchasableItem("Hat", 25M, "Clothing", true));
            basket.AddItem(1, new PurchasableItem("Jumper", 26M, "Clothing", true));

            basket.AddVoucher(new OfferVoucher("Voucher YYY-YYY", 5M, 50M, "Head Gear"));

            var checkoutResult = PerformCheckout(basket);

            Assert.Equal(51M, checkoutResult.Total);
            Assert.Equal("There are no products in your basket applicable to voucher Voucher YYY-YYY", checkoutResult.FailureMessage);
        }

        [Fact]
        public void Basket3()
        {
            var basket = NewBasket();

            basket.AddItem(1, new PurchasableItem("Hat", 25M, "Clothing", true));
            basket.AddItem(1, new PurchasableItem("Jumper", 26M, "Clothing", true));
            basket.AddItem(1, new PurchasableItem("Head Light", 3.5M, "Head Gear", true));

            basket.AddVoucher(new OfferVoucher("Voucher YYY-YYY", 5M, 50M, "Head Gear"));

            var checkoutResult = PerformCheckout(basket);

            Assert.Equal(51M, checkoutResult.Total);
            Assert.Null(checkoutResult.FailureMessage);
        }

        [Fact]
        public void Basket4()
        {
            var basket = NewBasket();

            basket.AddItem(1, new PurchasableItem("Hat", 25M, "Clothing", true));
            basket.AddItem(1, new PurchasableItem("Jumper", 26M, "Clothing", true));

            basket.AddVoucher(new GiftVoucher("XXX-XXX", 5M));
            basket.AddVoucher(new OfferVoucher("Voucher YYY-YYY", 5M, 50M, null));

            var checkoutResult = PerformCheckout(basket);

            Assert.Equal(41M, checkoutResult.Total);
            Assert.Null(checkoutResult.FailureMessage);
        }

        [Fact]
        public void Basket5()
        {
            var basket = NewBasket();

            basket.AddItem(1, new PurchasableItem("Hat", 25M, "Clothing", true));
            basket.AddItem(1, new PurchasableItem("Gift Voucher", 30M, "Gifts", false));

            basket.AddVoucher(new OfferVoucher("Voucher YYY-YYY", 5M, 50M, null));

            var checkoutResult = PerformCheckout(basket);

            Assert.Equal(55M, checkoutResult.Total);

            const string expectedMessage = "You have not reached the spend threshold for Voucher YYY-YYY. Spend another £25.01 to receive £5.00 discount from your basket total.";
            Assert.Equal(expectedMessage, checkoutResult.FailureMessage);
        }

        private static Basket NewBasket()
        {
            return new Basket(new DiscountValidator());
        }

        private static CheckoutResult PerformCheckout(Basket basket)
        {
            var checkout = new Checkout.Checkout();
            return checkout.PerformCheckout(basket);
        }
    }
}
