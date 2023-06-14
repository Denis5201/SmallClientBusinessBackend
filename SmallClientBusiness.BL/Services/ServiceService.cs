using Microsoft.EntityFrameworkCore;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Exceptions;
using SmallClientBusiness.Common.Interfaces;
using SmallClientBusiness.DAL;
using SmallClientBusiness.DAL.Entities;

namespace SmallClientBusiness.BL.Services;

public class ServiceService: IServiceService
{
    private readonly AppDbContext _context;

    public ServiceService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Service>> GetServices(Guid workerId)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        var services = await _context.Services
            .Where(e => e.WorkerId == null || (e.WorkerId == workerId && worker.IsSubscribing))
            .Select(e => new Service
            {
                Id = e.Id,
                Name = e.Name,
                Price = e.Price,
                Duration = e.Duration
            })
            .ToListAsync();

        return services;
    }

    public async Task<List<Service>> GetDefaultServices()
    {
       return await _context.Services
            .Where(e => e.WorkerId == null)
            .Select(e => new Service
            {
                Id = e.Id,
                Name = e.Name,
                Price = e.Price,
                Duration = e.Duration
            })
            .ToListAsync();
    }

    public async Task<List<Service>> GetCustomServices(Guid workerId)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        if (!worker.IsSubscribing)
            throw new NoPermissionException("У вас нет доступа для просмотра кастомных услуг. Проверьте наличие подписки");
        
        return await _context.Services
            .Where(e => e.WorkerId == workerId)
            .Select(e => new Service
            {
                Id = e.Id,
                Name = e.Name,
                Price = e.Price,
                Duration = e.Duration
            })
            .ToListAsync();
    }

    public async Task<Service> GetService(Guid workerId, Guid serviceId)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        var service = await _context.Services
            .Where(e => e.Id == serviceId)
            .FirstOrDefaultAsync();
        if (service == null)
            throw new ItemNotFoundException($"Не найдена услуга с id = {serviceId}");
        
        if (!worker.IsSubscribing && service.WorkerId == workerId)
            throw new NoPermissionException("У вас нет доступа для просмотра кастомной услуги. Проверьте наличие подписки");

        if (service.WorkerId != workerId || service.WorkerId != null)
            throw new NoPermissionException($"У вас нет доступа для просмотра услуги с id = {serviceId}");
        
        return new Service
        {
            Id = service.Id,
            Name = service.Name,
            Price = service.Price,
            Duration = service.Duration
        };
    }

    public async Task CreateService(Guid workerId, CreateService model)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        if (!worker.IsSubscribing)
            throw new NoPermissionException("Вы не можете создавать услугу, так как у вас отсутствует подписка");

        var service = new ServiceEntity
        {
            WorkerId = workerId,
            Name = model.Name,
            Price = model.Price,
            Duration = model.Duration,
            Worker = worker
        };
        
        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();
    }

    public async Task EditService(Guid workerId, Guid serviceId, EditService model)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");
        
        if (!worker.IsSubscribing)
            throw new NoPermissionException("Вы не можете изменять эту услугу, так как у вас отсутствует подписка");

        var service = await _context.Services.FindAsync(serviceId);
        if (service == null)
            throw new ItemNotFoundException($"Не найдена услуга с id = {serviceId}");
        
        if (service.WorkerId != workerId)
            throw new NoPermissionException($"У вас нет доступа для изменения услуги с id = {serviceId}");

        service = new ServiceEntity
        {
            Id = service.Id,
            WorkerId = service.WorkerId,
            Name = model.Name,
            Price = model.Price,
            Duration = model.Duration,
            Worker = worker
        };
        
        _context.Services.Attach(service);
        _context.Entry(service).State = EntityState.Modified;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteService(Guid workerId, Guid serviceId)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");
        
        if (!worker.IsSubscribing)
            throw new NoPermissionException("Вы не можете удалить эту услугу, так как у вас отсутствует подписка");
        
        var service = await _context.Services.FindAsync(serviceId);
        if (service == null)
            throw new ItemNotFoundException($"Не найдена услуга с id = {serviceId}");
        
        if (service.WorkerId != workerId)
            throw new NoPermissionException($"У вас нет доступа к удалению услуги с id = {serviceId}");

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
    }
}