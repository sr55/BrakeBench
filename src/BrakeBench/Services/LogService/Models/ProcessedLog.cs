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
    using BrakeBench.Services.Config.Model;

    public class ProcessedLog
    {
        public ProcessedLog()
        {
        }

        public TaskCommand Command { get; set; }

        public decimal? FPS { get; set; }
    }
}
