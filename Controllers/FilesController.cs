using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileAgent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FileAgent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        [HttpGet("GetItemsAtPath")]
        public IActionResult GetItemsAtPath(string path)
        {
            List<String> files = Directory.GetFiles(path).ToList();
            List<String> directories = Directory.GetDirectories(path).ToList();
            if (files.Count > 0 || directories.Count > 0)
            {
                ListingModel listingModel = new ListingModel();
                listingModel.Files = files;
                listingModel.Directories = directories;
                return Ok(listingModel);
            }

            return NotFound();
        }

    }
}
