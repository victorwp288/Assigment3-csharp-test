using System.Collections.Generic;
using Assigment3.models;

namespace Assigment3
{
    public static class DataStore
    {
        public static List<Category> Categories { get; set; } = new List<Category>();
        private static int nextId = 4; // IDs start from 4 since initial data has IDs up to 3.

        static DataStore()
        {
            // Initialize data
            Categories.Add(new Category { Id = 1, Name = "Beverages" });
            Categories.Add(new Category { Id = 2, Name = "Condiments" });
            Categories.Add(new Category { Id = 3, Name = "Confections" });
        }

        public static int GetNextId()
        {
            return nextId++;
        }
    }
}
