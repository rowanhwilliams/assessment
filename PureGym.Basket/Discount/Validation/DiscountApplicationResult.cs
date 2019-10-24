namespace PureGym.Basket.Discount.Validation
{
    public class DiscountApplicationResult
    {
        private DiscountApplicationResult(bool isDiscountAllowed, string message)
        {
            IsDiscountAllowed = isDiscountAllowed;
            Message = message;
        }

        public static DiscountApplicationResult CreateSuccess()
        {
            return new DiscountApplicationResult(true, null);
        }

        public static DiscountApplicationResult CreateFailed(string message)
        {
            return new DiscountApplicationResult(false, message);
        }

        public bool IsDiscountAllowed { get; }
        public string Message { get; }
    }
}