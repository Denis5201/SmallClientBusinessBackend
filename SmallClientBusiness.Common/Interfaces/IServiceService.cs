using SmallClientBusiness.Common.Dto;

namespace SmallClientBusiness.Common.Interfaces;

public interface IServiceService
{
    Task<List<Service>> GetServices(Guid workerId);
    Task<List<Service>> GetDefaultServices();
    Task<List<Service>> GetCustomServices(Guid workerId);
    Task<Service> GetService(Guid workerId, Guid serviceId);

    Task CreateService(Guid workerId, CreateService model);
    Task EditService(Guid workerId, Guid serviceId, EditService model);
    Task DeleteService(Guid workerId, Guid serviceId);
}