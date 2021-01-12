// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportService.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.Reporting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using BrakeBench.Helpers;
    using BrakeBench.Services.Config.Model;
    using BrakeBench.Services.Reporting.Interfaces;
    using BrakeBench.Services.TaskRunner.Models;

    public class ReportService : IReportService
    {
        public void GenerateCSVReport(TaskItem task, List<CommandResult> results, string filename)
        {
            ConsoleOutput.WriteLine(string.Format(" - Generating CSV Report ({0})", filename), ConsoleColor.Cyan);

            string filePath = Path.Combine("reports", filename);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                List<decimal> fpsList = new List<decimal>();

                // Header
                writer.WriteLine("Task ID:, " + task.TaskId);
                writer.WriteLine("Task Name:, " + task.Name);
                writer.WriteLine("Task Description:, " + task.Description);
                writer.WriteLine();

                writer.WriteLine("Task ID, Task Name, Average FPS, Filesize (MB)");

                // Result Body
                foreach (CommandResult commandResult in results)
                {
                    if (commandResult.ProcessedLog.FPS != null)
                    {
                        fpsList.Add(commandResult.ProcessedLog.FPS.Value);
                    }

                    decimal filesize = Math.Round((decimal)commandResult.FileSizeBytes / 1024 / 1024, 2);
                    writer.WriteLine(string.Format("{0}, {1}, {2}, {3}", commandResult.ProcessedLog.Command.CommandId, commandResult.ProcessedLog.Command.Name,  Math.Round(commandResult.ProcessedLog.FPS ?? 0, 2), filesize));
                }

                // Averages
                writer.WriteLine();
                writer.WriteLine(string.Format("Average:, {0}", Math.Round(Enumerable.Average(fpsList), 2) ));
            }
        }

        public void GenerateHTMLReport(TaskItem task, List<CommandResult> results, string filename)
        {
           // ConsoleOutput.WriteLine(string.Format(" - Generating HTML Report ({0})", filename), ConsoleColor.Cyan);

        }
    }
}
