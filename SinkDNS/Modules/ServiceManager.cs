using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Diagnostics;

//MIT License

//Copyright (c) 2025 Dimon

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

namespace SinkDNS.Modules
{
    //This will manage and monitor DNSCrypt as a service. It will start, stop, and restart the service as needed. Including checking its status.
    //This will also handle the installation of DNSCrypt if the user hasn't installed it yet.
    internal class ServiceManager
    {
        private const string DnsCryptServiceName = "dnscrypt-proxy";
        public static bool IsDNSCryptRunning()
        {
            try
            {
                using (var serviceController = new ServiceController(DnsCryptServiceName))
                {
                    return serviceController.Status == ServiceControllerStatus.Running;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking DNSCrypt service status: {ex.Message}");
                return false;
            }
        }
        public static bool IsDnsCryptInstalled()
        {
            try
            {
                using (var service = new ServiceController(DnsCryptServiceName))
                {
                    return true;
                }
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking DNSCrypt service installation: {ex.Message}");
                return false;
            }
        }

        public static bool StartDnsCrypt()
        {
            try
            {
                using (var service = new ServiceController(DnsCryptServiceName))
                {
                    if (service.Status != ServiceControllerStatus.Running)
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                        return true;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting DNSCrypt service: {ex.Message}");
                return false;
            }
        }

        public static bool StopDnsCrypt()
        {
            try
            {
                using (var service = new ServiceController(DnsCryptServiceName))
                {
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                        return true;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping DNSCrypt service: {ex.Message}");
                return false;
            }
        }
        public static bool RestartDnsCrypt()
        {
            try
            {
                if (!StopDnsCrypt())
                    return false; //Failed to stop service, bail.

                Task.Delay(1000).Wait();

                return StartDnsCrypt();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error restarting DNSCrypt service: {ex.Message}");
                return false;
            }
        }
    }
}
