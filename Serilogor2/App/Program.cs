using App.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                
                // để kích hoạt Middleware thì chuyển sang Warning
                // còn không thì để Information
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)

                // khai báo thằng để được thêm các thuộc tính tùy ý trong quá trình xử lý
                .Enrich.FromLogContext()

                // nằm trong Package Enricher.Environment
                .Enrich.WithMachineName()

                // SourceContext là một Property cho biết vị trí của đoạn Log được gọi từ lớp nào, ở đâu
                // JobId được khai báo ở Controller bằng Log.ForContext("property", value)
                //.WriteTo.Console(outputTemplate: "{Timestamp:G}[{Level:u3}] [{SourceContext}] [{JobId}] - {Message}{NewLine}{Exception}")
                .WriteTo.Console(outputTemplate: "{Timestamp:G}[{Level:u3}] [{Properties}] - {Message}{NewLine}{Exception}")

                // rollinginterval tức là sau một khoảng thời gian thì nó sẽ đóng file, và ghi log vào file mới
                .WriteTo.File(new CompactJsonFormatter() ,"log.json") // key có tiền tố @, có mã @i gì đó
                .WriteTo.File(new RenderedCompactJsonFormatter() ,"log2.json") // key có tiền tố @, không @i
                .WriteTo.File(new JsonFormatter() ,"log3.json") // không có, đẹp, dễ nhìn
                
                // cái này dùng khi chạy trên IIS server
                //.WriteTo.Debug()

                // Customizing the stored data ==> chỉ lấy các thuộc tính trong một đối tượng phức tạp (nhiều thuộc tính)
                .Destructure.ByTransforming<Player>(p => new { Ten = p.Name, Tuoi = p.Age }) // khi dùng nhớ có @

                .CreateLogger();

            try
            {
                Log.Information("Starting web host ...");
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexceptedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
