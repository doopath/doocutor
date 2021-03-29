﻿
namespace Doocutor.Core.Commands
{
    internal class EditorCommand : ICommand
    {
        public CommandType Type { get; private set; } = CommandType.EDITOR_COMMAND;
        public string Content { get; private set; }

        public EditorCommand(string content)
        {
            this.Content = content;
        }
    }
}