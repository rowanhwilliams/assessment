using System.Collections.Generic;

namespace PureGym.Basket.Discount.Validation
{
    public interface IValidateAppliedDiscounts
    {
        DiscountApplicationResult CanApplyDiscount(IEnumerable<IMayApplyDiscount> existingDiscounts, IMayApplyDiscount toAdd);
    }
}