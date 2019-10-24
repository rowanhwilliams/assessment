using System;
using PureGym.Basket.Checkout;

namespace PureGym.Basket.Discount
{
    public class GiftVoucher : IMayApplyDiscount
    {
        private readonly string _id;
        private readonly decimal _value;
        
        public GiftVoucher(string id, decimal value)
        {
            _id = id;
            _value = value;
        }

        public CheckoutState Apply(CheckoutState checkoutState)
        {
            if (checkoutState.DiscountableItemsTotal == 0) return ToNotAppliedState(checkoutState);

            var discountToApply = Math.Min(checkoutState.DiscountableItemsTotal, _value);
            var newBasketTotal = checkoutState.CurrentTotal - discountToApply;
            return checkoutState.ToNewState(newBasketTotal, DiscountResult.CreateApplied(_id));
        }

        public DiscountType GetDiscountType()
        {
            return DiscountType.GiftVoucher;
        }

        private CheckoutState ToNotAppliedState(CheckoutState checkoutState)
        {
            var notAppliedResult = DiscountResult.CreateNotApplied(_id, GetNotAppliedMessage());
            return checkoutState.ToNewState(checkoutState.CurrentTotal, notAppliedResult);
        }

        private string GetNotAppliedMessage()
        {
            return $"There are no products in your basket applicable to voucher {_id} ";
        }
    }
}