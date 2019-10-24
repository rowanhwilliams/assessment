using System;
using System.Collections.Generic;
using System.Text;
using PureGym.Basket.Checkout;
using Xunit;

namespace PureGym.Basket.Tests.Unit.Discount
{
    public class OfferVoucherTests
    {
        private const string VoucherId = "Id";
        private const decimal VoucherValue = 5;
        private const decimal VoucherThreshold = 20;
        private const string VoucherRestrictedToCategory = "Restricted to category";

        [Fact]
        public void Apply_DoesNotApplyToEmptyBaskets()
        {
            var emptyStartingState = CheckoutState.Initialise(new BasketItem[0]);

            var offerVoucher = CreateVoucherWithoutRestrictedCategory();
            var resultingState = offerVoucher.Apply(emptyStartingState);

            AssertDiscountWasNotApplied(resultingState);
        }

        [Fact]
        public void Apply_DoesNotApply_WhenSpendDoesNotExceedThreshold()
        {
            var offerVoucher = CreateVoucherWithoutRestrictedCategory();

            var startingState = CheckoutStateWith
            (
                new BasketItem(1, new PurchasableItem("name", VoucherThreshold, "category", true))
            );

            var resultingState = offerVoucher.Apply(startingState);

            AssertDiscountWasNotApplied(resultingState);
        }

        [Fact]
        public void Apply_DoesApply_WhenSpendDoesExceedThreshold()
        {
            var offerVoucher = CreateVoucherWithoutRestrictedCategory();

            var basketItemOverThreshold = new BasketItem(1, new PurchasableItem("name", VoucherThreshold + 0.01M, "category", true));

            var startingState = CheckoutStateWith(basketItemOverThreshold);

            var resultingState = offerVoucher.Apply(startingState);

            var expectedTotal = basketItemOverThreshold.Price - VoucherValue;
            AssertDiscountWasApplied(resultingState, expectedTotal);
        }

        [Fact]
        public void Apply_DoesNotApply_WhenNoApplicableProducts()
        {
            var offerVoucher = CreateVoucherWithRestrictedCategory();

            var basketItemNotInCorrectCategory = new BasketItem(1, new PurchasableItem("name", VoucherThreshold + 0.01M, "category", true));

            var startingState = CheckoutStateWith(basketItemNotInCorrectCategory);

            var resultingState = offerVoucher.Apply(startingState);

            AssertDiscountWasNotApplied(resultingState);
        }

        [Fact]
        public void Apply_DoesApply_WhenApplicableProductsAndOverThreshold()
        {
            var offerVoucher = CreateVoucherWithRestrictedCategory();

            var basketItemNotInCorrectCategory = new BasketItem(1, new PurchasableItem("name", VoucherThreshold + 0.01M, "category", true));
            var basketItemApplicableProduct = new BasketItem(1, new PurchasableItem("name", 10, VoucherRestrictedToCategory, true));

            var startingState = CheckoutStateWith(basketItemNotInCorrectCategory, basketItemApplicableProduct);

            var resultingState = offerVoucher.Apply(startingState);

            var expectedTotal = basketItemNotInCorrectCategory.Price + basketItemApplicableProduct.Price - VoucherValue;
            AssertDiscountWasApplied(resultingState, expectedTotal);
        }

        [Fact]
        public void Apply_OnlyAppliesToValueOfApplicableItem_WhenApplicableItemIsLessThanVoucher()
        {
            var offerVoucher = CreateVoucherWithRestrictedCategory();

            var basketItemNotInCorrectCategory = new BasketItem(1, new PurchasableItem("name", VoucherThreshold + 0.01M, "category", true));
            var basketItemApplicableProduct = new BasketItem(1, new PurchasableItem("name", VoucherValue - 2, VoucherRestrictedToCategory, true));

            var startingState = CheckoutStateWith(basketItemNotInCorrectCategory, basketItemApplicableProduct);

            var resultingState = offerVoucher.Apply(startingState);

            var expectedTotal = basketItemNotInCorrectCategory.Price;
            AssertDiscountWasApplied(resultingState, expectedTotal);
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

        private static OfferVoucher CreateVoucherWithoutRestrictedCategory()
        {
            return new OfferVoucher(VoucherId, VoucherValue, VoucherThreshold, null);
        }

        private OfferVoucher CreateVoucherWithRestrictedCategory()
        {
            return new OfferVoucher(VoucherId, VoucherValue, VoucherThreshold, VoucherRestrictedToCategory);
        }

        private static CheckoutState CheckoutStateWith(params BasketItem[] basketItems)
        {
            return CheckoutState.Initialise(basketItems);
        }
    }
}
