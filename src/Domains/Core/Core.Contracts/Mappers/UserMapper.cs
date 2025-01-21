using Core.Contracts.Data;
using Core.Contracts.Entities;
using Riok.Mapperly.Abstractions;

namespace Core.Contracts.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserMapper
{
    public partial UserData ToData(User user);
}