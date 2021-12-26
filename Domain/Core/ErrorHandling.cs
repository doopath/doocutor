using System;
using System.Diagnostics.CodeAnalysis;
using Domain.Core.Exceptions.NotExitExceptions;
using Domain.Core.Widgets;
using NLog;

namespace Domain.Core;

public class ErrorHandling
{
    public static Logger FileLogger { get; }
    public static Logger ConsoleLogger { get; }

    static ErrorHandling()
    {
        FileLogger = LogManager.GetLogger("ErrorHandlerFileLogger");
        ConsoleLogger = LogManager.GetLogger("ErrorHandlerConsoleLogger");
    }

    public static void ShowError(Exception error)
        => OutputColorizing.ColorizeForeground(ConsoleColor.Red, () =>
        {
            FileLogger.Debug($"Thrown an error: \"{error.Message}\"\n {error.StackTrace} \n");
            string alertMessage = $"INTERNAL ERROR: {error.Message} " +
                $"(see the logfile in {Settings.ApplicationPath.Replace(@"\", "/")}/logs/doocutor.log)";
            WidgetsMount.Mount(new AlertWidget(alertMessage, AlertLevel.ERROR));
        });
    
    public static void ShowError(string errorMessage)
        => OutputColorizing.ColorizeForeground(ConsoleColor.Red, () =>
        {
            FileLogger.Debug($"Thrown an error: \"{errorMessage}\"\n");
            string alertMessage = $"INTERNAL ERROR: {errorMessage} " + 
                $"(see the logfile in {Settings.ApplicationPath.Replace(@"\", "/")}/logs/doocutor.log)";
            WidgetsMount.Mount(new AlertWidget(alertMessage, AlertLevel.ERROR));
        });

    public static void HandleInterruptedExecutionException(
        [NotNull] InterruptedExecutionException error,
        [NotNull] Action action)
    {
        OutputColorizing.ColorizeForeground(ConsoleColor.Cyan, () => FileLogger.Debug(error.Message));
        action.Invoke();
        Environment.Exit(0);
    }
}