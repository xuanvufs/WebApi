﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Data.Edm.Library;
using Microsoft.Data.OData;
using Microsoft.TestCommon;

namespace System.Web.Http.OData.Query.Validators
{
    public class OrderByQueryValidatorTest
    {
        private OrderByQueryValidator _validator;
        private ODataQueryContext _context;

        public OrderByQueryValidatorTest()
        {
            _validator = new OrderByQueryValidator();
            _context = ValidationTestHelper.CreateCustomerContext();
        }

        [Fact]
        public void ValidateThrowsOnNullOption()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _validator.Validate(null, new ODataValidationSettings()));
        }

        [Fact]
        public void ValidateThrowsOnNullSettings()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _validator.Validate(new OrderByQueryOption("Name eq 'abc'", _context), null));
        }

        [Fact]
        public void ValidateWillNotAllowName()
        {
            // Arrange
            OrderByQueryOption option = new OrderByQueryOption("Name", _context);
            ODataValidationSettings settings = new ODataValidationSettings();
            settings.AllowedOrderByProperties.Add("Id");

            // Act & Assert
            Assert.Throws<ODataException>(() => _validator.Validate(option, settings),
                "Order by 'Name' is not allowed. To allow it, set the 'AllowedOrderByProperties' property on QueryableAttribute or QueryValidationSettings.");
        }

        [Fact]
        public void ValidateWillNotAllowMultipleProperties()
        {
            // Arrange
            OrderByQueryOption option = new OrderByQueryOption("Name desc, Id asc", _context);
            ODataValidationSettings settings = new ODataValidationSettings();
            Assert.DoesNotThrow(() => _validator.Validate(option, settings));

            settings.AllowedOrderByProperties.Add("Address");
            settings.AllowedOrderByProperties.Add("Name");

            // Act & Assert
            Assert.Throws<ODataException>(() => _validator.Validate(option, settings),
                "Order by 'Id' is not allowed. To allow it, set the 'AllowedOrderByProperties' property on QueryableAttribute or QueryValidationSettings.");
        }

        [Fact]
        public void ValidateWillAllowId()
        {
            // Arrange
            OrderByQueryOption option = new OrderByQueryOption("Id", _context);
            ODataValidationSettings settings = new ODataValidationSettings();
            settings.AllowedOrderByProperties.Add("Id");

            // Act & Assert
            Assert.DoesNotThrow(() => _validator.Validate(option, settings));
        }

        [Fact]
        public void ValidateAllowsOrderByIt()
        {
            // Arrange
            OrderByQueryOption option = new OrderByQueryOption("$it", _context);
            ODataValidationSettings settings = new ODataValidationSettings();

            // Act & Assert
            Assert.DoesNotThrow(() => _validator.Validate(option, settings));
        }

        [Fact]
        public void ValidateDisallowsOrderByIt_IfTurnedOff()
        {
            // Arrange
            _context = new ODataQueryContext(EdmCoreModel.Instance, typeof(int));
            OrderByQueryOption option = new OrderByQueryOption("$it", _context);
            ODataValidationSettings settings = new ODataValidationSettings();
            settings.AllowedOrderByProperties.Add("dummy");

            // Act & Assert
            Assert.Throws<ODataException>(
                () => _validator.Validate(option, settings),
                "Order by '$it' is not allowed. To allow it, set the 'AllowedOrderByProperties' property on QueryableAttribute or QueryValidationSettings.");
        }
    }
}