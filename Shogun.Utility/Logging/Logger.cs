// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Text;

namespace Shogun.Utility.Logging
{
    /// <summary>
    /// Very simple logger that is using Console output.
    /// </summary>
    public sealed class Logger
    {
        private readonly LogLevel _level;
        private readonly FlushPolicy _flushPolicy;
        private readonly StringBuilder _messages = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="logLevel">Defines which types of messages to display and  which not.</param>
        /// <param name="flushPolicy">Defines the way of displaying messages.</param>
        public Logger(LogLevel logLevel, FlushPolicy flushPolicy)
        {
            _level = logLevel;
            _flushPolicy = flushPolicy;
        }
        
        public void Trace(string message)
        {
            Log("TRACE: " + message, LogLevel.Trace);
        }

        public void Debug(string message)
        {
            Log("DEBUG: " + message, LogLevel.Debug);
        }

        public void Info(string message)
        {
            Log("INFO: " + message, LogLevel.Info);
        }

        public void Error(string message)
        {
            Log("ERROR: " + message, LogLevel.Error);
        }

        public void Warning(string message)
        {
            Log("WARN: " + message, LogLevel.Warning);
        }

        /// <summary>
        /// If <c>flushPolicy</c> is set to <see cref="FlushPolicy.Buffered"/> all buffered messages will be displayed.
        /// </summary>
        public void Flush()
        {
            if (_flushPolicy == FlushPolicy.Buffered)
            {
                Console.WriteLine(_messages.ToString());
                _messages.Clear();
            }
        }

        private void Log(string msg, LogLevel level)
        {
            if (_level >= level)
            {
                if (_flushPolicy == FlushPolicy.Buffered)
                {
                    _messages.AppendLine(msg);   
                }
                else
                {
                    Console.WriteLine(msg);
                }
            }
        }
    }
}
