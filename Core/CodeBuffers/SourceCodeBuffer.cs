﻿using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Doocutor.Core.CodeBuffers.CodePointers;
using Doocutor.Core.Exceptions;

namespace Doocutor.Core.CodeBuffers
{
    internal class SourceCodeBuffer : ICodeBuffer
    {
        private static readonly List<string> SourceCode = InitialSourceCode.GetInitialSourceCode();
        private static int _pointerPosition = InitialSourceCode.InitialPointerPosition;

        public int BufferSize => SourceCode.Count;
        public string CodeWithLineNumbers => GetSourceCodeWithLineNumbers();
        public string Code => GetSourceCode();

        public void RemoveCodeBlock(ICodeBlockPointer pointer)
        {
            CheckIfLineExistsAt(pointer.EndLineNumber - 1);

            for (var i = 0; i < pointer.EndLineNumber - pointer.StartLineNumber; i++)
            {
                SourceCode.RemoveAt(pointer.StartLineNumber - 1);
            }

            AddLineIfBufferIsEmpty();
            SetPointerAtLastLineIfNecessary();
        }

        public void RemoveLine(int lineNumber)
        {
            CheckIfLineExistsAt(lineNumber);
            SourceCode.RemoveAt(lineNumber - 1);
            SetPointerAtLastLineIfNecessary();
        }

        public void ReplaceLineAt(int lineNumber, string newLine)
        {
            CheckIfLineExistsAt(lineNumber);
            SourceCode[lineNumber - 1] = ModifyLine(newLine, lineNumber);
        }

        public void Write(string line)
        {
            SourceCode.Insert(_pointerPosition, ModifyLine(line));
            _pointerPosition++;
        }

        public void WriteAfter(int lineNumber, string line)
        {
            CheckIfLineExistsAt(lineNumber);
            SourceCode.Insert(lineNumber, ModifyLine(line, lineNumber));
        }

        public void WriteBefore(int lineNumber, string line)
        {
            if (lineNumber - 1 > -1 && lineNumber <= SourceCode.Count)
                SourceCode.Insert(lineNumber - 1, ModifyLine(line, 1));
            else
                throw new OutOfCodeBufferSizeException(
           $"You cannot write line before the line with line number = {lineNumber}!");
        }

        private void CheckIfLineExistsAt(int lineNumber)
        {
            if (SourceCode.Count < lineNumber || lineNumber < 1)
            {
                throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
            }
        }

        private void SetPointerAtLastLineIfNecessary()
            => _pointerPosition = (_pointerPosition <= SourceCode.Count) ? _pointerPosition : SourceCode.Count;

        private string ModifyLine(string line, int lineNumber) => GetTabulationForLineAt(lineNumber, line) + line.Trim();
        private string ModifyLine(string line) => GetTabulationForLineAt(_pointerPosition, line) + line.Trim();

        private string GetTabulationForLineAt(int lineNumber, string line)
        {
            int previousTabulationLength = SourceCode[lineNumber - 1].Length - SourceCode[lineNumber - 1].Trim().Length;
            int additionalTabs = LineHasOpeningBracket(SourceCode[lineNumber - 1]) ? 4 : 0;
            
            additionalTabs -= LineHasClosingBracket(line) ? 4 : 0;

            return new string(' ', previousTabulationLength + additionalTabs);
        }

        private bool LineHasOpeningBracket(string line)
        {
            line = RemoveAllButBracketsIn(line);
            RemoveAllCoupleBracketsIn(ref line);
            
            return line.Length == 1 && line.Equals("{");
        }

        private bool LineHasClosingBracket(string line)
        {
            line = RemoveAllButBracketsIn(line);
            RemoveAllCoupleBracketsIn(ref line);
            
            return line.Length == 1 && line.Equals("}");
        }
        
        private string RemoveAllButBracketsIn(string line)
            => Regex.Replace(line, @"[^{}]", string.Empty);

        private void RemoveAllCoupleBracketsIn(ref string line)
        {
            while (line.Contains("{") && line.Contains("}"))
                line = line.Replace(@"{}", string.Empty);
        }

        private string GetOutputSpacesForLineAt(int lineNumber)
            => ' ' + new string(' ', SourceCode.Count.ToString().Length - lineNumber.ToString().Length);

        private string GroupOutputLineAt(int lineNumber)
            => $"  {lineNumber}{GetOutputSpacesForLineAt(lineNumber)}|{SourceCode[lineNumber-1]}\n";

        private string GetSourceCodeWithLineNumbers()
            => string.Join("", SourceCode.Select((_, i) => GroupOutputLineAt(i + 1)).ToArray());

        private string GetSourceCode() => string.Join("", SourceCode.Select(l => l + "\n"));

        private void AddLineIfBufferIsEmpty()
        {
            if (SourceCode.Count == 0)
                SourceCode.Add("");
        }
    }

    internal static class InitialSourceCode
    {
        public static List<string> GetInitialSourceCode() => new(new[] {
            "namespace Doocutor",
            "{",
            "    public class Program",
            "    {",
            "        public static void Main(string[] args)",
            "        {",
            "        }",
            "    }",
            "}"
        });

        public const int InitialPointerPosition = 6;
    }
}