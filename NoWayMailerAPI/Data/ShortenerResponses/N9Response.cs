using Newtonsoft.Json;

namespace NoWayMailerAPI.Data.ShortenerResponses;

public record N9Response
{
	[JsonProperty("status")]
	public string Status { get; set; }
	
	[JsonProperty("permalink")]
	public string Permalink { get; set; }
	
	[JsonProperty("short")]
	public string Short { get; set; }
}