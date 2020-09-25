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
        private readonly Dictionary<int, TaskSet> taskList = new Dictionary<int, TaskSet>();

        public ConfigService()
        {
        }

        public List<TaskSet> GetConfig()
        {
            return this.taskList.Values.ToList();
        }

        public TaskSet GetConfig(int task)
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
                    List<TaskSet> jsonList = JsonConvert.DeserializeObject<List<TaskSet>>(tasks);
                    if (this.taskList != null)
                    {
                        foreach (TaskSet task in jsonList)
                        {
                            this.taskList.Add(task.TaskId, task);
                        }
                    }
                }
            }
        }

        public void SaveConfig()
        {
            using (StreamWriter writer = new StreamWriter("config.json"))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                string json = JsonConvert.SerializeObject(this.taskList.Values.ToList(), Formatting.Indented, settings);
                writer.Write(json);
            }
        }
    }
}
