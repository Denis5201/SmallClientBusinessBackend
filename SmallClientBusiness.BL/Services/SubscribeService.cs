using Microsoft.EntityFrameworkCore;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Exceptions;
using SmallClientBusiness.Common.Interfaces;
using SmallClientBusiness.DAL;
using SmallClientBusiness.DAL.Entities;

namespace SmallClientBusiness.BL.Services;

public class SubscribeService : ISubscribeService
{
    private readonly AppDbContext _appDbContext;

    public SubscribeService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    
    public async Task<Subscribe> GetSubscribeInformation(Guid userId)
        {
            var worker = await _appDbContext.Workers
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync();
            if (worker == null)
            {
                throw new ItemNotFoundException("Аккаунт не найден");
            }

            var subscribe = await _appDbContext.SubscribeEntities
                .Where(e => e.UserId == userId)
                .FirstOrDefaultAsync();
            if (subscribe == null)
            {
                throw new ItemNotFoundException("У пользователя еще нет подписки");
            }

            return new Subscribe
            {
                CreateDate = subscribe.CreateDate
            };
        }

        public async Task SetSubscribingStatus(string userId, bool isSubscribing)
        {
            var user = await _appDbContext.Users
                .Where(x => x.Id == Guid.Parse(userId))
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ItemNotFoundException("Аккаунт не найден");
            }
            
            var worker = await _appDbContext.Workers
                .Where(x => x.Id == Guid.Parse(userId))
                .FirstOrDefaultAsync();
            if (worker == null)
            {
                throw new ItemNotFoundException("Аккаунт не найден");
            }
            
            worker.IsSubscribing = isSubscribing;
            
            if (isSubscribing)
            {
                await _appDbContext.SubscribeEntities.AddAsync(new SubscribeEntity
                {
                    UserId = worker.Id,
                    User = user,
                    CreateDate = DateTime.UtcNow
                });
            }
            else
            {
                var subscribe = await _appDbContext.SubscribeEntities
                    .Where(e => e.UserId == Guid.Parse(userId))
                    .FirstOrDefaultAsync();
                if (subscribe == null)
                    throw new ItemNotFoundException("Не удалось найти подписку у пользователя");

                _appDbContext.SubscribeEntities.Remove(subscribe);
            }

            await _appDbContext.SaveChangesAsync();
        }

        public async Task<bool> IsSubscribing(Guid userId)
        {
            var worker = await _appDbContext.Workers
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync();
            
            if (worker == null)
                throw new ItemNotFoundException("Аккаунт не найден");

            return worker.IsSubscribing;
        }
}