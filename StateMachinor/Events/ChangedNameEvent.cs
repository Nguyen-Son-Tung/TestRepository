using Contracts.MemberChanged;
using StateMachinor.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachinor.Events
{
    public class ChangedNameEvent : ChangedName
    {
        readonly MemberChangedState _state;
        public ChangedNameEvent(MemberChangedState state)
        {
            _state = state;
        }
        public Guid CorrelationId => _state.CorrelationId;

        public string Name => _state.FullName;

        public DateTime TriggerAt => _state.TriggerAt;
    }
}
