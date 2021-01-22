using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeepMinds
{
    enum Delimiter
    {
        CarriageReturn = '\r', // CR
        LineFeed = '\n', // LF
        NewLine // CLRF or LF

    }

    static class StreamReaderExtensions
    {
        public static IEnumerable<string> ReadLines(this StreamReader reader, Delimiter delimiter, int? maxLines = null)
        {
            var stringBuilder = new StringBuilder();
            int lines = 0;

            while (!reader.EndOfStream)
            {
                if (delimiter != Delimiter.NewLine)
                {
                    var c = (char)reader.Read();

                    if (c == (char)delimiter)
                    {
                        yield return stringBuilder.ToString();
                        stringBuilder.Clear();
                        lines++;
                    }
                    else
                        stringBuilder.Append(c);
                }
                else
                {
                    yield return reader.ReadLine();
                    lines++;
                }

                if (maxLines.HasValue && maxLines.Value == lines)
                    yield break;
            }
        }
    }
}