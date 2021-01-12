// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Config.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// <summary>
//   Defines the Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.Config.Model
{
    using System.Collections.Generic;

    public class Config
    {
        public Config()
        {
            this.Instances = 1;
            this.Tasks = new List<TaskItem>();
        }

        public int Instances { get; set; }

        public List<TaskItem> Tasks { get; set; }
    }
}
