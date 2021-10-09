﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Domain.Core.Exceptions;

namespace Domain.Core.CodeFormatters
{
    public class SourceCodeFormatter : ICodeFormatter
    {
        public List<string> SourceCode { get; }

        public SourceCodeFormatter(List<string> sourceCode)
        {
            SourceCode = sourceCode;
        }

        public void AdaptCodeForBufferSize(int maxLineLength)
        {
            for (var i = 0; i < SourceCode.Count; i++)
            {
                var lineNumber = i + 1;
                var line = GetLineAt(lineNumber);
                var prefixLength = GetPrefixLength(lineNumber);

                if (line.Length + prefixLength > maxLineLength)
                {
                    SourceCode[i] = line[..(maxLineLength - prefixLength)];

                    if (SourceCode.Count - 1 < i + 1)
                        SourceCode.Insert(i + 1, "");

                    SourceCode[i + 1] = line[(maxLineLength - prefixLength)..] + SourceCode[i + 1];
                }
            }
        }

        public string GroupNewLineOfACurrentOne(string newPart, int cursorPositionFromTop, int cursorPositionFromLeft)
        {
            var lineNumber = cursorPositionFromTop + 1;
            var currentLine = GroupOutputLineAt(lineNumber)[..^1];

            return currentLine[..cursorPositionFromLeft] + newPart + currentLine[cursorPositionFromLeft..];
        }

        public string GroupOutputLineAt(int lineNumber)
            => $"  {lineNumber}{GetOutputSpacesForLineAt(lineNumber)}|{SourceCode[LineNumberToIndex(lineNumber)]}\n";

        public string SeparateLineFromLineNumber(string line)
            => string.Join("|", line.Split("|")[1..]);

        public string GetTabulationForLineAt(int lineNumber, string line)
        {

            int previousTabulationLength = 0;
            int additionalTabulationLength = 0;

            if (lineNumber != 1)
            {
                var previousLine = SourceCode[LineNumberToIndex(lineNumber) - 1];
                previousTabulationLength = previousLine.Length - previousLine.Trim().Length;
                additionalTabulationLength = LineHasOpeningBrace(previousLine) ? 4 : 0;
            }

            additionalTabulationLength -= LineHasClosingBrace(line) ? 4 : 0;

            var countOfTabs = previousTabulationLength + additionalTabulationLength;

            return countOfTabs > 0 ? new string(' ', countOfTabs) : "";
        }

        public string GetSourceCodeWithLineNumbers()
            => string.Join("", SourceCode.Select((_, i) => GroupOutputLineAt(IndexToLineNumber(i))).ToArray());

        public string GetLineAt(int lineNumber)
        {
            CheckIfLineExistsAt(lineNumber);
            return SourceCode.ToArray()[LineNumberToIndex(lineNumber)];
        }

        public string GetSourceCode()
            => string.Join("", SourceCode.Select(l => l + "\n"));

        public int IndexToLineNumber(int index) => index + 1;

        public int LineNumberToIndex(int lineNumber) => lineNumber - 1;

        public int GetPrefixLength(int currentLineNumber)
        {
            var lastLineNumber = SourceCode.Count;
            var lastLine = GroupOutputLineAt(lastLineNumber)[..^1];
            var lastLineContent = SeparateLineFromLineNumber(lastLine);
            var currentLine = GroupOutputLineAt(currentLineNumber);
            var currentLineContent = SeparateLineFromLineNumber(currentLine);

            return lastLine.Length - lastLineContent.Length + (currentLineContent == "" ? 1 : 0);
        }

        public string ModifyLine(string line, int lineNumber)
            => GetTabulationForLineAt(lineNumber, line) + line.Trim();

        private bool IsClosingBrace(string line)
            => line.Equals("}");

        private bool LineHasOpeningBrace(string line)
        {
            RemoveAllButBracesIn(ref line);
            RemoveAllCoupleBracesIn(ref line);

            return IsOpeningBrace(line);
        }

        private bool LineHasClosingBrace(string line)
        {
            RemoveAllButBracesIn(ref line);
            RemoveAllCoupleBracesIn(ref line);

            return IsClosingBrace(line);
        }

        private bool IsOpeningBrace(string line)
            => line.Equals("{");

        private string RemoveAllButBracesIn(ref string line)
            => line = Regex.Replace(line, @"[^{}]", string.Empty);

        private void RemoveAllCoupleBracesIn(ref string line)
        {
            while (LineContainsBraces(line))
                line = RemoveCoupleBracesIn(ref line);
        }

        private string RemoveCoupleBracesIn(ref string line)
            => line = line.Replace(@"{}", string.Empty);

        private bool LineContainsBraces(string line)
            => line.Contains("{") && line.Contains(("}"));

        private string GetOutputSpacesForLineAt(int lineNumber)
            => ' ' + new string(' ', GetTimesOfSpacesRepeationForLineAt(lineNumber));

        private int GetTimesOfSpacesRepeationForLineAt(int lineNumber)
            => SourceCode.Count.ToString().Length - lineNumber.ToString().Length;

        private void CheckIfLineExistsAt(int lineNumber)
        {
            if (SourceCode.Count < lineNumber || lineNumber < 1)
            {
                throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
            }
        }
    }
}
