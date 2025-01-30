using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Public;
using Riok.Mapperly.Abstractions;

namespace Core.Contracts.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class GameRecordMapper
{
    public partial GameRecord ToData(GameRecordEntity entity);
}