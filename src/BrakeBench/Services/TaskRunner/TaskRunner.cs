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
    using System.Linq;
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

        private bool isDone;

        public async Task<List<CommandResult>> RunTaskSet(TaskItem task)
        {
            List<CommandResult> results = new List<CommandResult>();
            ConsoleOutput.WriteLine(string.Format("Running Task: {0}", task.Name), ConsoleColor.Cyan);
            ConsoleOutput.WriteLine(string.Format(" - {0}", task.Description), ConsoleColor.Cyan);
    
            switch (task.Mode)
            {
                case TaskType.Benchmark:
                    if (task.CustomCommands.Count > 1)
                    {
                        ConsoleOutput.WriteLine("Benchmark mode can only have 1 custom command.", ConsoleColor.Red);
                        return null;
                    }

                    List<TaskCommand> preparedCommands = this.PrepareBenchmarkCommands(task.CustomCommands.FirstOrDefault(), task.Runs);

                    ConsoleOutput.WriteLine(string.Format(" - {0} command(s) to execute.", preparedCommands.Count), ConsoleColor.Cyan);
                    ConsoleOutput.WriteLine("-----------------------------------------------------");

                    foreach (TaskCommand command in preparedCommands)
                    {
                        CommandResult result = this.RunTaskCommand(task, command, command.AssignedId).Result;
                        results.Add(result);
                    }

                    break;
                case TaskType.QualitySweep:
                    if (task.CustomCommands.Count > 1)
                    {
                        ConsoleOutput.WriteLine("Quality Sweep mode can only have 1 custom command.", ConsoleColor.Red);
                        return null;
                    }

                    preparedCommands = this.PrepareQualitySweepTaskCommand(task.CustomCommands.FirstOrDefault());

                    ConsoleOutput.WriteLine(string.Format(" - {0} command(s) to execute.", preparedCommands.Count), ConsoleColor.Cyan);
                    ConsoleOutput.WriteLine("-----------------------------------------------------");

                    foreach (TaskCommand command in preparedCommands)
                    {
                        CommandResult result = this.RunTaskCommand(task, command, command.UsedQuality).Result;
                        results.Add(result);
                    }

                    break;
            }

            return results;
        }

        private async Task<CommandResult> RunTaskCommand(TaskItem task, TaskCommand command, int taskNameValue)
        {
            Stopwatch watch = Stopwatch.StartNew();
            this.isDone = false;

            // Execute the run
            CommandResult result = new CommandResult();
            ConsoleOutput.WriteLine(string.Format("{1}{1} - Running Command: {0}", command.Name, Environment.NewLine), ConsoleColor.Cyan);

            string commandString = this.PreProcessCommand(task, command, taskNameValue);
            StringBuilder logData = await this.RunHandBrakeCli(commandString);
            watch.Stop();

            // Gather the results of the run.
            result.Log = logData.ToString();
            ProcessedLog processedLog = logService.ProcessLog(logData);
            processedLog.Command = command;
            result.ProcessedLog = processedLog;

            try
            {
                string testFile = Path.Combine("output", this.GetOutputFilename(command, task, taskNameValue));
                result.FileSizeBytes = new FileInfo(testFile).Length;
            }
            catch (Exception e)
            {
                ConsoleOutput.WriteLine(e.ToString(), ConsoleColor.Red);
            }

            this.WriteLogData(result.Log, task, command, taskNameValue);

            decimal time = Math.Round((decimal)watch.ElapsedMilliseconds / 1000, 2);
            ConsoleOutput.WriteLine(string.Format("   - Task Complete. Took: {0} seconds", time), ConsoleColor.Green);
            result.ExecutionTime = watch.ElapsedMilliseconds / 1000;
            result.TaskInfo = task;

            return result;
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

        private string GetOutputFilename(TaskCommand command, TaskItem task, int runCount)
        {
            return string.Format("{0}_{1}.{2}", task.TaskId, runCount, task.SourceFile);
        }
        
        private string PreProcessCommand(TaskItem task, TaskCommand command, int runCount)
        {
            string result = command.Command;
            
            result = result.Replace("{source}", string.Format("\"{0}\"", Path.Combine("sources", task.SourceFile)));
            result = result.Replace("{output_file}", Path.Combine("output", this.GetOutputFilename(command, task, runCount)));

            return result;
        }

        private List<TaskCommand> PrepareBenchmarkCommands(TaskCommand baseCommand, int runCount)
        {
            List<TaskCommand> preparedCommands = new List<TaskCommand>();

            for (int i = 1; i <= runCount; i++)
            {
                TaskCommand command = new TaskCommand(baseCommand);
                command.Name = string.Format(command.Name, i);
                command.AssignedId = i;
                preparedCommands.Add(command);
            }

            return preparedCommands;
        }

        private List<TaskCommand> PrepareQualitySweepTaskCommand(TaskCommand command)
        {
            List<TaskCommand> preparedCommands = new List<TaskCommand>();

            for (int i = command.QualityMin; i <= command.QualityMax; i++)
            {
                TaskCommand preparedCommand = new TaskCommand(command);

                preparedCommand.Command = command.Command.Replace("{quality}", i.ToString());
                preparedCommand.UsedQuality = i;
                preparedCommand.Name = preparedCommand.Name.Replace("{0}", i.ToString()); // TODO Refactor
                preparedCommands.Add(preparedCommand);
            }

            return preparedCommands;
        }

        private void WriteLogData(string log, TaskItem task, TaskCommand command, int runCount)
        {
            try
            {
                string filename = Path.Combine("reports/logs", this.GetOutputFilename(command, task, runCount) + ".log");
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
