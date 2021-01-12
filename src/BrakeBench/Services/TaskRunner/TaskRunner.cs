// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskRunner.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.TaskRunner
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using System.Threading.Tasks;

    using BrakeBench.Helpers;
    using BrakeBench.Services.Config.Model;
    using BrakeBench.Services.LogService;
    using BrakeBench.Services.LogService.Interfaces;
    using BrakeBench.Services.LogService.Models;
    using BrakeBench.Services.TaskRunner.Interfaces;
    using BrakeBench.Services.TaskRunner.Models;

    public class TaskRunner : ITaskRunner
    {
        private static ILogService logService = new LogService();
        private readonly object lockObject = new object();

        private bool isDone = false;

        public async Task<List<CommandResult>> RunTaskSet(TaskItem task)
        {
            List<CommandResult> results = new List<CommandResult>();
            ConsoleOutput.WriteLine(string.Format("Running Task: {0}", task.Name), ConsoleColor.Cyan);
            ConsoleOutput.WriteLine(string.Format(" - {0}", task.Description), ConsoleColor.Cyan);
            ConsoleOutput.WriteLine(string.Format(" - {0} command(s) to execute.", task.CustomCommands.Count), ConsoleColor.Cyan);
            ConsoleOutput.WriteLine("-----------------------------------------------------");

            foreach (TaskCommand command in task.CustomCommands)
            {
                Stopwatch watch = Stopwatch.StartNew();
                this.isDone = false;

                // Execute the run
                CommandResult result = new CommandResult();
                ConsoleOutput.WriteLine(string.Format("{2}{2} - Running Command {0} - {1}", command.CommandId, command.Name, Environment.NewLine), ConsoleColor.Cyan);

                string commandString = this.PreProcessCommand(task, command);
                StringBuilder logData = await this.RunHandBrakeCli(commandString);
                watch.Stop();

                // Gather the results of the run.
                result.Log = logData.ToString();
                ProcessedLog processedLog = logService.ProcessLog(logData);
                processedLog.Command = command;
                result.ProcessedLog = processedLog;

                try
                {
                    string testFile = Path.Combine("output", this.GetOutputFilename(command, task)); 
                    result.FileSizeBytes = new FileInfo(testFile).Length;
                }
                catch (Exception e)
                {
                    ConsoleOutput.WriteLine(e.ToString(), ConsoleColor.Red);
                }

                this.WriteLogData(result.Log, task, command);

                decimal time = Math.Round((decimal)watch.ElapsedMilliseconds / 1000, 2);
                ConsoleOutput.WriteLine(string.Format("   {0} - Task Complete. Took: {1} seconds", command.CommandId, time), ConsoleColor.Green);
                result.ExecutionTime = watch.ElapsedMilliseconds / 1000;
                result.TaskInfo = task;

                results.Add(result);
            }

            return results;
        }
        
        private Task<StringBuilder> RunHandBrakeCli(string arguments)
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
                lock (this.lockObject)
                {
                    ConsoleOutput.ClearLine();
                    Console.WriteLine("   Done");

                    isDone = true;
                    watch.Stop();
                    tcs.SetResult(logBuffer);
                    process.Dispose();
                }
            };

            process.OutputDataReceived += (sender, args) =>
            {
                lock (this.lockObject)
                {
                    if (!isDone)
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write("   " + args.Data);
                        }
                    }
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

        private string GetOutputFilename(TaskCommand command, TaskItem task)
        {
            return string.Format("{0}_{1}.{2}", task.TaskId, command.CommandId, task.SourceFile);
        }
        
        private string PreProcessCommand(TaskItem task, TaskCommand command)
        {
            string result = command.Command;
            
            result = result.Replace("{source}", string.Format("\"{0}\"", Path.Combine("sources", task.SourceFile)));
            result = result.Replace("{output_file}", Path.Combine("output", this.GetOutputFilename(command, task)));

            return result;
        }

        private void WriteLogData(string log, TaskItem task, TaskCommand command)
        {
            try
            {
                string filename = Path.Combine("reports/logs", this.GetOutputFilename(command, task) + ".log");
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine(log);
                }
            }
            catch (Exception e)
            {
                ConsoleOutput.WriteLine(e.ToString(), ConsoleColor.Red);
            }
        }
    }
}
