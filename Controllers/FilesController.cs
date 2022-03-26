using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FileAgent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FileAgent.Controllers
{
    public class Path
    {
        public string PathString { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        [HttpGet("GetItemsAtPath")]
        
        public IActionResult GetItemsAtPath([FromBody] Path path)
        {
            List<String> files = Directory.GetFiles(path.PathString).ToList();
            List<String> directories = Directory.GetDirectories(path.PathString).ToList();
            if (files.Count > 0 || directories.Count > 0)
            {
                ListingModel listingModel = new ListingModel();
                listingModel.Files = files;
                listingModel.Directories = directories;
                return Ok(JsonSerializer.Serialize(listingModel));
            }

            return NotFound();
        }

    }
}
