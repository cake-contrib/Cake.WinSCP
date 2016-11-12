using System;
using Cake.Core.Diagnostics;

namespace Cake.WinSCP
{
    /// <summary>
    /// Add-in logger.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Gets or sets logger engine.
        /// </summary>
        public static ICakeLog LogEngine { private get; set; }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="message">Message.</param>
        public static void Log(string message)
        {
            var text = $"Cake.WinSCP: {message}";

            if (LogEngine == null)
            {
                Console.WriteLine(text);
            }
            else
            {
                LogEngine.Write(Verbosity.Normal, LogLevel.Information, text);
            }
        }
    }
}
