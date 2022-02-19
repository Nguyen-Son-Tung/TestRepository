using Contracts.MemberChanged;
using MassTransit;
using MassTransit.Saga;
using StateMachinor.StateMachines;
using StateMachinor.States;
using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.EntityFrameworkCoreIntegration;

namespace StateMachinor
{
    class Program
    {
        public static async Task Main()
        {
            var machine = new MemberChangedStateMachine();
            var repository = new InMemorySagaRepository<MemberChangedState>();

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("tung");
                    h.Password("123456");
                });

                cfg.ReceiveEndpoint("ChangedMember", e =>
                {
                    e.StateMachineSaga(machine, repository);
                });
            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                while (true)
                {
                    string value = await Task.Run(() =>
                    {
                        Console.WriteLine("Enter message (or quit to exit)");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                        break;

                    await busControl.Publish<ChangedName>(new
                    {
                        InVar.CorrelationId,
                        Name = value,
                        TriggerAt = DateTime.UtcNow
                    });
                }
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
