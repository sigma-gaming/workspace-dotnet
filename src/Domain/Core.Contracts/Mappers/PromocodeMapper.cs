using Core.Contracts.Data;
using Core.Contracts.Entities;
using Riok.Mapperly.Abstractions;

namespace Core.Contracts.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PromocodeMapper
{
    public partial Promocode ToData(PromocodeEntity entity);
}