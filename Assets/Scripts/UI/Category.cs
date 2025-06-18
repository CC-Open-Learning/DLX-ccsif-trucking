using System;
using System.Collections.Generic;
using UnityEngine;

namespace VARLab.CCSIF
{
    /// <summary>
    /// Represents a category that contains its own tasks in a progress indicator.
    /// </summary>
    [Serializable]
    public class Category
    {
        /// <summary>
        /// The name of the category.
        /// </summary>
        [field: SerializeField]
        public string Name { get; private set; }

        /// <summary>
        /// The tasks attributed to this category.
        /// </summary>
        public List<Task> Tasks;

        public Category(string name)
        {
            Name = name;
            Tasks = new List<Task>();
        }
    }
}
