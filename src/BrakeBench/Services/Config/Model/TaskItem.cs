// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskItem.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.Config.Model
{
    using System.Collections.Generic;

    public class TaskItem
    {
        public TaskItem()
        {
            this.CustomCommands = new List<TaskCommand>();
        }

        public int TaskId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SourceFile { get; set; }

        public TaskType Mode { get; set; }

        public int Runs { get; set; }

        public List<TaskCommand> CustomCommands { get; set; }
    }
}
