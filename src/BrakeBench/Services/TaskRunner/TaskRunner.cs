// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskRunner.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.TaskRunner
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using BrakeBench.Helpers;
    using BrakeBench.Services.Config.Model;
    using BrakeBench.Services.LogService;
    using BrakeBench.Services.LogService.Interfaces;
    using BrakeBench.Services.LogService.Models;
    using BrakeBench.Services.TaskRunner.Interfaces;

    public class TaskRunner : ITaskRunner
    {
        private ILogService logService = new LogService();

        public async void RunTaskSet(TaskSet task)
        {
            Stopwatch watch = Stopwatch.StartNew();
            ConsoleOutput.WriteLine(string.Format("Running Task: {0}", task.Name), ConsoleColor.Cyan);
            ConsoleOutput.WriteLine(string.Format(" - {0}", task.Description), ConsoleColor.Cyan);
            ConsoleOutput.WriteLine(string.Format(" - {0} command(s) to execute.", task.CustomCommands.Count), ConsoleColor.Cyan);
            ConsoleOutput.WriteLine("-----------------------------------------------------", ConsoleColor.Cyan);

            foreach (TaskCommand command in task.CustomCommands)
            {
                ConsoleOutput.WriteLine(string.Format("{2}{2} - Running Command {0} - {1}", command.CommandId, command.Name, Environment.NewLine), ConsoleColor.Cyan);
                ConsoleOutput.WriteLine(string.Empty);

                string commandString = this.PreProcessCommand(task, command);
                StringBuilder result = await RunHandBrakeCli(commandString);
                if (result != null)
                {
                    ProcessedLog processedLog = this.logService.ProcessLog(result);
                }
            }

            watch.Stop();
            ConsoleOutput.WriteLine(string.Format("Task Complete. Took: {0} seconds", watch.ElapsedMilliseconds / 1000), ConsoleColor.Green);
        }
        
        private static Task<StringBuilder> RunHandBrakeCli(string arguments)
        {
            var tcs = new TaskCompletionSource<StringBuilder>();
            StringBuilder logBuffer = new StringBuilder();
            Stopwatch watch = Stopwatch.StartNew();

            var process = new Process
            {
                StartInfo =
                 {
                    FileName = "HandBrakeCLI",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                 },
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
            {
                watch.Stop();
                tcs.SetResult(logBuffer);
                process.Dispose();

                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ConsoleOutput.WriteLine(string.Format(" - Task Completed. Took {0} seconds", watch.ElapsedMilliseconds / 1000), ConsoleColor.Cyan);
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.WriteLine(args.Data);
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    logBuffer.AppendLine(args.Data);
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }

        private string PreProcessCommand(TaskSet taskset, TaskCommand command)
        {
            string result = command.Command;
            string sourceFileName = Path.GetFileNameWithoutExtension(taskset.SourceFile);
            string outputDirectory = Path.Combine("output", taskset.TaskId.ToString());

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            result = result.Replace("{source}", string.Format("\"{0}\"", Path.Combine("sources", taskset.SourceFile)));
            result = result.Replace("{output_file}", string.Format("\"{0}\"", Path.Combine(outputDirectory, string.Format("{0}_{1}", command.CommandId, taskset.SourceFile))));

            return result;
        }
    }
}
