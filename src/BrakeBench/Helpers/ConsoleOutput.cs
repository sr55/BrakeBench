// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleOutput.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// <summary>
//   Defines the ConsoleOutput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Helpers
{
    using System;

    public class ConsoleOutput
    {
        public static void WriteLine(string text, ConsoleColor colour = ConsoleColor.White)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void ClearLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
