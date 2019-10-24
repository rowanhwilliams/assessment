using PureGym.Basket.Checkout;

namespace PureGym.Basket.Discount
{
    public interface IMayApplyDiscount
    {
        CheckoutState Apply(CheckoutState checkoutState);
        DiscountType GetDiscountType();
    }
}