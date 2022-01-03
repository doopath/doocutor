﻿using System;
using CommandLine;
using Common.Options;
using Utils.Exceptions;
using CUI;
using Domain.Core;
using NLog;
using Utils;

namespace Doocutor
{
    public static class Program
    {
        private static Logger Logger { get; }

        static Program()
        {
            Logger = LogManager.GetLogger("Doocutor.Program");
        }

        public static void Main(string[] args)
        {
            try
            {
                Console.CancelKeyPress += (sender, ea)
                    => ErrorHandling.HandleInterruptedExecutionException(
                           new("You have came out of the Doocutor! Good bye!"), End);
                Start();
                new DynamicEditorSetup().Run(ParseCommandLineArguments(args));
            }
            catch (InterruptedExecutionException error)
            {
                ErrorHandling.HandleInterruptedExecutionException(error, End);
            }
            catch (Exception error)
            {
                ErrorHandling.ShowError(error);
            }
            finally
            {
                End();
            }
        }

        private static void Start()
            => OutputColorizing.ColorizeForeground(ConsoleColor.Cyan, () =>
           {
               Logger.Debug("Start of the program");
               Info.ShowDoocutorInfo();
           });

        private static void End()
            => OutputColorizing.ColorizeForeground(ConsoleColor.Cyan,
                () =>
                {
                    Logger.Debug("End of the program\n\n");
                    OutputColorizing.ColorizeForeground(ConsoleColor.Cyan,
                       () => Console.WriteLine("\nGood bye!\n"));
                });

        private static ProgramOptions ParseCommandLineArguments(string[] args)
        {
            var result = new ProgramOptions();

            Parser
                .Default
                .ParseArguments<ProgramOptions>(args)
                .WithParsed(ops => result = ops);

            return result;
        }
    }
}
