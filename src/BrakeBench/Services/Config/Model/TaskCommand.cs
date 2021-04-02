// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskCommand.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// <summary>
//   Defines the TaskCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.Config.Model
{
    using System.Threading;

    public class TaskCommand
    {
        public TaskCommand()
        {
        }

        public TaskCommand(TaskCommand command)
        {
            this.Name = command.Name;
            this.Command = command.Command;
            this.UsedQuality = command.UsedQuality;
            this.QualityMax = command.QualityMax;
            this.QualityMin = command.QualityMin;
        }

        public int AssignedId { get; set; }

        public string Name { get; set; }

        public string Command { get; set; }

        public int UsedQuality { get; set; }

        public int QualityMin { get; set; }

        public int QualityMax { get; set; }
    }
}
