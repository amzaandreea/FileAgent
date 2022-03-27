using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using FileAgent.Models;
using Microsoft.AspNetCore.Mvc;
using static FileAgent.Models.ListingModel;

namespace FileAgent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        [HttpPost("GetItemsAtPath")]
        public IActionResult GetItemsAtPath([FromBody] RequestModel reqModel)
        {
            DirectoryInfo di = new DirectoryInfo(reqModel.PathString);
            List<FileInformation> files = di.GetFiles("*", SearchOption.TopDirectoryOnly).Select(f => new FileInformation(f.FullName, (f.Length/1024/1024).ToString())).ToList();
            List<String> directories = Directory.GetDirectories(reqModel.PathString).ToList();
            if (files.Count > 0 || directories.Count > 0)
            {
                ListingModel listingModel = new ListingModel();
                listingModel.Files = files;
                listingModel.Directories = directories;
                return Ok(JsonSerializer.Serialize(listingModel));
            }

            return NotFound();
        }

        [HttpPost("DownloadFile")]//sa fim atenti la extensii(e text.txt nu text)
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

        [HttpPost("FindItem")]
        public IActionResult FindItem([FromBody] RequestModel reqModel)
        {
            string startFolder = Path.GetPathRoot(Environment.SystemDirectory);
            DirectoryInfo dir = new DirectoryInfo(startFolder);

            var list = GetFiles(startFolder, reqModel.FileName);

            return Ok(JsonSerializer.Serialize(list));
        }

        [HttpPost("ListProcesses")]
        public IActionResult ListProcesses()
        {
            Process[] processCollection = Process.GetProcesses();

            var list = processCollection.Select(p => p.ProcessName).ToList();

            return Ok(JsonSerializer.Serialize(list));
        }

        [HttpPost("ListProcessesSorted")]
        public IActionResult ListProcessesSorted()
        {
            Process[] processCollection = Process.GetProcesses();

            var list = processCollection.Where(x => x.SessionId == Process.GetCurrentProcess().SessionId)
                    .OrderByDescending(x => x.PagedMemorySize64)
                    .Select(x => x.ProcessName).ToList();//ram

            return Ok(JsonSerializer.Serialize(list));
        }

        private static IEnumerable<string> GetFiles(string root, string searchPattern)
        {
            Stack<string> pending = new Stack<string>();
            pending.Push(root);
            while (pending.Count != 0)
            {
                var path = pending.Pop();
                string[] next = null;
                try
                {
                    next = Directory.GetFiles(path, searchPattern);
                }
                catch { }
                if (next != null && next.Length != 0)
                    foreach (var file in next) yield return file;
                try
                {
                    next = Directory.GetDirectories(path);
                    foreach (var subdir in next) pending.Push(subdir);
                }
                catch { }
            }
        }

        public double GetUsage(Process process)
        {
            // Getting information about current process
            //var process = Process.GetCurrentProcess();

            // Preparing variable for application instance name
            var name = string.Empty;

            foreach (var instance in new PerformanceCounterCategory("Process").GetInstanceNames())
            {
                if (instance.StartsWith(process.ProcessName))
                {
                    using (var processId = new PerformanceCounter("Process", "ID Process", instance, true))
                    {
                        if (process.Id == (int)processId.RawValue)
                        {
                            name = instance;
                            break;
                        }
                    }
                }
            }

            var cpu = new PerformanceCounter("Process", "% Processor Time", name, true);
           // var ram = new PerformanceCounter("Process", "Private Bytes", name, true);

            // Getting first initial values
            cpu.NextValue();
            //ram.NextValue();

            // Creating delay to get correct values of CPU usage during next query
            Thread.Sleep(500);

            dynamic result = new ExpandoObject();

            // If system has multiple cores, that should be taken into account
            result.CPU = Math.Round(cpu.NextValue() / Environment.ProcessorCount, 2);
            // Returns number of MB consumed by application
            //result.RAM = Math.Round(ram.NextValue() / 1024 / 1024, 2);

            return result.CPU;
        }
    }
}
