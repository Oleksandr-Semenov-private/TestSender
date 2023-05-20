using System.Text.Json;
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

		return await UseL8Nu(link);

		//return await UseShrtcoDe(link);

		//return await UseN9Cl(link);
	}

	private async Task<string> UseL8Nu(string link)
	{
		_client.BaseAddress = new Uri("https://l8.nu/");

		var msg = await _client.GetAsync("yourls-api.php?action=shorturl&format=json&url=" + link);

		var response = await ReadResponseAsync<L8NuResponse>(msg);

		return response.shorturl;
	}
	private async Task<string> UseN9Cl(string link)
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

	private async Task<string> UseShrtcoDe(string link)
	{
		_client.BaseAddress = new Uri("https://api.shrtco.de");

		var msg = await _client.GetAsync("v2/shorten?url=" + link);

		var response = await ReadResponseAsync<ShortCo>(msg);

		return response.result.full_short_link2;
	}

	private static async Task<TResponse> ReadResponseAsync<TResponse>(HttpResponseMessage msg,
		CancellationToken ct = default) where TResponse : class
	{
		var response = await msg.Content.ReadAsStringAsync(ct);
		
		int firstIndex = response.IndexOf('{');
		int secondIndex = response.IndexOf('{', firstIndex + 1);

		string result = response.Substring(secondIndex);


		return IsJson(result) ? result.Deserialize<TResponse>() : default!;
	}

	private static bool IsJson(string response)
	{
		return (response.StartsWith("{") && response.TrimEnd().EndsWith("}")) ||
				(response.StartsWith("[") && response.TrimEnd().EndsWith("]"));
	}
}