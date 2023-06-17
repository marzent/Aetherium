using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Aetherium.Configuration.Internal;
using Aetherium.Interface.ImGuiFileDialog;
using Aetherium.Utility;
using Serilog;

namespace Aetherium;

public static class NativeMethods
{
    [DllImport("libAetherium", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern void showAlert(string message, string info, string buttonTitle);
}

/// <summary>
/// The main Aetherium class containing all subsystems.
/// </summary>
public class Aetherium : IServiceType
{
    private readonly ManualResetEventSlim _unloadSignal = new(false);

    /// <summary>
    /// Initializes a new instance of the <see cref="Aetherium"/> class.
    /// </summary>
    /// <param name="info">AetheriumStartInfo instance.</param>
    /// <param name="configuration">The Aetherium configuration.</param>
    /// <param name="mainThreadContinueEvent">Event used to signal the main thread to continue.</param>
    internal Aetherium(AetheriumStartInfo info, AetheriumConfiguration configuration, ManualResetEventSlim mainThreadContinueEvent)
    {
        ServiceManager.InitializeProvidedServices(this, info, configuration);
        
        Task.Run(async () =>
        {
            try
            {
                var tasks = new[]
                {
                    ServiceManager.InitializeEarlyLoadableServices(),
                    ServiceManager.BlockingResolved,
                };

                await Task.WhenAny(tasks);
                var faultedTasks = tasks.Where(x => x.IsFaulted).Select(x => (Exception)x.Exception!).ToArray();
                if (faultedTasks.Any())
                    throw new AggregateException(faultedTasks);

                mainThreadContinueEvent.Set();

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Log.Error(e, "Service initialization failure");
                Environment.Exit(-1);
            }
            finally
            {
                mainThreadContinueEvent.Set();
            }
        });
    }
    
    /// <summary>
    /// Queue an unload of Aetherium when it gets the chance.
    /// </summary>
    public void Unload()
    {
        _unloadSignal.Set();
    }

    /// <summary>
    /// Wait for an unload request to start.
    /// </summary>
    public void WaitForUnload()
    {
        _unloadSignal.Wait();
    }
    
}