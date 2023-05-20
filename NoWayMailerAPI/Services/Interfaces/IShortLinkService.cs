using NoWayMailerAPI.Data;

namespace NoWayMailerAPI.Services.Interfaces;

public interface IShortLinkService
{
	Task<string> GetShortLink(string link, EbayTemplate template = EbayTemplate.Custom);
}