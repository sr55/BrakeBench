// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandResult.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.TaskRunner.Models
{
    using BrakeBench.Services.Config.Model;
    using BrakeBench.Services.LogService.Models;

    public class CommandResult
    {
        public string Log { get; set; }

        public ProcessedLog ProcessedLog { get; set; }

        public long ExecutionTime { get; set; }

        public long FileSizeBytes { get; set; }

        public TaskItem TaskInfo { get; set; }
    }
}
