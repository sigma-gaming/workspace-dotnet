using Core.Contracts.Data;
using Core.Contracts.Entities;
using Riok.Mapperly.Abstractions;

namespace Core.Contracts.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class GlobalTaskMapper
{
    public partial GlobalTask ToData(GlobalTaskEntity entity);
}