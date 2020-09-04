using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RelationalSubsettingLib.Masking.Tests
{
    [TestClass()]
    public class MaskingStrategyFactoryTests
    {
        #region Public Methods

        [TestMethod()]
        public void CreateStrategyFromMaskingOptionTest()
        {
            var x = MaskingStrategyFactory.CreateStrategyFromMaskingOption(MaskingOptions.Replace, "rando value");
            Assert.IsNotNull(x);
            x = null;
            x = MaskingStrategyFactory.CreateStrategyFromMaskingOption(MaskingOptions.Replace);
            Assert.IsNotNull(x);

            //not implemented yet, change these tests when relevant code paths are implemented.
            Assert.ThrowsException<NotImplementedException>(
                () => MaskingStrategyFactory.CreateStrategyFromMaskingOption(MaskingOptions.Randomize));
            Assert.ThrowsException<NotImplementedException>(
                () => MaskingStrategyFactory.CreateStrategyFromMaskingOption(MaskingOptions.Generate));
        }

        #endregion Public Methods
    }
}