using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WebApplication1.Data.Entities
{

    public class AskEntity : BaseEntity
{
    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonProperty("price")]
    public float Price { get; set; }

    [JsonProperty("volume")]
    public int Volume { get; set; }
}
}
