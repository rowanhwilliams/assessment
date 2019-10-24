using System.Collections.Generic;
using Moq;
using PureGym.Basket.Discount;
using PureGym.Basket.Discount.Validation;
using Xunit;

namespace PureGym.Basket.Tests.Unit
{
    public class BasketTests
    {
        private readonly Mock<IValidateAppliedDiscounts> _discountValidatorMock;

        public BasketTests()
        {
            _discountValidatorMock = new Mock<IValidateAppliedDiscounts>();
        }

        [Fact]
        public void AddVoucher_DelegatesToValidator()
        {
            var discountToAddMock = new Mock<IMayApplyDiscount>();
            var expectedValidationResult = DiscountApplicationResult.CreateSuccess();

            _discountValidatorMock
                .Setup(x => x.CanApplyDiscount(It.IsAny<IEnumerable<IMayApplyDiscount>>(), discountToAddMock.Object))
                .Returns(expectedValidationResult)
                .Verifiable();

            var basket = new Basket(_discountValidatorMock.Object);

            var applicationResult = basket.AddVoucher(discountToAddMock.Object);

            Assert.Equal(expectedValidationResult, applicationResult);
            _discountValidatorMock.Verify(x => x.CanApplyDiscount(It.IsAny<IEnumerable<IMayApplyDiscount>>(), discountToAddMock.Object), Times.Once);
        }
    }
}
