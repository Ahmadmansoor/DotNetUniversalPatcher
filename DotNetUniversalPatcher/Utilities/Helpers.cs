﻿using dnlib.DotNet.Emit;
using DotNetUniversalPatcher.Exceptions;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DotNetUniversalPatcher.Utilities
{
    internal static class Helpers
    {
        internal static bool CreatePatcherDirIfNotExists()
        {
            bool result = false;

            try
            {
                if (!Directory.Exists(Constants.PatchersDir))
                {
                    Directory.CreateDirectory(Constants.PatchersDir);
                    File.WriteAllText(Path.Combine(Constants.PatchersDir, "Move Your DNUP Script Files Here!"), "Delete me.");

                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        internal static void SetDebugMode()
        {
#if DEBUG
            Program.IsDebugModeEnabled = true;
#else
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length == 2 && args[1].ToLowerInvariant() == "/debug")
            {
                Program.IsDebugModeEnabled = true;
            }
#endif
        }

        internal static bool IsAdministrator()
        {
            bool result;
            try
            {
                using (WindowsIdentity current = WindowsIdentity.GetCurrent())
                {
                    result = new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        internal static string GetProgramFilesPath(bool getX86 = false)
        {
            if (!getX86 && Environment.Is64BitOperatingSystem)
            {
                return Environment.ExpandEnvironmentVariables("%ProgramW6432%");
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }

        internal static string GetCommonProgramFilesPath(bool getX86 = false)
        {
            if (!getX86 && Environment.Is64BitOperatingSystem)
            {
                return Environment.ExpandEnvironmentVariables("%CommonProgramW6432%");
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
        }

        internal static OpCode GetOpCodeFromString(string name)
        {
            var opCode = (OpCode)typeof(OpCodes).GetField(name.Replace('.', '_'), BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)?.GetValue(null);

            if (opCode == null)
            {
                throw new PatcherException("Invalid OpCode", name);
            }

            return opCode;
        }

        internal static void VisitWebsite(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url)) return;

                if (!Regex.IsMatch(url, "^https?://", RegexOptions.IgnoreCase))
                {
                    url = $"http://{url}";
                }

                Process.Start(url);
            }
            catch
            {
                // ignored
            }
        }

        public static DialogResult CustomMessageBox(string text, bool error = false)
        {
            if (error)
            {
                return MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return MessageBox.Show(text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}