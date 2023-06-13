using SmallClientBusiness.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.Common.Interfaces
{
    public interface IAuthService
    {
        Task<TokenPair> CreateWorker(CreateWorkerUser createWorker);

        Task<TokenPair> Login(LoginCredentials credentials);

        Task<TokenPair> Refresh(TokenPair oldTokens);

        Task Logout(string userId);
    }
}
