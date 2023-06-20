using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Exceptions;
using SmallClientBusiness.Common.Interfaces;
using SmallClientBusiness.DAL;
using SmallClientBusiness.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.BL.Services
{
    internal class ProfileService : IProfileService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly AppDbContext _appDbContext;

        public ProfileService(UserManager<UserEntity> userManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
        }

        public async Task<WorkerProfile> GetWorkerProfile(string userId)
        {
            var worker = await _appDbContext.Workers
                .Where(x => x.Id == Guid.Parse(userId))
                .Include(u => u.User).FirstOrDefaultAsync();
            if (worker == null)
            {
                throw new ItemNotFoundException("Аккаунт не найден");
            }

            return new WorkerProfile
            {
                Id = worker.Id,
                Email = worker.User.Email,
                FullName = worker.User.UserName,
                Avatar = worker.User.Avatar,
                PhoneNumber = worker.User.PhoneNumber,
                IsSubscribing = worker.IsSubscribing
            };
        }

        public async Task ChangeProfile(string userId, ChangeUser changeUser)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ItemNotFoundException("Аккаунт не найден");
            }

            user.UserName = changeUser.FullName;
            user.PhoneNumber = changeUser.PhoneNumber;

            await _appDbContext.SaveChangesAsync();
        }

        public async Task ChangePassword(string userId, ChangePassword changePassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ItemNotFoundException("Аккаунт не найден");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
            if (!result.Succeeded)
            {
                throw new IncorrectDataException(result.Errors.First().Description);
            }
        }

        public async Task UploadAvatar(Guid userId, AvatarUpload avatarUpload, string path)
        {
            var user = await _appDbContext.Users
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync();
            
            if (user == null)
                throw new ItemNotFoundException("Аккаунт не найден");

            if (avatarUpload.avatar.Length == 0)
                throw new FailedLoadAvatarException("Не удалось загрузить новую фотографию на аватар профиля");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            if (!avatarUpload.avatar.FileName.Contains(".jpeg"))
                throw new IncorrectDataException("Необходимо прикрепить фотографию расширения jpeg");
            
            await using (var fileStream = File.Create(path + user.Id + ".jpeg"))
            {
                await avatarUpload.avatar.CopyToAsync(fileStream);
                fileStream.Flush();
                
                user.Avatar = true;
                
                _appDbContext.Users.Attach(user);
                _appDbContext.Entry(user).State = EntityState.Modified;

                await _appDbContext.SaveChangesAsync();
            }
        }

        public async Task<byte[]> LoadAvatar(Guid userId, string path)
        {
            var user = await _appDbContext.Users
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync();
            
            if (user == null)
                throw new ItemNotFoundException("Аккаунт не найден");

            if (user.Avatar == false)
                throw new ItemNotFoundException("У пользователя еще нет аватара");
            
            var filePath = path + user.Id + ".jpeg";
            
            if (!File.Exists(filePath))
            {
                throw new ItemNotFoundException("Не удалось найти аватар профиля");
            }

            return await File.ReadAllBytesAsync(filePath);
        }

        public async Task DeleteAvatar(Guid userId, string path)
        {
            var user = await _appDbContext.Users
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync();
            
            if (user == null)
                throw new ItemNotFoundException("Аккаунт не найден");
            
            if (user.Avatar == false)
                throw new ItemNotFoundException("У пользователя еще нет аватара");
            
            File.Delete(path + user.Id + ".png");
            
            user.Avatar = false;
                
            _appDbContext.Users.Attach(user);
            _appDbContext.Entry(user).State = EntityState.Modified;

            await _appDbContext.SaveChangesAsync();
        }
    }
}
