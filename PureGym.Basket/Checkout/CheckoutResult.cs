namespace PureGym.Basket.Checkout
{
    public class CheckoutResult
    {
        public CheckoutResult(decimal total, string failureMessage)
        {
            Total = total;
            FailureMessage = failureMessage;
        }

        public decimal Total { get; }
        public string FailureMessage { get; }
    }
}