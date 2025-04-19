using Core.Services.Finance;
using Core.Services.Payment;
using Core.Services.Payment.Interfaces;
using Core.Services.Payment.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {   
        // Add services
        services.AddScoped<BalanceService>();
        services.AddScoped<CurrencyRatesService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<WalletService>();
        
        // Add provider factory
        services.AddScoped<PaymentProviderFactory>();
        
        return services;
    }
} 