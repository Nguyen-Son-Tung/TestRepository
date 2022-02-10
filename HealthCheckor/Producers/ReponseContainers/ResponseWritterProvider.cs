using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Producers.ReponseContainers
{
    public static class ResponseWritterProvider
    {
        public static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject(); // level 1
                    writer.WriteString("status", result.Status.ToString());
                    writer.WriteString("totalDuration", result.TotalDuration.ToString());


                    writer.WriteStartObject("entries"); // level 2
                    foreach (var entry in result.Entries)
                    {
                        writer.WriteStartObject(entry.Key); //level 3

                        writer.WriteStartObject("data"); // level 4
                        foreach (var item in entry.Value.Data)
                        {
                            writer.WritePropertyName(item.Key);
                            JsonSerializer.Serialize(
                                writer, item.Value, item.Value?.GetType() ??
                                typeof(object));
                        }
                        writer.WriteEndObject(); // end level 4



                        writer.WriteString("duration", entry.Value.Duration.ToString());
                        writer.WriteString("status", entry.Value.Status.ToString());

                        writer.WriteStartArray("tags"); // start array [TAGS]
                        foreach(var tag in entry.Value.Tags)
                        {
                            writer.WriteStringValue(tag);
                        }
                        writer.WriteEndArray(); // end array

                        writer.WriteEndObject(); // end level 3
                    }
                    writer.WriteEndObject(); // end Level 2
                    writer.WriteEndObject(); // end Level 1
                }

                var json = Encoding.UTF8.GetString(stream.ToArray());

                return context.Response.WriteAsync(json);
            }
        }
    }
}
