// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using BrakeBench.Helpers;
    using BrakeBench.Services.Config;
    using BrakeBench.Services.Config.Interfaces;
    using BrakeBench.Services.Config.Model;
    using BrakeBench.Services.Reporting;
    using BrakeBench.Services.Reporting.Interfaces;
    using BrakeBench.Services.TaskRunner;
    using BrakeBench.Services.TaskRunner.Interfaces;
    using BrakeBench.Services.TaskRunner.Models;

    public class Program
    {
        private static IConfigService configService = new ConfigService();
        private static ITaskRunner benchmarkService = new TaskRunner();
        private static IReportService reportService = new ReportService();

        public static void Main(string[] args)
        {
            Init();

            ConsoleOutput.WriteLine("###################################", ConsoleColor.Yellow);
            ConsoleOutput.WriteLine("BrakeBench 1.0 ", ConsoleColor.Yellow);
            ConsoleOutput.WriteLine("###################################", ConsoleColor.Yellow);

            if (args.Length == 0)
            {
                ConsoleOutput.WriteLine("Invalid Command Detected.", ConsoleColor.Red);
                ConsoleOutput.WriteLine("Example Usage: ");
                ConsoleOutput.WriteLine("    ./BrakeBench <task_id>");
                ConsoleOutput.WriteLine(string.Empty);
                ConsoleOutput.WriteLine("Available Task IDs are as follows: ");

                foreach (TaskItem task in configService.GetConfig())
                {
                    ConsoleOutput.WriteLine(string.Format("   {0} - {1} ({2})", task.TaskId, task.Name, task.Description), ConsoleColor.White);
                }

                return;
            }

            if (!File.Exists("HandBrakeCLI") && !File.Exists("HandBrakeCLI.exe"))
            {
                ConsoleOutput.WriteLine("HandBrakeCLI was not found. Please download it from https://handbrake.fr and place it into this directory.");
                Console.Read();
                return;
            }

            string command = args[0];
            
            int taskId = -1;
            if (!string.IsNullOrEmpty(command))
            {
                int.TryParse(command, out taskId);
            }

            if (taskId < 0)
            {
                ConsoleOutput.WriteLine("Please enter a valid task id. These are defined in the config.json file.");
                Console.Read();
                return;
            }

            TaskItem taskItem = configService.GetConfig(taskId);
            Task<List<CommandResult>> results = benchmarkService.RunTaskSet(taskItem);
            results.Wait();
         
            string csvFilename = string.Format("{0}.brakebench.csv", taskItem.TaskId);
            string htmlFilename = string.Format("{0}.brakebench.html", taskItem.TaskId);

            ConsoleOutput.WriteLine(string.Empty);
            ConsoleOutput.WriteLine("-----------------------------------------------------");
            ConsoleOutput.WriteLine("Generating Reports:", ConsoleColor.Cyan);

            reportService.GenerateCSVReport(results.Result, csvFilename);
            reportService.GenerateHTMLReport(results.Result, htmlFilename);

            ConsoleOutput.WriteLine(string.Empty);
            ConsoleOutput.WriteLine(string.Empty);
            ConsoleOutput.WriteLine("Benchmark Complete. Press any key to exit.");

            Console.Read();
        }

        public static void Init()
        {
            if (!Directory.Exists("sources"))
            {
                Directory.CreateDirectory("sources");
            }

            if (!Directory.Exists("output"))
            {
                Directory.CreateDirectory("output");
            }

            if (!Directory.Exists("reports"))
            {
                Directory.CreateDirectory("reports");
            }

            configService.LoadConfig();
        }
    }
}
