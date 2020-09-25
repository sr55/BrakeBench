// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigService.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// <summary>
//   Defines the IConfigService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.Config.Interfaces
{
    using System.Collections.Generic;

    using BrakeBench.Services.Config.Model;

    public interface IConfigService
    {
        List<TaskSet> GetConfig();

        TaskSet GetConfig(int task);

        void LoadConfig();

        void SaveConfig();
    }
}