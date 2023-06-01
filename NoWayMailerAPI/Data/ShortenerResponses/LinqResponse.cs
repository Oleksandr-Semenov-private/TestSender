using Newtonsoft.Json;

namespace NoWayMailerAPI.Data.ShortenerResponses;

public record LinqResponse
{
	
	[JsonProperty("UrlEncurtada")]
	public string UrlEncurtada { get; set; }
}