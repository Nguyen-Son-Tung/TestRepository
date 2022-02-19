using Automatonymous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachinor.States
{
    public class MemberChangedState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public State CurrentState { get; set; }
        public int MemberId { get; set; }
        public string FullName { get; set; }
        public int Balance { get; set; }
        public DateTime TriggerAt { get; set; }
    }
}
