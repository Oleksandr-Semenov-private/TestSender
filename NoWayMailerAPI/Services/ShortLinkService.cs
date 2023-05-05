using System.Net.Http.Headers;
using MailerRobot.Bot;
using MailerRobot.Bot.Domain.Responses;
using NoWayMailerAPI.Data;
using NoWayMailerAPI.Services.Interfaces;
using RestSharp;

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
		_client.BaseAddress = new Uri("https://wklej.to/");

		var content = JsonContent.Create(new
		{
			url = link
		});

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "d08568b9cb0317d6829f");

		try
		{
			var msg = await _client.PostAsync("api/url/add", content);

			var response = await ReadResponseAsync<WklejResponse>(msg);

			return response.Short;
		}
		catch(Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		/*_client.BaseAddress = new Uri("https://n9.cl/");

		var content = JsonContent.Create(new
		{
			url = link
		});

		var msg = await _client.PostAsync("api/short", content);

		var response = await ReadResponseAsync<N9Response>(msg);

		return response.Short;*/
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