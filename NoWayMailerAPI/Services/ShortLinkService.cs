using System.Net.Http.Headers;
using MailerRobot.Bot;
using MailerRobot.Bot.Domain.Responses;
using NoWayMailerAPI.Data;
using NoWayMailerAPI.Services.Interfaces;

namespace NoWayMailerAPI.Services;

public class ShortLinkService : IShortLinkService
{
	private readonly HttpClient _client;

	public ShortLinkService()
	{
		_client = new HttpClient();
	}

	public async Task<string> GetShortLink(string link)
	{
		_client.BaseAddress = new Uri("https://n9.cl/");

		var content = JsonContent.Create(new
		{
			url = link
		});

		var msg = await _client.PostAsync("api/short", content);

		var response = await ReadResponseAsync<N9Response>(msg);

		return response.Short;
	}

	private static async Task<TResponse> ReadResponseAsync<TResponse>(HttpResponseMessage msg,
		CancellationToken ct = default) where TResponse : class
	{
		var response = await msg.Content.ReadAsStringAsync(ct);

		return IsJson(response) ? response.Deserialize<TResponse>() : default!;
	}

	private static bool IsJson(string response)
	{
		return (response.StartsWith("{") && response.TrimEnd().EndsWith("}")) ||
				(response.StartsWith("[") && response.TrimEnd().EndsWith("]"));
	}
}