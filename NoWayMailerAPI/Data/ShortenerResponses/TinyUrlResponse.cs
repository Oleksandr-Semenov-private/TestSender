using Newtonsoft.Json;

namespace NoWayMailerAPI.Data.ShortenerResponses;

public class TinyUrlResponse
{
	[JsonProperty("data")]
	public Data data { get; set; }

	[JsonProperty("code")]
	public int code { get; set; }

	[JsonProperty("errors")]
	public List<object> errors { get; set; }
}

public class Analytics
{
	[JsonProperty("enabled")]
	public bool enabled { get; set; }

	[JsonProperty("public")]
	public bool @public { get; set; }
}

public class Data
{
	[JsonProperty("domain")]
	public string domain { get; set; }

	[JsonProperty("alias")]
	public string alias { get; set; }

	[JsonProperty("deleted")]
	public bool deleted { get; set; }

	[JsonProperty("archived")]
	public bool archived { get; set; }

	[JsonProperty("analytics")]
	public Analytics analytics { get; set; }

	[JsonProperty("tags")]
	public List<object> tags { get; set; }

	[JsonProperty("created_at")]
	public DateTime created_at { get; set; }

	[JsonProperty("expires_at")]
	public object expires_at { get; set; }

	[JsonProperty("tiny_url")]
	public string tiny_url { get; set; }

	[JsonProperty("url")]
	public string url { get; set; }
}

