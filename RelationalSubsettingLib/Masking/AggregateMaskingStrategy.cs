using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RelationalSubsettingLib.Masking
{
    public class AggregateMaskingStrategy : IMaskingStrategy
    {
        #region Public Properties

        public Action<DataRow> ParallelMaskingAction => LeAction;

        #endregion Public Properties

        #region Private Fields

        private List<IMaskingStrategy> m_Strategies;

        #endregion Private Fields

        #region Public Constructors

        public AggregateMaskingStrategy()
        {
            m_Strategies = new List<IMaskingStrategy>();
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddStrategy(IMaskingStrategy strategy)
        {
            m_Strategies.Add(strategy);
        }

        public void AddStrategyRange(IEnumerable<IMaskingStrategy> strategies)
        {
            m_Strategies.AddRange(strategies);
        }

        public void Initialize(DataTable table, string columnName)
        {
            return;
        }

        #endregion Public Methods

        #region Private Methods

        private void LeAction(DataRow obj)
        {
            foreach (var strat in m_Strategies)
            {
                strat.ParallelMaskingAction(obj);
            }
        }

        #endregion Private Methods
    }
}