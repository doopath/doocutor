﻿using System;

namespace Doocutor.Core
{
    internal static class Info
    {
        /// <summary>
        /// Build version variants:
        /// U - unstable
        /// S - stable
        /// F - full
        /// C - changes
        /// B - build
        /// DDMMYY - Day.Month.Year datetime
        /// </summary>
        public const string BuildInfo = "UCB-100621";
        public const string Updated = "10th of June 2021";
        public const string Company = "Doopath";
        public const string Version = "0.1.5";
        public const string ProductName = "Doocutor";
        public const string ConfigurationAttribute = "Debug";

        public static string DoocutorInfo
            => $"Doocutor v{Version}. Build: {BuildInfo}\n" +
               "Try :help command for getting help list, :info for getting doocutor description or :quit to exit.";

        public static string Description
            => $"Doocutor v{Version}. Build: {BuildInfo}\n" +
               "Doocutor is doopath's project with GPL V3 license.\n" +
               "It's C# code executor (doo & executor) working in a terminal.";

        public static string HelpList
            => $"The doocutor (v{Version}) help list.\n" +
               ":quit - Exit the program.\n" + 
               ":view - View code.\n" + 
               ":write <new line> - Write a new line after current pointer position (see :showPos).\n\t" +
                   "Also you can add a new line without a command. Just type it and press enter.\n" +
               ":writeAfter <line number> <new line> - Write a new line after <line number>.\n" +
               ":writeBefore <line number> <new line> - Write a new line before <line number>.\n" +
               ":compile - Compile current code.\n" +
               ":run - Run compiled code. You should not compile code before running. It will be compiled automatically.\n" +
               ":using <namespace> - Add a namespace to using list (for example: System).\n" +
               ":copy <line number> - Copy the <line number> content.\n" +
               ":remove <line number> - Remove line at <line number>.\n" +
               ":removeBlock <since> <to> - Remove a block of code since <since> to <to> (line number).\n" +
               ":replace <line number> <new content> - Replace a line at <line number> with <new content>.\n" +
               ":set <line number> - Set current cursor position.\n" +
               ":showPos - Show current position of the cursor.\n";
        
        public static void ShowDoocutorInfo() => Console.WriteLine(DoocutorInfo);
    }
}
