using SmallClientBusiness.Common.Dto;

namespace SmallClientBusiness.Common.Interfaces;

public interface ISubscribeService
{
    Task SetSubscribingStatus(string userId, bool isSubscribing);

    Task<bool> IsSubscribing(Guid userId);
    Task<Subscribe> GetSubscribeInformation(Guid userId);
}