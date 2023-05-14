// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

namespace WifftOCR.Extensions
{
    internal static class TextWriterExtensions
    {
        private const string DEFAULT_FOREGROUND_COLOR = "\x1B[39m\x1B[22m";
        private const string DEFAULT_BACKGROUND_COLOR = "\x1B[49m";

        public static void WriteWithColor(
            this TextWriter textWriter,
            string message,
            ConsoleColor? background,
            ConsoleColor? foreground
        ) {
            string backgroundColor = background.HasValue ? GetBackgroundColorEscapeCode(background.Value) : null;
            string foregroundColor = foreground.HasValue ? GetForegroundColorEscapeCode(foreground.Value) : null;

            if (backgroundColor != null) textWriter.Write(backgroundColor);
            if (foregroundColor != null) textWriter.Write(foregroundColor);

            textWriter.Write(message);

            if (foregroundColor != null) textWriter.Write(DEFAULT_FOREGROUND_COLOR);
            if (backgroundColor != null) textWriter.Write(DEFAULT_BACKGROUND_COLOR);
        }

        static string GetForegroundColorEscapeCode(ConsoleColor color) => color switch {
            ConsoleColor.Black => "\x1B[30m",
            ConsoleColor.DarkRed => "\x1B[31m",
            ConsoleColor.DarkGreen => "\x1B[32m",
            ConsoleColor.DarkYellow => "\x1B[33m",
            ConsoleColor.DarkBlue => "\x1B[34m",
            ConsoleColor.DarkMagenta => "\x1B[35m",
            ConsoleColor.DarkCyan => "\x1B[36m",
            ConsoleColor.Gray => "\x1B[37m",
            ConsoleColor.Red => "\x1B[1m\x1B[31m",
            ConsoleColor.Green => "\x1B[1m\x1B[32m",
            ConsoleColor.Yellow => "\x1B[1m\x1B[33m",
            ConsoleColor.Blue => "\x1B[1m\x1B[34m",
            ConsoleColor.Magenta => "\x1B[1m\x1B[35m",
            ConsoleColor.Cyan => "\x1B[1m\x1B[36m",
            ConsoleColor.White => "\x1B[1m\x1B[37m",

            _ => DEFAULT_FOREGROUND_COLOR
        };

        static string GetBackgroundColorEscapeCode(ConsoleColor color) => color switch {
            ConsoleColor.Black => "\x1B[40m",
            ConsoleColor.DarkRed => "\x1B[41m",
            ConsoleColor.DarkGreen => "\x1B[42m",
            ConsoleColor.DarkYellow => "\x1B[43m",
            ConsoleColor.DarkBlue => "\x1B[44m",
            ConsoleColor.DarkMagenta => "\x1B[45m",
            ConsoleColor.DarkCyan => "\x1B[46m",
            ConsoleColor.Gray => "\x1B[47m",

            _ => DEFAULT_BACKGROUND_COLOR
        };
    }
}
