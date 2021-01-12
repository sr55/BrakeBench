// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigService.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// <summary>
//   Defines the ConfigService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Services.Config
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using BrakeBench.Services.Config.Interfaces;
    using BrakeBench.Services.Config.Model;

    using Newtonsoft.Json;

    public class ConfigService : IConfigService
    {
        private readonly Dictionary<int, TaskItem> taskList = new Dictionary<int, TaskItem>();

        public ConfigService()
        {
        }

        public List<TaskItem> GetConfig()
        {
            return this.taskList.Values.ToList();
        }

        public TaskItem GetConfig(int task)
        {
            if (this.taskList.ContainsKey(task))
            {
                return this.taskList[task];
            }

            return null;
        }

        public void LoadConfig()
        {
            using (StreamReader reader = new StreamReader("config.json"))
            {
                string tasks = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(tasks))
                {
                    Config config = JsonConvert.DeserializeObject<Config>(tasks);
                    if (this.taskList != null)
                    {
                        foreach (TaskItem task in config.Tasks)
                        {
                            this.taskList.Add(task.TaskId, task);
                        }
                    }

                    // TODO handle other config here.
                }
            }
        }

        public void SaveConfig()
        {
            using (StreamWriter writer = new StreamWriter("config.json"))
            {
                Config config = new Config();
                config.Tasks = new List<TaskItem>();
                config.Tasks.Add(new TaskItem(){ CustomCommands = new List<TaskCommand>(), Description = "Desc"});

                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                string json = JsonConvert.SerializeObject(config, Formatting.Indented, settings);
                writer.Write(json);
            }
        }
    }
}
