using Serilog;
using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("myapp.txt", rollingInterval: RollingInterval.Minute)
                .CreateLogger();

            log.Information("Console Application is starting ...");

            int a = 10, b = 0;


            try
            {
                log.Debug("Diving {A} by {B}", a, b);
                Console.WriteLine(a/b);
            }
            catch(Exception ex)
            {
                log.Error(ex, "Something went wrong");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            Console.ReadKey();
        }
    }
}
