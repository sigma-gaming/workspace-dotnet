using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Public;
using Riok.Mapperly.Abstractions;

namespace Core.Contracts.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ReferralCampaignMapper
{
    public partial ReferralCampaign ToData(ReferralCampaignEntity entity);
}