// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessedLog.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// <summary>
//   Defines the ProcessedLog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.LogService.Models
{
    using System.Text;

    public class ProcessedLog
    {
        public ProcessedLog()
        {
        }

        public int TaskSetId { get; set; }

        public StringBuilder RawLog { get; set; }

        public decimal? FPS { get; set; }
    }
}
