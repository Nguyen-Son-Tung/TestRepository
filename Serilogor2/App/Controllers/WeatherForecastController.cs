using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDiagnosticContext diagnosticContext)
        {
            _logger = logger;
            _diagnosticContext = diagnosticContext ?? throw new ArgumentNullException(nameof(diagnosticContext));

        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            int age = 20;
            var fruit = new Dictionary<string, int> { { "Apple", 1 }, { "Pear", 5 } };

            // như nhau
            // chắc class Log sẽ thuận tiện hơn vì khỏi cần khai báo nhưng nó sẽ không biết được cái nguồn phát ra từ đâu
            Log.Information("In my bowl I have {Fruit}", fruit);
            _logger.LogInformation("In my bowl I have {Fruit}", fruit);

            // để khắc phục cái vấn đề trên thì cần cấu hình thêm ForContext
            var myLog = Log.ForContext<WeatherForecastController>();
            myLog.Information("Ok chua");
            var jobLog = Log.ForContext("JobId", 255);
            jobLog.Information("Have a JobId");

            using (LogContext.PushProperty("ABC", "Chicken 123"))
            {
                myLog.Information("Con ha mat le");
            }


                Log.Information("I am {age}", age);
            _diagnosticContext.Set("x-request-id", "Alo alo 1234 Alo alo");
            _logger.LogInformation("abc", _diagnosticContext);



            var sensorInput = new { Latitude = 25, Longitude = 134 };
            Log.Information("Processing {@SensorInput}", sensorInput);
            Log.Information("Processing {SensorInput}", sensorInput);

            Player player = new Player()
            {
                ID = 1,
                Age = 10,
                Name = "Tung",
                UserName = "tung.it"
            };

            Log.Information("I have a player: {@player}", player);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
