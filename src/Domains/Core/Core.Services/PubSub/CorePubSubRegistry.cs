using Common.PubSub.Abstractions;

namespace Core.Services.PubSub;

public class CorePubSubRegistry(IPubSubService pubSubService)
{
  public ITypedPubSub<object> BalanceUpdated { get; } =
      pubSubService.Create<object>("balance-updated");
}
