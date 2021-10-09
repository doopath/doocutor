﻿using System;
using NLog;
using Domain.Core.Commands;
using Libraries.Core;

namespace Domain.Core.Executors
{
    public class EditorCommandExecutor : ICommandExecutor<EditorCommand>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public void Execute(EditorCommand command)
        {
            Logger.Debug($"Start execution of the editor command ({command.Content})");

            try
            {
                NativeCommandExecutionProvider.GetExecutingFunction(new NativeCommand(":write " + command.Content.Trim()))();
            }
            catch (Exception error)
            {
                ErrorHandling.showError(error);
            }
        }
    }
}