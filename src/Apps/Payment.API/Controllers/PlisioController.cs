using Core.Contracts.Enums;
using Core.Services.Payment.Providers.Plisio;
using Core.Services.Payment.Webhooks;
using Microsoft.AspNetCore.Mvc;

namespace Payment.API.Controllers;

[ApiController]
public class PlisioController(PlisioWebhookVerifier webhookVerifier, PaymentWebhookOutbox webhookOutbox)
    : ControllerBase
{
    [HttpPost]
    [Route("/providers/plisio/webhook")]
    public async Task<IActionResult> HandleWebhook([FromBody] PlisioWebhookRequest request)
    {
        // Get the signature from the header
        if (!Request.Headers.TryGetValue("X-Plisio-Signature", out var signatureValues))
            return BadRequest("Missing signature header");

        var signature = signatureValues.FirstOrDefault();
        if (string.IsNullOrEmpty(signature)) return BadRequest("Invalid signature header");

        if (!webhookVerifier.VerifySignature(request, signature)) return BadRequest();

        // Processed by Core.Tasks, the provider gets the response without waiting for it
        await webhookOutbox.EnqueueAsync(PaymentProvider.Plisio, request.TxnId, request);

        return Ok();
    }
}