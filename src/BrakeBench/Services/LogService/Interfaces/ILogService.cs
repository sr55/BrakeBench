// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogService.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// <summary>
//   Defines the ILogService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.LogService.Interfaces
{
    using System.Text;

    using BrakeBench.Services.LogService.Models;

    public interface ILogService
    {
        ProcessedLog ProcessLog(StringBuilder logBuffer);
    }
}
