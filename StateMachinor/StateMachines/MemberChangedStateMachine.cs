using Automatonymous;
using Contracts.MemberChanged;
using StateMachinor.Events;
using StateMachinor.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachinor.StateMachines
{
    public class MemberChangedStateMachine : MassTransitStateMachine<MemberChangedState>
    {
        public MemberChangedStateMachine()
        {
            Initially(
                When(ChangedName)
                    .ThenAsync(async context => {
                        await Console.Out.WriteLineAsync($"Triggered a Changed Name Event with CorrelationId {context.Data.CorrelationId}");
                        context.Instance.CorrelationId = context.Data.CorrelationId;
                        context.Instance.TriggerAt = context.Data.TriggerAt;
                        context.Instance.FullName = context.Data.Name;
                        context.Instance.Balance = 50;
                    })
                    .TransitionTo(Changed)
                    .Publish(context => (ChangedBalance)new ChangedBalanceEvent(context.Instance))
                    .Then(context => Console.WriteLine("Published Changed Name Event ...")));

            DuringAny(
                When(ChangedBalance)
                .Then(context => Console.WriteLine($"Triggered a Changed Balance Event with CorrelationId {context.Data.CorrelationId}"))
                .Then(context => Console.WriteLine($"Received New Balance is {context.Data.Balance}"))
                .TransitionTo(Canceled));

        }

        public State Submitted { get; private set; }
        public State Changed { get; private set; }
        public State Canceled { get; private set; }
        public Event<ChangedName> ChangedName { get; private set; }
        public Event<ChangedBalance> ChangedBalance { get; private set; }

    }
}
