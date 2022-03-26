using System.Text.Json.Serialization;

namespace FileAgent.Models
{
    public class RequestModel
    {
        [JsonPropertyName("path")]
        public string PathString { get; set; }

        [JsonPropertyName("content")]
        public string FileContent { get; set; }

        [JsonPropertyName("type")]
        public string FileType { get; set; }

        public RequestModel() { }
    }
}
