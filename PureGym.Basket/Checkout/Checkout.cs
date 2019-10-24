using System.Linq;

namespace PureGym.Basket.Checkout
{
    public class Checkout
    {
        public CheckoutResult PerformCheckout(Basket basket)
        {
            var checkoutState = CheckoutState.Initialise(basket.BasketItems);
            var finalCheckoutState = basket.Vouchers.Aggregate(checkoutState, (lastState, nextVoucher) => nextVoucher.Apply(lastState));
            return finalCheckoutState.ToCheckoutResult();
        }
    }
}