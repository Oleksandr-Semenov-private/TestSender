using Newtonsoft.Json;

namespace NoWayMailerAPI.Data;

public record WklejResponse
{
	[JsonProperty("error")]
	public int Error { get; set; }
	
	[JsonProperty("id")]
	public int Id { get; set; }
	
	[JsonProperty("shorturl")]
	public string Short { get; set; }
}
