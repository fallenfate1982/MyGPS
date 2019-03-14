using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlertService
{
  static class Program
  {
    static ServiceStop win;
    static ServiceBase[] servicesToRun;

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    
    static void Main()
    {
     
      servicesToRun = new ServiceBase[] 
            { 
                new Alerts() 
            };
      if (Environment.UserInteractive)
      {
        RunInteractive();
      }
      else
      {
        ServiceBase.Run(servicesToRun);
      }
    }

    static void RunInteractive()
    {
      MethodInfo onStartMethod = typeof(ServiceBase).GetMethod("OnStart",
          BindingFlags.Instance | BindingFlags.NonPublic);
      foreach (ServiceBase service in servicesToRun)
      {
        onStartMethod.Invoke(service, new object[] { new string[] { } });
      }

      win = new ServiceStop();
      win.ShowDialog();
      /*MethodInfo onStopMethod = typeof(ServiceBase).GetMethod("OnStop",
          BindingFlags.Instance | BindingFlags.NonPublic);
      foreach (ServiceBase service in servicesToRun)
      {
        Console.Write("Stopping {0}...", service.ServiceName);
        onStopMethod.Invoke(service, null);
        Console.WriteLine("Stopped");
      }

      Console.WriteLine("All services stopped.");
      // Keep the console alive for a second to allow the user to see the message.
      Thread.Sleep(1000);*/
    }

    public static void StopService()
    {
      MethodInfo onStopMethod = typeof(ServiceBase).GetMethod("OnStop",
          BindingFlags.Instance | BindingFlags.NonPublic);
      foreach (ServiceBase service in servicesToRun)
      {
        Console.Write("Stopping {0}...", service.ServiceName);
        onStopMethod.Invoke(service, null);
        Console.WriteLine("Stopped");
      }
    }
  }
}
