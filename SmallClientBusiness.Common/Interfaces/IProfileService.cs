using SmallClientBusiness.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.Common.Interfaces
{
    public interface IProfileService
    {
        Task<WorkerProfile> GetWorkerProfile(string userId);

        Task ChangeProfile(string userId, ChangeUser changeWorker);

        Task ChangePassword(string userId, ChangePassword changePassword);
        
        Task UploadAvatar(Guid userId, AvatarUpload avatarUpload, string path);
        Task<byte[]> LoadAvatar(Guid userId, string path);
        Task DeleteAvatar(Guid userId, string path);
    }
}
