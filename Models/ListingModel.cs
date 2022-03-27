using System;
using System.Collections.Generic;

namespace FileAgent.Models
{
    public class ListingModel
    {
        public List<FileInformation> Files { get; set; }
        public List<String> Directories { get; set; }

        public ListingModel() 
        {
            Files = new List<FileInformation>();
            Directories = new List<String>();
        }
        public class FileInformation
        {
            public string Name { get; set; }
            public string Size { get; set; }
            public FileInformation(string name, string size)
            {
                this.Name = name;
                this.Size = size;
            }

        }
    }
}