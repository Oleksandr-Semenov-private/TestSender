using Newtonsoft.Json;

namespace NoWayMailerAPI.Data.ShortenerResponses;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public record RedirectUrlResponse
{
	[JsonProperty("idString")]
	public string idString { get; set; }

	[JsonProperty("path")]
	public string path { get; set; }

	[JsonProperty("title")]
	public object title { get; set; }

	[JsonProperty("icon")]
	public object icon { get; set; }

	[JsonProperty("archived")]
	public bool archived { get; set; }

	[JsonProperty("originalURL")]
	public string originalURL { get; set; }

	[JsonProperty("iphoneURL")]
	public object iphoneURL { get; set; }

	[JsonProperty("androidURL")]
	public object androidURL { get; set; }

	[JsonProperty("splitURL")]
	public object splitURL { get; set; }

	[JsonProperty("expiresAt")]
	public object expiresAt { get; set; }

	[JsonProperty("expiredURL")]
	public object expiredURL { get; set; }

	[JsonProperty("redirectType")]
	public object redirectType { get; set; }

	[JsonProperty("cloaking")]
	public object cloaking { get; set; }

	[JsonProperty("source")]
	public object source { get; set; }

	[JsonProperty("AutodeletedAt")]
	public object AutodeletedAt { get; set; }

	[JsonProperty("createdAt")]
	public DateTime createdAt { get; set; }

	[JsonProperty("updatedAt")]
	public DateTime updatedAt { get; set; }

	[JsonProperty("DomainId")]
	public int DomainId { get; set; }

	[JsonProperty("OwnerId")]
	public int OwnerId { get; set; }

	[JsonProperty("secureShortURL")]
	public string secureShortURL { get; set; }

	[JsonProperty("shortURL")]
	public string shortURL { get; set; }
}

