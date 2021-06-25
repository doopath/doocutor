﻿using Doocutor.Core.Commands;

namespace Doocutor.Core.Executors
{
    /// <summary>
    /// Executor of input commands
    /// (for example: natime command - ":run" or some editor command).
    /// </summary>
    public interface ICommandExecutor<T> where T : ICommand
    {
        void Execute(T command);
    }
}