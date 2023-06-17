using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Aetherium.Configuration.Internal;
using Aetherium.Logging.Internal;
using Aetherium.Utility;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Aetherium;

/// <summary>
/// The main entrypoint for the Aetherium system.
/// </summary>
public sealed class EntryPoint
{
    /// <summary>
    /// Log level switch for runtime log level change.
    /// </summary>
    public static readonly LoggingLevelSwitch LogLevelSwitch = new(LogEventLevel.Verbose);

    /// <summary>
    /// Initialize Aetherium.
    /// </summary>
    /// <param name="executablePath"></param>
    /// <param name="baseAddress"></param>
    [UnmanagedCallersOnly]
    public static void Initialize(nint executablePath, long baseAddress)
    {
        var infoStr = Marshal.PtrToStringUTF8(executablePath)!;
        var continueExecution = new ManualResetEventSlim(false);
        new Thread(() => RunThread(infoStr, baseAddress, continueExecution)).Start();
        continueExecution.Wait();
        Log.Information("Resuming main executable");
    }

    /// <summary>
    /// Sets up logging.
    /// </summary>
    /// <param name="baseDirectory">Base directory.</param>
    /// <param name="logConsole">Whether to log to console.</param>
    /// <param name="logSynchronously">Log synchronously.</param>
    /// <param name="logName">Name that should be appended to the log file.</param>
    internal static void InitLogging(string baseDirectory, bool logConsole, bool logSynchronously, string? logName)
    {
        var logFileName = logName.IsNullOrEmpty() ? "Aetherium" : $"Aetherium-{logName}";

#if DEBUG
        var logPath = Path.Combine(baseDirectory, $"{logFileName}.log");
        var oldPath = Path.Combine(baseDirectory, $"{logFileName}.old.log");
        var oldPathOld = Path.Combine(baseDirectory, $"{logFileName}.log.old");
#else
        var logPath = Path.Combine(baseDirectory, "..", "..", "..", $"{logFileName}.log");
        var oldPath = Path.Combine(baseDirectory, "..", "..", "..", $"{logFileName}.old.log");
        var oldPathOld = Path.Combine(baseDirectory, "..", "..", "..", $"{logFileName}.log.old");
#endif
        Log.CloseAndFlush();
        
#if DEBUG
        var oldFileOld = new FileInfo(oldPathOld);
        if (oldFileOld.Exists)
        {
            var oldFile = new FileInfo(oldPath);
            if (oldFile.Exists)
                oldFileOld.Delete();
            else
                oldFileOld.MoveTo(oldPath);
        }

        CullLogFile(logPath, 1 * 1024 * 1024, oldPath, 10 * 1024 * 1024);
#else
        try
        {
            if (File.Exists(logPath))
                File.Delete(logPath);
            
            if (File.Exists(oldPath))
                File.Delete(oldPath);
            
            if (File.Exists(oldPathOld))
                File.Delete(oldPathOld);
        }
        catch
        {
            // ignored
        }
#endif

        var config = new LoggerConfiguration()
                     .WriteTo.Sink(SerilogEventSink.Instance)
                     .MinimumLevel.ControlledBy(LogLevelSwitch);

        if (logSynchronously)
        {
            config = config.WriteTo.File(logPath, fileSizeLimitBytes: null);
        }
        else
        {
            config = config.WriteTo.Async(a => a.File(
                                              logPath,
                                              fileSizeLimitBytes: null,
                                              buffered: false,
                                              flushToDiskInterval: TimeSpan.FromSeconds(1)));
        }

        if (logConsole)
            config = config.WriteTo.Console();

        Log.Logger = config.CreateLogger();
    }

