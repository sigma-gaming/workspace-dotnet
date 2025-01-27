using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Public;
using Riok.Mapperly.Abstractions;

namespace Core.Contracts.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class BalanceMapper
{
    public partial Balance ToData(BalanceEntity entity);
    public partial BalancePublic ToPublic(BalanceEntity entity);
    public partial BalancePublic ToPublic(Balance data);
}