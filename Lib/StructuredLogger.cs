using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lib {
    public static class StructuredLogger {
        static private Func<string, Task> Callback=null;
        static private int Depth=0;

        public static void SetupRecording(Func<string, Task> callback) {
            Callback = callback;
            Depth = 0;
        }

        private static string Space() {
            return string.Concat(Enumerable.Repeat(" ", 4*Depth));
        }

        public static async Task Announce(string message, string kind="log") {
            var ex = kind == "conclusion" ? "***** ": "";
            Console.WriteLine($"{Space()}{ex}{message}");
            if (Callback!=null)
                await Callback($"{Space()}<div class=\"{kind}\">{message}</div>\n"); 
        }

        public static async Task StartSection(string message, string kind="section")  {
            Console.WriteLine($"{Space()}{message}");            
            if (Callback != null) {
                await Callback($"{Space()}<div class=\"{kind}\">\n {Space()}<div class=\"heading\">{message}</div>\n");
            }
            Depth ++;
        }

        public static async Task EndSection() {
            if (Depth > 0) {
                Depth --;
                if (Callback != null) {
                    await Callback($"{Space()}</div>\n");
                }
            }
        }
    }
}