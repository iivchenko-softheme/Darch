// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Text;

namespace Shogun.Patterns.Tests.Performance.Utility.Logging
{
    public sealed class Logger
    {
        private readonly LogLevel _level;
        private readonly FlushPolicy _flushPolicy;
        private readonly StringBuilder _messages = new StringBuilder();

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
            Log("ERROR: " + message, LogLevel.Warning);
        }

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
