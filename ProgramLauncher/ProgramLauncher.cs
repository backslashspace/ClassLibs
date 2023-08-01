using System;
using System.Diagnostics;
using System.IO;

namespace ProgramLauncher
{
    ///<summary>Launcher for executables</summary>
    public sealed class Execute
    {
        ///<summary>Launches executables.</summary>
        ///<exception cref="FileLoadException"></exception>
        public static (String, Int32) EXE(String Path, String Args = null, Boolean RunAs = false, Boolean InternalOutputRedirect = false, Boolean HiddenExecute = false, Boolean WaitForExit = false, String WorkingDirectory = null, Boolean RedirectErrors = false)
        {
            Process process = new();

            process.StartInfo.FileName = Path;

            if (RunAs)
            {
                process.StartInfo.Verb = "runas";
            }

            if (HiddenExecute)
            {
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
            }

            if (InternalOutputRedirect)
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;

                if (RedirectErrors)
                {
                    process.StartInfo.RedirectStandardError = true;
                }
                else
                {
                    process.StartInfo.RedirectStandardError = false;
                }
            }
            else
            {
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.RedirectStandardError = false;
            }

            if (WorkingDirectory != null)
            {
                process.StartInfo.WorkingDirectory = WorkingDirectory;
            }

            if (Args != null)
            {
                process.StartInfo.Arguments = Args;
            }

            process.Start();

            //while (process.StandardOutput.Peek() > -1)
            //{
            //    Output.Add(process.StandardOutput.ReadLine());
            //}

            //while (process.StandardError.Peek() > -1)
            //{
            //    Output.Add(process.StandardError.ReadLine());
            //}

            if (InternalOutputRedirect)
            {
                String Output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();

                if (RedirectErrors)
                {
                    Output += process.StandardError.ReadToEnd();
                }

                return (Output, process.ExitCode);
            }
            else if (WaitForExit)
            {
                process.WaitForExit();

                return (null, process.ExitCode);
            }

            return (null, Int32.MaxValue - 1);
        }
    }
}