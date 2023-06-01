using MailerRobot.Bot;
using NoWayMailerAPI.Data;
using NoWayMailerAPI.Data.ShortenerResponses;
using NoWayMailerAPI.Services.Interfaces;

namespace NoWayMailerAPI.Services;

public class ShortLinkService : IShortLinkService
{
	private readonly HttpClient _client;

	public ShortLinkService()
	{
		_client = new HttpClient();
	}

	public async Task<string> GetShortLink(string link, ShrtCoLink type, EbayTemplate template = EbayTemplate.Custom)
	{                                                                                                           
		//return template == EbayTemplate.Custom ? await UseShrtcoDe(link,type) : await UseLinq(link);
		return await UseShrtcoDe(link, type);

		//return await UseRedirectUrl(link);
		//return await UseLinq(link);
		//return await UseN9Cl(link);
		//return await UseShrtcoDe(link);

	}

	private async Task<string> UseLinq(string link)
	{
		_client.BaseAddress = new Uri("https://api.encurtador.dev/encurtamentos");

		var content = JsonContent.Create(new
		{
			url = link
		});

		var msg = await _client.PostAsync("", content);

		var response = await ReadResponseAsync<LinqResponse>(msg);

		var reponseLink = "https://" + response.UrlEncurtada;
		return reponseLink;
	}
	private async Task<string> UseRedirectUrl(string link)
	{
		_client.BaseAddress = new Uri("https://api.short.io/");
		_client.DefaultRequestHeaders.Add("Authorization", "sk_dqL5Ng8Ifkm3hkbH");
		
		var requestBody = new
		{
			domain = "9r59.short.gy",
			originalURL = link
		};

		var msg = await _client.PostAsJsonAsync("links", requestBody);

		var response = await ReadResponseAsync<RedirectUrlResponse>(msg);

		return response.shortURL;

	}
	
	private async Task<string> UseL8Nu(string link)
	{
		_client.BaseAddress = new Uri("https://l8.nu/");

		var msg = await _client.GetAsync("yourls-api.php?action=shorturl&format=json&url=" + link);

		var response = await ReadResponseL8NuAsync<L8NuResponse>(msg);

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

	private async Task<string> UseShrtcoDe(string link, ShrtCoLink type)
	{
		_client.BaseAddress = new Uri("https://api.shrtco.de");

		var msg = await _client.GetAsync("v2/shorten?url=" + link);

		var response = await ReadResponseAsync<ShortCo>(msg);

		return type switch
		{
			ShrtCoLink.UseShrtCo => response.result.full_short_link,
			ShrtCoLink.Use9Nr => response.result.full_short_link2,
			ShrtCoLink.UseShinyLink => response.result.full_short_link3,
			_ => response.result.full_short_link
		};
	}

	private static async Task<TResponse> ReadResponseL8NuAsync<TResponse>(HttpResponseMessage msg,
		CancellationToken ct = default) where TResponse : class
	{
		var response = await msg.Content.ReadAsStringAsync(ct);

		var firstIndex = response.IndexOf('{');
		var secondIndex = response.IndexOf('{', firstIndex + 1);

		var result = response.Substring(secondIndex);

		return IsJson(result) ? result.Deserialize<TResponse>() : default!;
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