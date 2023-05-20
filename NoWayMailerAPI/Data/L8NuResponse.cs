using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace NoWayMailerAPI.Data;

public record L8NuResponse
(
	[property: JsonPropertyName("url")] Url url,
	[property: JsonPropertyName("status")] string status,
	[property: JsonPropertyName("message")]
	string message,
	[property: JsonPropertyName("title")] string title,
	[property: JsonPropertyName("shorturl")]
	string shorturl,
	[property: JsonPropertyName("statusCode")]
	int statusCode
);
public record Url(
	[property: JsonPropertyName("keyword")] string keyword,
	[property: JsonPropertyName("url")] string url,
	[property: JsonPropertyName("title")] string title,
	[property: JsonPropertyName("date")] string date,
	[property: JsonPropertyName("ip")] string ip
);