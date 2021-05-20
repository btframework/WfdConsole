using System;
using System.Threading;

using wclCommon;
using wclWiFi;

namespace WfdConsole
{
    class Program
    {
        private static ManualResetEvent FTermEvent = new ManualResetEvent(false);
        private static ManualResetEvent FInitEvent = new ManualResetEvent(false);
        private static Int32 FInitResult = wclErrors.WCL_E_SUCCESS;

        private static void WfdThread()
        {
            wclWiFiSoftAP Ap = new wclWiFiSoftAP();
            Ap.OnDeviceConnected += Ap_OnDeviceConnected;
            Ap.OnDeviceDisconnected += Ap_OnDeviceDisconnected;
            Ap.OnStarted += Ap_OnStarted;
            Ap.OnStopped += Ap_OnStopped;

            FInitResult = Ap.Start("Test", "12345678");
            FInitEvent.Set();

            if (FInitResult == wclErrors.WCL_E_SUCCESS)
            {
                FTermEvent.WaitOne();
                Ap.Stop();
            }

            Ap = null;
        }

        private static void Ap_OnStopped(object sender, EventArgs e)
        {
            Console.WriteLine("Soft AP started");
        }

        private static void Ap_OnStarted(object sender, EventArgs e)
        {
            Console.WriteLine("Sofy AP started");
        }

        private static void Ap_OnDeviceDisconnected(object Sender, wclWiFiDirectDevice Device)
        {
            Console.WriteLine("Device disconnected: " + Device.Name);
        }

        private static void Ap_OnDeviceConnected(object Sender, wclWiFiDirectDevice Device)
        {
            Console.WriteLine("Device connected: " + Device.Name);
        }

        static void Main(string[] args)
        {
            wclMessageBroadcaster.SetSyncMethod(wclMessageSynchronizationKind.skThread);

            Thread t = new Thread(WfdThread);
            t.Start();
            FInitEvent.WaitOne();
            if (FInitResult != wclErrors.WCL_E_SUCCESS)
                Console.WriteLine("Start failed: 0x" + FInitResult.ToString("X8"));

            Console.WriteLine("Press any key");
            Console.ReadKey();

            if (FInitResult == wclErrors.WCL_E_SUCCESS)
            {
                FTermEvent.Set();
                t.Join();
            }
        }
    }
}
