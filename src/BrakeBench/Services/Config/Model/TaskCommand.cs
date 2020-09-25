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
    public class TaskCommand
    {
        public string Name { get; set; }
        public int CommandId { get; set; }
        public string Command { get; set; }
    }
}
