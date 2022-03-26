using System;
using System.Collections.Generic;

namespace FileAgent.Models
{
    public class ListingModel
    {
        public List<String> Files { get; set; }
        public List<String> Directories { get; set; }

        public ListingModel() 
        {
            Files = new List<String>();
            Directories = new List<String>();
        }
    }
}