using Newtonsoft.Json;

namespace DataSubscibe.Core.Domain.Models
{
    public class Timeline
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string[] Value { get; set; }
    }
}