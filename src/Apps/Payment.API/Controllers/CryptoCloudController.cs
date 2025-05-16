using Core.Contracts.Enums;
using Core.Services.Payment.Providers.CryptoCloud;
using Core.Services.Payment.Webhooks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Payment.API.Controllers;

/// <summary>
/// Controller for handling CryptoCloud payment provider webhooks
/// </summary>
[ApiController]
public class CryptoCloudController : ControllerBase
{
    private readonly CryptoCloudWebhookVerifier webhookVerifier;
    private readonly PaymentWebhookOutbox webhookOutbox;
    private readonly ILogger<CryptoCloudController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoCloudController"/> class
    /// </summary>
    /// <param name="webhookVerifier">CryptoCloud webhook verifier</param>
    /// <param name="webhookOutbox">Outbox for verified webhooks</param>
    /// <param name="logger">Logger</param>
    public CryptoCloudController(
        CryptoCloudWebhookVerifier webhookVerifier,
        PaymentWebhookOutbox webhookOutbox,
        ILogger<CryptoCloudController> logger)
    {
        this.webhookVerifier = webhookVerifier;
        this.webhookOutbox = webhookOutbox;
        this.logger = logger;
    }

    /// <summary>
    /// Handles webhook notifications from CryptoCloud payment provider
    /// </summary>
    /// <param name="request">The webhook request data</param>
    /// <returns>Success response if the webhook was verified and queued, otherwise bad request</returns>
    [HttpPost]
    [Route("/providers/cryptocloud/webhook")]
    public async Task<IActionResult> HandleWebhook([FromBody] CryptoCloudWebhookRequest request)
    {
        logger.LogInformation("Received CryptoCloud webhook for invoice {InvoiceId}", request.InvoiceId);

        if (!webhookVerifier.VerifyWebhook(request))
        {
            logger.LogWarning("Rejected CryptoCloud webhook for invoice {InvoiceId}", request.InvoiceId);
            return BadRequest(new { status = "error", message = "Failed to verify webhook" });
        }

        await webhookOutbox.EnqueueAsync(PaymentProvider.CryptoCloud, request.InvoiceId, request);

        logger.LogInformation("Queued CryptoCloud webhook for invoice {InvoiceId}", request.InvoiceId);
        return Ok(new { status = "success" });
    }
}