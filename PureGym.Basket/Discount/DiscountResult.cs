namespace PureGym.Basket.Discount
{
    public class DiscountResult
    {
        private DiscountResult(string discountId, bool wasApplied, string notAppliedReason)
        {
            DiscountId = discountId;
            WasApplied = wasApplied;
            NotAppliedReason = notAppliedReason;
        }

        public static DiscountResult CreateApplied(string discountId)
        {
            return new DiscountResult(discountId, true, null);
        }

        public static DiscountResult CreateNotApplied(string discountId, string notAppliedReason)
        {
            return new DiscountResult(discountId, false, notAppliedReason);
        }

        public bool WasApplied { get; }
        public string DiscountId { get; }
        public string NotAppliedReason { get; }
    }
}