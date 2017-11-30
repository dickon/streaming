using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using System.Net;
using static Lib.StructuredLogger;

namespace Lib {
    public static class StreamedAction {
        public static async Task Go(HttpContext context, Func<Task> invoke, bool throwExceptionOnDisconnect=true) {            
            // chunking inspired from https://stackoverflow.com/questions/42722936/asp-net-core-1-1-chunked-responses
            var response = context.Response;
            response.StatusCode = 200;
            response.ContentType = "text/html";
            await response.WriteAsync($"<html><head><link rel=\"stylesheet\" href=\"https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/css/bootstrap.min.css\" /> </head> <body>");
            // Edge, Firefox and IE will all wait 10+ seconds before showing anything if you send them a small amount of chunked HTML including a piece that will display. Chrome displays it immediately. If you send 1000 spaces in the first chunk they all do what I wanted. Ugh.
            await response.WriteAsync(string.Concat(Enumerable.Repeat(" ", 1000)));
            try {
                SetupRecording(async x=> {
                    if (throwExceptionOnDisconnect && context.RequestAborted.IsCancellationRequested)
                        throw new Exception("client disconnected"); // TODO: throw a better exception
                    await response.WriteAsync(x);
                });
                await invoke.Invoke();
                await response.WriteAsync("</body></html>");
                await response.Body.FlushAsync();
            } catch (Exception ex) {
                await response.WriteAsync($"<strong>ERROR: <PRE>{ex.ToString()}</PRE>");
            }
        }
    }
}