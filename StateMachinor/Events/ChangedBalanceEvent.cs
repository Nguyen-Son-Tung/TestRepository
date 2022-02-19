using Contracts.MemberChanged;
using StateMachinor.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachinor.Events
{
    public class ChangedBalanceEvent : ChangedBalance
    {
        readonly MemberChangedState _state;
        public ChangedBalanceEvent(MemberChangedState state)
        {
            _state = state;
        }

        public Guid CorrelationId => _state.CorrelationId;

        public int Balance => _state.Balance;

        public DateTime TriggerAt => _state.TriggerAt;
    }
}
