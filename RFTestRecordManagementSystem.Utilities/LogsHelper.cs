using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFTestRecordManagementSystem.Utilities
{
    public static class LogsHelper
    {
        public static string EnsureLogDirectory()
        {
            var logDirection = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

            Directory.CreateDirectory(logDirection);

            return logDirection;
        }
    }
}
