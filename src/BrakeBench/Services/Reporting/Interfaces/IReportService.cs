// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReportService.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.Reporting.Interfaces
{
    using System.Collections.Generic;

    using BrakeBench.Services.TaskRunner.Models;

    public interface IReportService
    {
        void GenerateCSVReport(List<CommandResult> result, string filename);

        void GenerateHTMLReport(List<CommandResult> result, string filename);
    }
}
