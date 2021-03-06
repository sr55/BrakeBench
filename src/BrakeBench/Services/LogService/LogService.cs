﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogService.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// <summary>
//   Defines the LogService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.LogService
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using BrakeBench.Extensions;
    using BrakeBench.Services.LogService.Interfaces;
    using BrakeBench.Services.LogService.Models;

    public class LogService : ILogService
    {
        public ProcessedLog ProcessLog(StringBuilder logBuffer)
        {
            ProcessedLog logData = new ProcessedLog();

            List<string> logRows = logBuffer.ToList();

            foreach (string line in logRows)
            {
                if (line.Contains("work: average"))
                {
                    List<string> regexList = GetResultRegex(@"([0-9.]*) fps", line);
                    if (regexList.Count >= 2)
                    {
                        string fpsStr = regexList[1];
                        decimal fps;
                        if (decimal.TryParse(fpsStr, out fps))
                        {
                            logData.FPS = fps;
                        }
                    }
                }

                // Video Track 
                if (line.Contains("mux: track 0")) 
                {
                    List<string> muxedTrack = GetResultRegex(@"track ([0-9.]*), ([0-9.]*) frames, ([0-9.]*) bytes, ([0-9.]*) kbps, fifo", line);

                    if (muxedTrack.Count >= 5 && decimal.TryParse(muxedTrack[4], out decimal averageBitrate))
                    {
                        logData.VideoAvgBitrate = averageBitrate;
                    }
                }
            }

            return logData;
        }

        public List<string> GetResultRegex(string regex, string line)
        {
            List<string> groups = new List<string>();
            Match match = Regex.Match(line, regex, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                foreach (var group in match.Groups)
                {
                    groups.Add(((Group)group).Value);
                }

                return groups;
            }

            return null;
        }
    }
}
