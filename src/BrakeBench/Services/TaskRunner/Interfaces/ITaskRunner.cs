// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITaskRunner.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.TaskRunner.Interfaces
{
    using BrakeBench.Services.Config.Model;

    public interface ITaskRunner
    {
        public void RunTaskSet(TaskSet task);
    }
}
