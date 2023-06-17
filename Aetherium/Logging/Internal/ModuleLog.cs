using System;

using Serilog;
using Serilog.Events;

namespace Aetherium.Logging.Internal;

/// <summary>
/// Class offering various methods to allow for logging in Aetherium modules.
/// </summary>
public class ModuleLog
{
    private readonly string moduleName;
    private readonly ILogger moduleLogger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleLog"/> class.
    /// This class can be used to prefix logging messages with a Aetherium module name prefix. For example, "[PLUGINR] ...".
    /// </summary>
    /// <param name="moduleName">The module name.</param>
    public ModuleLog(string? moduleName)
    {
        // FIXME: Should be namespaced better, e.g. `Aetherium.PluginLoader`, but that becomes a relatively large
        //        change.
        this.moduleName = moduleName ?? "AetheriumInternal";
        this.moduleLogger = Log.ForContext("SourceContext", this.moduleName);
    }

    /// <summary>
    /// Log a templated verbose message to the in-game debug log.
    /// </summary>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="values">Values to log.</param>
    public void Verbose(string messageTemplate, params object[] values)
        => this.WriteLog(LogEventLevel.Verbose, messageTemplate, null, values);

    /// <summary>
    /// Log a templated verbose message to the in-game debug log.
    /// </summary>
    /// <param name="exception">The exception that caused the error.</param>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="values">Values to log.</param>
    public void Verbose(Exception exception, string messageTemplate, params object[] values)
        => this.WriteLog(LogEventLevel.Verbose, messageTemplate, exception, values);

    /// <summary>
    /// Log a templated debug message to the in-game debug log.
    /// </summary>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="values">Values to log.</param>
    public void Debug(string messageTemplate, params object[] values)
        => this.WriteLog(LogEventLevel.Debug, messageTemplate, null, values);

    /// <summary>
    /// Log a templated debug message to the in-game debug log.
    /// </summary>
    /// <param name="exception">The exception that caused the error.</param>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="values">Values to log.</param>
    public void Debug(Exception exception, string messageTemplate, params object[] values)
        => this.WriteLog(LogEventLevel.Debug, messageTemplate, exception, values);

    /// <summary>
    /// Log a templated information message to the in-game debug log.
    /// </summary>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="values">Values to log.</param>
    public void Information(string messageTemplate, params object[] values)
        => this.WriteLog(LogEventLevel.Information, messageTemplate, null, values);

    /// <summary>
    /// Log a templated information message to the in-game debug log.
    /// </summary>
    /// <param name="exception">The exception that caused the error.</param>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="values">Values to log.</param>
    public void Information(Exception exception, string messageTemplate, params object[] values)
        => this.WriteLog(LogEventLevel.Information, messageTemplate, exception, values);

    /// <summary>
    /// Log a templated warning message to the in-game debug log.
    /// </summary>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="values">Values to log.</param>
    public void Warning(string messageTemplate, params object[] values)
        => this.WriteLog(LogEventLevel.Warning, messageTemplate, null, values);

    /// <summary>
    /// Log a templated warning message to the in-game debug log.
    /// </summary>
    /// <param name="exception">The exception that caused the error.</param>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="values">Values to log.</param>
    public void Warning(Exception exception, string messageTemplate, params object[] values)
        => this.WriteLog(LogEventLevel.Warning, messageTemplate, exception, values);

    /// <summary>
    /// Log a templated error message to the in-game debug log.
    /// </summary>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="values">Values to log.</param>
    public void Error(string messageTemplate, params object[] values)
        => this.WriteLog(LogEventLevel.Error, messageTemplate, null, values);

    /// <summary>
    /// Log a templated error message to the in-game debug log.
    /// </summary>
    /// <param name="exception">The exception that caused the error.</param>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="values">Values to log.</param>
    public void Error(Exception exception, string messageTemplate, params object[] values)
        => this.WriteLog(LogEventLevel.Error, messageTemplate, exception, values);

    /// <summary>
    /// Log a templated fatal message to the in-game debug log.
    /// </summary>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="values">Values to log.</param>
    public void Fatal(string messageTemplate, params object[] values)
        => this.WriteLog(LogEventLevel.Fatal, messageTemplate, null, values);

    /// <summary>
    /// Log a templated fatal message to the in-game debug log.
    /// </summary>
    /// <param name="exception">The exception that caused the error.</param>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="values">Values to log.</param>
    public void Fatal(Exception exception, string messageTemplate, params object[] values)
        => this.WriteLog(LogEventLevel.Fatal, messageTemplate, exception, values);

    private void WriteLog(LogEventLevel level, string messageTemplate, Exception? exception = null, params object[] values)
    {
        // FIXME: Eventually, the `pluginName` tag should be removed from here and moved over to the actual log
        //        formatter.
        this.moduleLogger.Write(
            level,
            exception: exception,
            messageTemplate: $"[{this.moduleName}] {messageTemplate}",
            values);
    }
}
