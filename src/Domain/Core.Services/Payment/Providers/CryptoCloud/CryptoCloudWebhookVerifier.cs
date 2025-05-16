using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Core.Services.Payment.Providers.CryptoCloud;

/// <summary>
///     Verifies CryptoCloud webhooks before they are queued
/// </summary>
public class CryptoCloudWebhookVerifier(
    IOptions<CryptoCloudOptions> options,
    ILogger<CryptoCloudWebhookVerifier> logger)
{
    private readonly CryptoCloudOptions options = options.Value;

    /// <summary>
    ///     Verifies the JWT token and the status of the webhook request
    /// </summary>
    public bool VerifyWebhook(CryptoCloudWebhookRequest request)
    {
        // Verify the JWT token
        if (!VerifyToken(request.Token, request.InvoiceId))
        {
            logger.LogWarning("Invalid JWT token for CryptoCloud webhook with invoice ID {InvoiceId}",
                request.InvoiceId);
            return false;
        }

        // Validate request status
        if (request.Status != "success")
        {
            logger.LogWarning("Non-success status received from CryptoCloud: {Status} for invoice {InvoiceId}",
                request.Status, request.InvoiceId);
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Verifies the JWT token from the webhook request
    /// </summary>
    private bool VerifyToken(string token, string invoiceId)
    {
        try
        {
            // Set up JWT token validation parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(5), // Token valid for 5 minutes as per documentation
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
            };

            // Create the JWT token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Validate the token
            tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            // Verify the token contains the invoice ID
            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims.Any(claim => claim.Value == invoiceId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to verify CryptoCloud webhook JWT token for invoice {InvoiceId}", invoiceId);
            return false;
        }
    }
}