﻿
namespace Doocutor.Core.Commands
{
    public interface ICommand
    {
        CommandType Type { get; }
        string Content { get; }
    }
}