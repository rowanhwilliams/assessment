using System;
using System.Collections.Generic;
using System.Linq;
using PureGym.Basket.Checkout;
using PureGym.Basket.Discount;

namespace PureGym.Basket
{
    public class OfferVoucher : IMayApplyDiscount
    {
        private readonly string _id;
        private readonly decimal _value;
        private readonly decimal _minimumThreshold;
        private readonly string _applyToCategory;

        public OfferVoucher(string id, decimal value, decimal minimumThreshold, string applyToCategory)
        {
            _id = id;
            _value = value;
            _minimumThreshold = minimumThreshold;
            _applyToCategory = applyToCategory;
        }

        private const decimal MinimumUnit = 0.01M;

        public CheckoutState Apply(CheckoutState checkoutState)
        {
            var applicableItems = GetApplicableItems(checkoutState).ToArray();
            if (!applicableItems.Any())
            {
                return ToNotAppliedCheckoutState(checkoutState, GetNoApplicableProductsMessage());
            }

            var leftToSpend = AmountRequiredToPassThreshold(checkoutState.DiscountableItemsTotal);

            if(leftToSpend > 0)
            {
                return ToNotAppliedCheckoutState(checkoutState, GetThresholdNotReachedMessage(leftToSpend));
            }

            var currentSpendOnApplicableItems = CurrentSpendOnApplicableItems(applicableItems);
            var discountToApply = CalculateDiscountToApply(currentSpendOnApplicableItems);
            var newStateTotal = checkoutState.CurrentTotal - discountToApply;

            return checkoutState.ToNewState(newStateTotal, DiscountResult.CreateApplied(_id));
        }

        public DiscountType GetDiscountType()
        {
            return DiscountType.OfferVoucher;
        }

        private decimal CalculateDiscountToApply(decimal currentSpendOnApplicableItems)
        {
            return Math.Min(currentSpendOnApplicableItems, _value);
        }

        private decimal CurrentSpendOnApplicableItems(IEnumerable<BasketItem> applicableItems)
        {
            return applicableItems.Sum(item => item.Price);
        }

        private CheckoutState ToNotAppliedCheckoutState(CheckoutState checkoutState, string message)
        {
            var notAppliedResult = DiscountResult.CreateNotApplied(_id, message);
            return checkoutState.ToNewState(checkoutState.CurrentTotal, notAppliedResult);
        }

        private IEnumerable<BasketItem> GetApplicableItems(CheckoutState checkoutState)
        {
            if (_applyToCategory == null) return checkoutState.DiscountableItems;

            return checkoutState.DiscountableItems.Where(basketItem => basketItem.Item.Category == _applyToCategory);
        }

        private string GetThresholdNotReachedMessage(decimal amountRequiredToPassThreshold)
        {
            if(amountRequiredToPassThreshold <= 0) throw new InvalidOperationException($"{amountRequiredToPassThreshold} cannot be 0 or less");

            return $"You have not reached the spend threshold for {_id}. Spend another {amountRequiredToPassThreshold:C} to receive {_value:C} discount from your basket total.";
        }

        private string GetNoApplicableProductsMessage()
        {
            return $"There are no products in your basket applicable to voucher {_id}";
        }

        private decimal AmountRequiredToPassThreshold(decimal currentSpend)
        {
            return (_minimumThreshold - currentSpend) + MinimumUnit;
        }
    }
}