using EventContracts;
using GreenPipes;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Consumers
{
    class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {

                cfg.Host("rabbitmq://localhost", h =>
                {
                    h.Username("tung");
                    h.Password("123456");
                });

                cfg.ReceiveEndpoint("event-listener", e =>
                {
                    #region Kiểu khai báo chung (retry)
                    //e.UseMessageRetry(r =>
                    //{

                    //    r.Exponential(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(8), TimeSpan.FromSeconds(2));

                    //    r.Handle<Exception>(ex =>
                    //    {
                    //        Console.WriteLine(ex.Message);
                    //        return true;
                    //    });
                    //});
                    #endregion

                    // không chứa các tin error
                    e.DiscardFaultedMessages();

                    e.Consumer<EventConsumer>(cfg => {
                        cfg.UseMessageRetry(r =>
                        {
                            r.Exponential(2, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(2));

                            // select specified exception with some condition
                            r.Handle<Exception>();
                        });

                        cfg.UseCircuitBreaker(cb =>
                        {
                            cb.Handle<Exception>();

                            // có nghĩa là thời gian mà nó sẽ đếm lỗi và đưa ra tỉ lệ %, sau khi hết thgian này nó sẽ so sánh
                            // với TripThreshold xem thử đủ điều kiện chưa
                            cb.TrackingPeriod = TimeSpan.FromSeconds(10);

                            // khoảng 10%
                            cb.TripThreshold = 10;

                            // đặt giới hạn message để đánh giá, phục vụ cho Tripthreshold
                            cb.ActiveThreshold = 1;

                            // thời gian hoãn, từ chối truy cập
                            cb.ResetInterval = TimeSpan.FromMinutes(20);
                        });

                    });
                });

                cfg.ReceiveEndpoint("event-listener_fault", e =>
                {
                    e.Consumer<EventFaultConsumer>();
                });

            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                Console.WriteLine("Press enter to exit");

                await Task.Run(() => Console.ReadLine());
            }
            finally
            {
                await busControl.StopAsync();
            }
        }

        class EventConsumer :
            IConsumer<ValueEntered>
        {
            public async Task Consume(ConsumeContext<ValueEntered> context)
            {
                Console.WriteLine("Consumer Executing .....");
                await Task.Run(() =>
                {
                    Random rd = new Random();
                    if (rd.Next(1, 10) < 9)
                        throw new Exception("Fake Exception");
                    Console.WriteLine("Value: {0}", context.Message.Value);
                });
            }
        }

        class EventFaultConsumer : IConsumer<Fault<ValueEntered>>
        {
            public Task Consume(ConsumeContext<Fault<ValueEntered>> context)
            {
                return Task.Run(() =>
                {
                    Console.WriteLine("Retry finish and throw a new Exception");
                    Console.WriteLine($"{context.Message.Message.Value}");
                });
            }
        }
    }
}


namespace EventContracts
{
    public interface ValueEntered
    {
        string Value { get; }
    }
}
