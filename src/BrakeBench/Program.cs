// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench
{
    using System;
    using System.IO;

    using BrakeBench.Helpers;
    using BrakeBench.Services.Config;
    using BrakeBench.Services.Config.Interfaces;
    using BrakeBench.Services.Config.Model;
    using BrakeBench.Services.TaskRunner;
    using BrakeBench.Services.TaskRunner.Interfaces;

    public class Program
    {
        private static IConfigService configService = new ConfigService();
        private static ITaskRunner benchmarkService = new TaskRunner();

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

                foreach (TaskSet task in configService.GetConfig())
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
            string firstArgument = args.Length > 1 ? args[1] : null;
            string secondArgument = args.Length > 2 ? args[2] : null;
            string thridArgument = args.Length > 3 ? args[3] : null;

            int taskId = -1;
            if (!string.IsNullOrEmpty(command))
            {
                int.TryParse(command, out taskId);
            }

            //if (taskId < 0)
            //{
            //    ConsoleOutput.WriteLine("Please enter a valid task id. These are defined in the config.json file.");
            //    Console.Read();
            //    return;
            //}

            TaskSet taskset = configService.GetConfig(taskId);
            benchmarkService.RunTaskSet(taskset);

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

            configService.LoadConfig();
        }
    }
}
