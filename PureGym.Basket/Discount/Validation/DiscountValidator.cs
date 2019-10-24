using System.Collections.Generic;
using System.Linq;

namespace PureGym.Basket.Discount.Validation
{
    public class DiscountValidator : IValidateAppliedDiscounts
    {
        public DiscountApplicationResult CanApplyDiscount(IEnumerable<IMayApplyDiscount> existingDiscounts, IMayApplyDiscount toAdd)
        {
            return IsTryingToAddAdditionalOfferVoucher(existingDiscounts, toAdd)
                ? DiscountApplicationResult.CreateFailed("Only a single offer voucher can be applied to a basket")
                : DiscountApplicationResult.CreateSuccess();
        }

        private static bool IsTryingToAddAdditionalOfferVoucher(IEnumerable<IMayApplyDiscount> existingDiscounts, IMayApplyDiscount toAdd)
        {
            return
                toAdd.GetDiscountType() == DiscountType.OfferVoucher
                && existingDiscounts.Any(discount => discount.GetDiscountType() == DiscountType.OfferVoucher);
        }
    }
}