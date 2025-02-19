using Core.WS.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Core.WS.Extensions;

public static class ClientProxyExtensions
{
    public static Task TypedSendAsync(this IClientProxy proxy, Event sendEvent,
        CancellationToken ct = default)
    {
        return proxy.SendAsync(sendEvent.Name.Name(), sendEvent.Payload, ct);
    }
}