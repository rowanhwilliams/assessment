using System.Collections.Generic;
using PureGym.Basket.Discount;
using PureGym.Basket.Discount.Validation;

namespace PureGym.Basket
{
    public class Basket
    {
        private readonly List<BasketItem> _basketItems;
        private readonly List<IMayApplyDiscount> _vouchers;

        private readonly IValidateAppliedDiscounts _discountValidator;

        public Basket(IValidateAppliedDiscounts discountValidator)
        {
            _discountValidator = discountValidator;
            _basketItems = new List<BasketItem>();
            _vouchers = new List<IMayApplyDiscount>();
        }

        public void AddItem(int quantity, PurchasableItem purchaseableItem)
        {
            _basketItems.Add(new BasketItem(quantity, purchaseableItem));
        }

        public DiscountApplicationResult AddVoucher(IMayApplyDiscount voucher)
        {
            var applicationResult = _discountValidator.CanApplyDiscount(_vouchers, voucher);

            if (applicationResult.IsDiscountAllowed)
            {
                _vouchers.Add(voucher);
            }

            return applicationResult;
        }

        public BasketItem[] BasketItems => _basketItems.ToArray();
        public IMayApplyDiscount[] Vouchers => _vouchers.ToArray();
    }
}