// This file is part of MSI Fan Control.
// Copyright © Sparronator9999 2023-2024.
//
// MSI Fan Control is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// MSI Fan Control is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// MSI Fan Control. If not, see <https://www.gnu.org/licenses/>.

using MSIFanControl.Logs;
using System;
using System.Resources;
using System.ServiceProcess;

namespace MSIFanControl.Service
{
    internal static class Program
    {
        private static readonly Logger Log = new Logger
        {
            ConsoleLogLevel = LogLevel.None,
            FileLogLevel = LogLevel.Debug,
        };

        private static readonly ResourceManager Res = new ResourceManager(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;

            ServiceBase.Run(new ServiceBase[] { new svcFanControl(Log, Res) });
        }

        private static void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal(Res.GetString("svcException"), e.ExceptionObject);
        }
    }
}
