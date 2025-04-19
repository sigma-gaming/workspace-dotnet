using Core.Services.Payment.Providers.Plisio;
using Microsoft.AspNetCore.Mvc;

namespace Payment.API.Controllers;

[ApiController]
public class PlisioController(PlisioWebhookHandler webhookHandler) : ControllerBase
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

        // Handle the webhook
        var result = await webhookHandler.HandleWebhookAsync(request, signature);

        return result ? Ok() : BadRequest();
    }
}