    /// <summary>
    /// Initialize all Aetherium subsystems and start running on the main thread.
    /// </summary>
    /// <param name="info">The <see cref="AetheriumStartInfo"/> containing information needed to initialize Aetherium.</param>
    /// <param name="mainThreadContinueEvent">Event used to signal the main thread to continue.</param>
    private static void RunThread(string info, long baseAddress, ManualResetEventSlim continueExecution)
    {
        // Setup logger
        InitLogging(Assembly.GetExecutingAssembly().Location, true, true, "aetherium");
        SerilogEventSink.Instance.LogLine += SerilogOnLogLine;

        // Load configuration first to get some early persistent state, like log level
        //var configuration = AetheriumConfiguration.Load(info.ConfigurationPath!);

        // Set the appropriate logging level from the configuration
        //if (!configuration.LogSynchronously)
        //    InitLogging(info.WorkingDirectory!, info.BootShowConsole, configuration.LogSynchronously, info.LogName);
        LogLevelSwitch.MinimumLevel = LogEventLevel.Verbose;

        // Log any unhandled exception.
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        var unloadFailed = false;
        var delayInitializeMs = 0;

        try
        {
            if (delayInitializeMs > 0)
            {
                Log.Information(string.Format("Waiting for {0}ms before starting a session.", delayInitializeMs));
                Thread.Sleep(delayInitializeMs);
            }

            Log.Information(new string('-', 80));
            Log.Information("Initializing a session..");

            var startInfo = new AetheriumStartInfo();
            var config = new AetheriumConfiguration();

            startInfo.ConfigurationPath = "/Users/marc-aurel/Library/Application Support/Aetherium/config";

            var aetherium = new Aetherium(startInfo, config, continueExecution);
            Log.Information("This is Aetherium - Core: {GitHash}", Util.GetGitHash());

            aetherium.WaitForUnload();

            try
            {
                ServiceManager.UnloadAllServices();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Could not unload services.");
                unloadFailed = true;
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception on main thread.");
        }
        finally
        {
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;

            Log.Information("Session has ended.");
            Log.CloseAndFlush();
            SerilogEventSink.Instance.LogLine -= SerilogOnLogLine;
        }

        // If we didn't unload services correctly, we need to kill the process.
        //if (unloadFailed)
            Environment.Exit(-1);
    }

    private static void SerilogOnLogLine(object? sender, (string Line, LogEvent LogEvent) ev)
    {
        if (ev.LogEvent.Exception == null)
            return;

        // Don't pass verbose/debug/info exceptions to the troubleshooter, as the developer is probably doing
        // something intentionally (or this is known).
        if (ev.LogEvent.Level < LogEventLevel.Warning)
            return;

        //Troubleshooting.LogException(ev.LogEvent.Exception, ev.Line);
    }
    

    /// <summary>
    /// Trim existing log file to a specified length, and optionally move the excess data to another file.
    /// </summary>
    /// <param name="logPath">Target log file to trim.</param>
    /// <param name="logMaxSize">Maximum size of target log file.</param>
    /// <param name="oldPath">.old file to move excess data to.</param>
    /// <param name="oldMaxSize">Maximum size of .old file.</param>
    private static void CullLogFile(string logPath, int logMaxSize, string oldPath, int oldMaxSize)
    {
        var logFile = new FileInfo(logPath);
        var oldFile = new FileInfo(oldPath);
        var targetFiles = new[]
        {
            (logFile, logMaxSize),
            (oldFile, oldMaxSize),
        };
        var buffer = new byte[4096];

        try
        {
            if (!logFile.Exists)
                logFile.Create().Close();

            // 1. Move excess data from logFile to oldFile
            if (logFile.Length > logMaxSize)
            {
                using var reader = logFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var writer = oldFile.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

                var amountToMove = (int)Math.Min(logFile.Length - logMaxSize, oldMaxSize);
                reader.Seek(-(logMaxSize + amountToMove), SeekOrigin.End);

                for (var i = 0; i < amountToMove; i += buffer.Length)
                    writer.Write(buffer, 0, reader.Read(buffer, 0, Math.Min(buffer.Length, amountToMove - i)));
            }

            // 2. Cull each of .log and .old files
            foreach (var (file, maxSize) in targetFiles)
            {
                if (!file.Exists || file.Length <= maxSize)
                    continue;

                using var reader = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var writer = file.Open(FileMode.Open, FileAccess.Write, FileShare.ReadWrite);

                reader.Seek(file.Length - maxSize, SeekOrigin.Begin);
                for (int read; (read = reader.Read(buffer, 0, buffer.Length)) > 0;)
                    writer.Write(buffer, 0, read);

                writer.SetLength(maxSize);
            }
        }
        catch (Exception ex)
        {
            if (ex is IOException)
            {
                foreach (var (file, _) in targetFiles)
                {
                    try
                    {
                        if (file.Exists)
                            file.Delete();
                    }
                    catch (Exception ex2)
                    {
                        Log.Error(ex2, "Failed to delete {file}", file.FullName);
                    }
                }
            }

            Log.Error(ex, "Log cull failed");

            /*
            var caption = "XIVLauncher Error";
            var message = $"Log cull threw an exception: {ex.Message}\n{ex.StackTrace ?? string.Empty}";
            _ = MessageBoxW(IntPtr.Zero, message, caption, MessageBoxType.IconError | MessageBoxType.Ok);
            */
        }
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        switch (args.ExceptionObject)
        {
            case Exception ex:
                Log.Fatal(ex, "Unhandled exception on AppDomain");

                var info = "Further information could not be obtained";
                if (ex.TargetSite != null && ex.TargetSite.DeclaringType != null)
                {
                    info = $"{ex.TargetSite.DeclaringType.Assembly.GetName().Name}, {ex.TargetSite.DeclaringType.FullName}::{ex.TargetSite.Name}";
                }

                Log.Fatal(info);
                Log.CloseAndFlush();
                Environment.Exit(-1);
                break;
            default:
                Log.Fatal("Unhandled object on AppDomain: {Object}", args.ExceptionObject);

                Log.CloseAndFlush();
                Environment.Exit(-1);
                break;
        }
    }

    private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
    {
        if (!args.Observed)
            Log.Error(args.Exception, "Unobserved exception in Task.");
    }
}
