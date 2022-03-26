using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using FileAgent.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileAgent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        [HttpGet("GetItemsAtPath")]
        public IActionResult GetItemsAtPath([FromBody] RequestModel path)
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

        [HttpGet("DownloadFile")]//sa fim atenti la extensii(e text.txt nu text)
        public IActionResult DownloadFile([FromBody] RequestModel reqModel)
        {
            try
            {
                Byte[] bytes = System.IO.File.ReadAllBytes(reqModel.PathString);
                string base64 = Convert.ToBase64String(bytes);
                return Ok(base64);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("UploadItem")]
        public IActionResult UploadItem([FromBody] RequestModel reqModel)
        {
            try
            {
                Byte[] fileContent = Convert.FromBase64String(reqModel.FileContent);
                using (FileStream fs = System.IO.File.Create(reqModel.PathString))
                {
                    fs.Write(fileContent, 0, fileContent.Length);
                }
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("DeleteItem")]
        public IActionResult DeleteItem([FromBody] RequestModel reqModel)
        {
            try
            {
                if (System.IO.File.Exists(Path.Combine(reqModel.PathString)))
                {
                    System.IO.File.Delete(Path.Combine(reqModel.PathString));
                    return Ok();
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("CreateItem")]
        public IActionResult CreateItem([FromBody] RequestModel reqModel)
        {
            try
            {

                if (reqModel.FileType == "dir")
                {
                    if (!Directory.Exists(reqModel.PathString))
                    {
                        Directory.CreateDirectory(reqModel.PathString);
                    }
                }
                if (reqModel.FileType == "file")
                {
                    if (!Directory.Exists(reqModel.PathString))
                    {
                        var fs = System.IO.File.Create(reqModel.PathString);
                        fs.Close();
                    }
                }

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
