using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using PureGym.Basket.Checkout;
using PureGym.Basket.Discount;
using PureGym.Basket.Discount.Validation;
using Xunit;

namespace PureGym.Basket.Tests.Unit.Checkout
{
    public class CheckoutTests
    {
        private readonly PureGym.Basket.Checkout.Checkout _checkout;
        private const decimal ProductPrice = 10M;

        public CheckoutTests()
        {
            _checkout = new PureGym.Basket.Checkout.Checkout();
        }

        [Fact]
        public void PerformCheckout_HandlesNoDiscounts()
        {
            var basket = CreateBasket();
            var checkoutResult = _checkout.PerformCheckout(basket);

            Assert.Equal(ProductPrice, checkoutResult.Total);
        }

        [Fact]
        public void PerformCheckout_CallsApplyOnDiscounts()
        {
            var discount1 = CreateMockDiscount();
            var discount2 = CreateMockDiscount();

            var basket = CreateBasket(discount1.Object, discount2.Object);

            _checkout.PerformCheckout(basket);

            VerifyApplyCalled(discount1);
            VerifyApplyCalled(discount2);
        }

        private static Mock<IMayApplyDiscount> CreateMockDiscount()
        {
            var mockDiscount = new Mock<IMayApplyDiscount>();
            mockDiscount
                .Setup(x => x.Apply(It.IsAny<CheckoutState>()))
                .Returns<CheckoutState>(checkoutState => checkoutState)
                .Verifiable();

            return mockDiscount;
        }

        private void VerifyApplyCalled(Mock<IMayApplyDiscount> mockDiscount)
        {
            mockDiscount.Verify(x => x.Apply(It.IsAny<CheckoutState>()), Times.Once);
        }

        private Basket CreateBasket(params IMayApplyDiscount[] vouchers)
        {
            var basket = InitialiseBasket();

            basket.AddItem(1, new PurchasableItem("product", ProductPrice, "category", true));

            vouchers.ToList().ForEach(voucher => basket.AddVoucher(voucher));

            return basket;
        }

        private Basket InitialiseBasket()
        {
            var mockDiscountValidator = new Mock<IValidateAppliedDiscounts>();
            mockDiscountValidator
                .Setup(x => x.CanApplyDiscount(It.IsAny<IEnumerable<IMayApplyDiscount>>(), It.IsAny<IMayApplyDiscount>()))
                .Returns(DiscountApplicationResult.CreateSuccess());

            return new Basket(mockDiscountValidator.Object);
        }
    }
}
