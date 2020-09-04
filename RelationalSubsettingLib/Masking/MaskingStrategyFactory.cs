using System;
using System.Collections.Generic;

namespace RelationalSubsettingLib.Masking
{
    public class MaskingStrategyFactory
    {
        #region Private Fields

        private static Dictionary<MaskingOptions, Func<string, IMaskingStrategy>> m_OptionToStrategyFactoryMethods = new Dictionary<MaskingOptions, Func<string, IMaskingStrategy>>()
        {
            {MaskingOptions.Generate, Generate }
            ,{MaskingOptions.Randomize, Randomize }
            ,{MaskingOptions.Replace, Replace }
        };

        #endregion Private Fields

        #region Public Methods

        public static IMaskingStrategy CreateStrategyFromMaskingOption(MaskingOptions option, string maskingMethod = null)
        {
            IMaskingStrategy ret = m_OptionToStrategyFactoryMethods[option](maskingMethod);
            return ret;
        }

        #endregion Public Methods

        #region Private Methods

        private static IMaskingStrategy Generate(string obj)
        {
            throw new NotImplementedException();
        }

        private static IMaskingStrategy Randomize(string obj)
        {
            throw new NotImplementedException();
        }

        private static IMaskingStrategy Replace(string obj)
        {
            if (obj != null)
            {
                return new ReplaceMaskingStrategy(obj);
            }
            else
            {
                return new ReplaceMaskingStrategy();
            }
        }

        #endregion Private Methods
    }
}