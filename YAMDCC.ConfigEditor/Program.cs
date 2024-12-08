// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 2023-2024.
//
// YAMDCC is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// YAMDCC is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// YAMDCC. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.ServiceProcess;
using System.Windows.Forms;
using YAMDCC.ConfigEditor.Dialogs;

namespace YAMDCC.ConfigEditor
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #region Global exception handlers
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += static (sender, e) =>
                new CrashDialog(e.Exception).ShowDialog();

            AppDomain.CurrentDomain.UnhandledException += static (sender, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    new CrashDialog(ex).ShowDialog();
                }
            };
            #endregion

            // Make sure the application data directory structure is set up
            // because apparently windows services don't know how to create
            // directories:
            Directory.CreateDirectory(Paths.Logs);

            if (!IsAdmin())
            {
                Utils.ShowError(Strings.GetString("dlgNoAdmin"));
                return;
            }

            if (!Utils.ServiceExists("yamdccsvc"))
            {
                if (File.Exists("yamdccsvc.exe"))
                {
                    if (MessageBox.Show(
                        Strings.GetString("svcNotInstalled"),
                        "Service not installed",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        if (Utils.InstallService("yamdccsvc"))
                        {
                            if (Utils.StartService("yamdccsvc"))
                            {
                                // Start the program when the service finishes starting:
                                Start();
                            }
                            else
                            {
                                Utils.ShowError(Strings.GetString("svcErrorCrash"));
                            }
                        }
                        else
                        {
                            Utils.ShowError(Strings.GetString("svcInstallFail"));
                        }
                    }
                    return;
                }
                else
                {
                    Utils.ShowError(Strings.GetString("svcNotFound"));
                    return;
                }
            }

            // Check if the service is stopped:
            ServiceController yamdccSvc = new("yamdccsvc");
            try
            {
                ServiceControllerStatus status = yamdccSvc.Status;
                yamdccSvc.Close();

                if (status == ServiceControllerStatus.Stopped)
                {
                    if (MessageBox.Show(
                        Strings.GetString("svcNotRunning"),
                        "Service not running", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        if (!Utils.StartService("yamdccsvc"))
                        {
                            Utils.ShowError(Strings.GetString("svcErrorCrash"));
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowError(Strings.GetString("svcErrorStart", ex));
                return;
            }
            finally
            {
                yamdccSvc?.Close();
            }

            // Start the program when the service finishes starting:
            Start();
        }

        private static void Start()
        {
            int rebootFlag = -1;
            try
            {
                StreamReader sr = new(Paths.ECToConfPending);
                if (int.TryParse(sr.ReadToEnd(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                {
                    rebootFlag = value;
                }
                sr.Close();
            }
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }

            if (rebootFlag == 1)
            {
                if (MessageBox.Show(Strings.GetString("dlgECtoConfRebootPending"),
                    "Reboot pending", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(Paths.ECToConfPending);
                    }
                    catch (DirectoryNotFoundException) { }
                }
                else
                {
                    return;
                }
            }

            Application.Run(new MainWindow());
        }

        private static bool IsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            try
            {
                WindowsPrincipal principal = new(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
            finally
            {
                identity.Dispose();
            }
        }
    }
}
