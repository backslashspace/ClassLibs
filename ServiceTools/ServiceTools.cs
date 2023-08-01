using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace ServiceTools
{
    ///<summary>Interact with Windows services</summary>
    public sealed class Service
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern Boolean ChangeServiceConfig(IntPtr hService, UInt32 nServiceType, UInt32 nStartType, UInt32 nErrorControl, String lpBinaryPathName, String lpLoadOrderGroup, IntPtr lpdwTagId, [In] Char[] lpDependencies, String lpServiceStartName, String lpPassword, String lpDisplayName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(IntPtr hSCManager, String lpServiceName, UInt32 dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr OpenSCManager(String machineName, String databaseName, UInt32 dwAccess);

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        private static extern Int32 CloseServiceHandle(IntPtr hSCObject);

        private const UInt32 SERVICE_NO_CHANGE = 0xFFFFFFFF;
        private const UInt32 SERVICE_QUERY_CONFIG = 0x00000001;
        private const UInt32 SERVICE_CHANGE_CONFIG = 0x00000002;
        private const UInt32 SC_MANAGER_ALL_ACCESS = 0x000F003F;

        ///<summary>Returns the status of a given service.</summary>
        public static String GetServiceStatus(String ServiceName)
        {
            ServiceController sc = new(ServiceName);

            return sc.Status switch
            {
                ServiceControllerStatus.Running => "running",
                ServiceControllerStatus.Stopped => "stopped",
                ServiceControllerStatus.Paused => "paused",
                ServiceControllerStatus.StopPending => "stopping",
                ServiceControllerStatus.StartPending => "starting",
                _ => "status changing",
            };
        }

        ///<summary>Starts a given service</summary>
        public static void StartService(String ServiceName, String Arguments = null)
        {
            ServiceController sc = new(ServiceName);

            if (Arguments != null)
            {
                sc.Start();

                sc.Close();
                sc.Dispose();

                return;
            }

            sc.Start();

            sc.Close();
            sc.Dispose();
        }

        ///<summary>Stops a given service</summary>
        public static void StopService(String ServiceName)
        {
            ServiceController sc = new(ServiceName);

            sc.Stop();

            sc.Close();
            sc.Dispose();
        }

        ///<summary>Stops a given service</summary>
        public static void PauseService(String ServiceName)
        {
            ServiceController sc = new(ServiceName);

            sc.Pause();

            sc.Close();
            sc.Dispose();
        }

        ///<summary>Changes the service start type</summary>
        public static void SetStartupType(String ServiceName, ServiceStartMode NewMode)
        {
            ServiceController sc = new(ServiceName);

            ChangeStartMode(sc, NewMode);

            sc.Close();
            sc.Dispose();
        }

        private static void ChangeStartMode(ServiceController svc, ServiceStartMode mode)
        {
            IntPtr scManagerHandle = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
            if (scManagerHandle == IntPtr.Zero)
            {
                throw new ExternalException("Open Service Manager Error");
            }

            IntPtr serviceHandle = OpenService(scManagerHandle, svc.ServiceName, SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);

            if (serviceHandle == IntPtr.Zero)
            {
                throw new ExternalException("Open Service Error");
            }

            Boolean result = ChangeServiceConfig(serviceHandle, SERVICE_NO_CHANGE, (UInt32)mode, SERVICE_NO_CHANGE, null, null, IntPtr.Zero, null, null, null, null);

            if (result == false)
            {
                Int32 nError = Marshal.GetLastWin32Error();
                Win32Exception win32Exception = new(nError);
                throw new ExternalException("Could not change service start type: " + win32Exception.Message);
            }

            CloseServiceHandle(serviceHandle);
            CloseServiceHandle(scManagerHandle);
        }
    }
}