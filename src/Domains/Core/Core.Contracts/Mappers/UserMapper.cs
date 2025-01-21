using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Public;
using Riok.Mapperly.Abstractions;

namespace Core.Contracts.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserMapper
{
    public partial User ToData(UserEntity userEntity);
    public partial UserPublic ToPublic(UserEntity userEntity);
    public partial UserPublic ToPublic(User user);
}