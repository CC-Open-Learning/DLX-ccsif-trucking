using System;
using UnityEngine;

namespace VARLab.CCSIF
{
    /// <summary>
    /// Represents a task of a category in a progress indicator.
    /// </summary>
    [Serializable]
    public class Task
    {
        /// <summary>
        /// The name of the task.
        /// </summary>
        [field: SerializeField]
        public string Name { get; private set; }

        
        [field: SerializeField]
        public string Score { get; private set; }
        
        /// <summary>
        /// The completion status of the task.
        /// </summary>
        public bool Completed { get; set; }

        public Task(string name)
        {
            Name = name;
        }

        public Task(string name, string score)
        {
            Name = name;
            Score = score;
        }
    }
}
