﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITaskRunner.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.TaskRunner.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BrakeBench.Services.Config.Model;
    using BrakeBench.Services.TaskRunner.Models;

    public interface ITaskRunner
    {
        Task<List<CommandResult>> RunTaskSet(TaskItem task);
    }
}
