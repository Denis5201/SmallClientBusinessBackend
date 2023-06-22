using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Enum;
using SmallClientBusiness.Common.Exceptions;
using SmallClientBusiness.Common.Interfaces;
using SmallClientBusiness.DAL;
using SmallClientBusiness.DAL.Entities;

namespace SmallClientBusiness.BL.Services;

public class AppointmentService: IAppointmentService
{
    private readonly AppDbContext _context;

    public AppointmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Appointment>> GetAppointments(Guid workerId, DateTime? startDate, DateTime? endDate)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        var appointments = _context.Appointments
            .Where(e => e.WorkerId == workerId);

        appointments = SortingAppointmentsForDate(startDate, endDate, appointments);

        appointments = appointments.Include(a => a.AppointmentServices)
            .ThenInclude(s => s.Service);

        var appointmentsList = await appointments
            .Select(e => new Appointment
            {
                Id = e.Id,
                ClientName = e.ClientName,
                ClientPhone = e.ClientPhone,
                Price = e.Price,
                Services = e.AppointmentServices
                    .Select(s => new ServiceShort { Id = s.ServiceId, Name = s.Service.Name }).ToList(),
                StartDateTime = e.StartDateTime,
                EndDateTime = e.EndDateTime,
                Status = e.Status
            })
            .ToListAsync();

        return appointmentsList;
    }

    public async Task<AppointmentPagedList> GetAppointments(
        Guid workerId, 
        double? startPrice,
        double? endPrice, 
        DateTime? startDate, 
        DateTime? endDate, 
        List<Guid> servicesId,
        int page
        )
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");
        
        if (page < 1)
        {
            throw new IncorrectDataException(message: "Page value must be greater than 0");
        }

        var appointments = _context.Appointments
            .Where(e => e.WorkerId == workerId);

        appointments = SortingAppointmentsForDate(startDate, endDate, appointments);
        appointments = SortingAppointmentsForPrice(startPrice, endPrice, appointments);

        appointments = appointments.Include(a => a.AppointmentServices)
            .ThenInclude(s => s.Service);

        if (servicesId.Any())
        {
            appointments = appointments.Where(a => a.AppointmentServices.All(s => servicesId.Contains(s.ServiceId)));
        }

        const int pageSize = 5;
        var countDishes = appointments.Count();
        var count = countDishes % pageSize < pageSize && countDishes % pageSize != 0 
            ? countDishes / 5 + 1 
            : countDishes / 5;

        if (page > count && appointments.Any())
        {
            throw new ItemNotFoundException(message: "Invalid value for attribute page");
        }

        var itemsAppointment = await appointments.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(e => new Appointment
            {
                Id = e.Id,
                ClientName = e.ClientName,
                ClientPhone = e.ClientPhone,
                Price = e.Price,
                Services = e.AppointmentServices
                    .Select(s => new ServiceShort { Id = s.ServiceId, Name = s.Service.Name } ).ToList(),
                StartDateTime = e.StartDateTime,
                EndDateTime = e.EndDateTime,
                Status = e.Status
            })
            .ToListAsync();

        return new AppointmentPagedList
        {
            Appointments = itemsAppointment,
            PageInfo = new PageInfo
            {
                Size = pageSize,
                Count = count,
                Current = page
            }
        };
    }

    public async Task<List<Appointment>> GetAppointments(Guid workerId, double? startPrice, double? endPrice, DateTime? startDate, DateTime? endDate,
        List<Guid> servicesId)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");
        
        var appointments = _context.Appointments
            .Where(e => e.WorkerId == workerId);

        appointments = SortingAppointmentsForDate(startDate, endDate, appointments);
        appointments = SortingAppointmentsForPrice(startPrice, endPrice, appointments);

        appointments = appointments.Include(a => a.AppointmentServices)
            .ThenInclude(s => s.Service);
        if (servicesId.Any())
        {
            appointments = appointments.Where(a => a.AppointmentServices
                .All(s => servicesId.Contains(s.ServiceId)));
        }

        return await appointments.Select(e => new Appointment
            {
                Id = e.Id,
                ClientName = e.ClientName,
                ClientPhone = e.ClientPhone,
                Price = e.Price,
                Services = e.AppointmentServices
                    .Select(s => new ServiceShort { Id = s.ServiceId, Name = s.Service.Name }).ToList(),
                StartDateTime = e.StartDateTime,
                EndDateTime = e.EndDateTime,
                Status = e.Status
            }).ToListAsync();
    }

    public async Task<Appointment> GetAppointment(Guid workerId, Guid appointmentId)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        var appointment = await _context.Appointments.Where(a => a.Id == appointmentId)
            .Include(a => a.AppointmentServices)
            .ThenInclude(s => s.Service)
            .FirstOrDefaultAsync();
        if (appointment == null)
            throw new ItemNotFoundException($"Не найдена запись с id = {appointmentId}");

        return new Appointment
        {
            Id = appointment.Id,
            ClientName = appointment.ClientName,
            ClientPhone = appointment.ClientPhone,
            Price = appointment.Price,
            Services = appointment.AppointmentServices
                .Select(s => new ServiceShort { Id = s.ServiceId, Name = s.Service.Name }).ToList(),
            StartDateTime = appointment.StartDateTime,
            EndDateTime = appointment.EndDateTime,
            Status = appointment.Status
        };
    }

    public async Task CreateAppointment(Guid workerId, CreateAppointment model)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        model.StartDateTime = model.StartDateTime.ToUniversalTime();

        var appointment = new AppointmentEntity
        {
            Id = Guid.NewGuid(),
            ClientName = model.ClientName,
            ClientPhone = model.ClientPhone,
            WorkerId = workerId,
            StartDateTime = model.StartDateTime,
            Status = StatusAppointment.New,
            Worker = worker
        };
        
        var priceAppointment = new double();
        var endDateTime = appointment.StartDateTime;
        foreach (var serviceId in model.IdServices)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
            {
                throw new ItemNotFoundException($"Сервис с id = {serviceId} не найден");
            }

            if (!worker.IsSubscribing && service.WorkerId == worker.Id)
                throw new NoPermissionException($"Для добавления кастомной услуги с name = {service.Name} к записи необходима подписка. Проверьте наличие подписки и попробуйте снова");

            priceAppointment += service.Price;
            endDateTime += service.Duration.ToTimeSpan();
            
            await _context.AppointmentService.AddAsync(new AppointmentServiceEntity
            {
                AppointmentId = appointment.Id,
                ServiceId = serviceId,
                Appointment = appointment,
                Service = service
            });
        }

        await CheckSameTimeAppointment(workerId,null, model.StartDateTime, endDateTime);

        appointment.EndDateTime = endDateTime;
        appointment.Price = priceAppointment;

        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task EditAppointment(Guid workerId, Guid appointmentId, EditAppointment model)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment == null)
            throw new ItemNotFoundException($"Не найдена запись с id = {appointmentId}");
        
        if (appointment.WorkerId != workerId)
            throw new NoPermissionException($"У вас нет доступа для изменения данной записи с id = {appointment.Id}");

        model.StartDateTime = model.StartDateTime.ToUniversalTime();
        
        appointment.ClientName = model.ClientName;
        appointment.StartDateTime = model.StartDateTime;
        appointment.ClientPhone = model.ClientPhone;

        var priceAppointment = new double();
        var endDateTime = appointment.StartDateTime;

        var currentServices = await _context.AppointmentService
            .Where(e => e.AppointmentId == appointmentId)
            .ToListAsync();

        foreach (var currentService in currentServices)
        {
            _context.AppointmentService.Remove(currentService);
        }
        
        await _context.SaveChangesAsync();
        
        foreach (var serviceId in model.ServicesId)
        {
            var appointmentService = await _context.AppointmentService
                .Where(e => e.AppointmentId == appointment.Id && e.ServiceId == serviceId)
                .FirstOrDefaultAsync();
            if (appointmentService != null)
                continue;
            
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
            {
                throw new ItemNotFoundException($"Сервис с id = {serviceId} не найден");
            }
            
            if (!worker.IsSubscribing && service.WorkerId == worker.Id)
                throw new NoPermissionException($"Для добавления кастомной услуги с name = {service.Name} к записи необходима подписка. Проверьте наличие подписки и попробуйте снова");

            priceAppointment += service.Price;
            endDateTime += service.Duration.ToTimeSpan();
            
            await _context.AppointmentService.AddAsync(new AppointmentServiceEntity
            {
                AppointmentId = appointment.Id,
                ServiceId = serviceId,
                Appointment = appointment,
                Service = service
            });
        }
        
        await CheckSameTimeAppointment(workerId, appointmentId, model.StartDateTime, endDateTime);
        
        appointment.EndDateTime = endDateTime;
        appointment.Price = priceAppointment;

        _context.Appointments.Attach(appointment);
        _context.Entry(appointment).State = EntityState.Modified;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAppointment(Guid workerId, Guid appointmentId)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment == null)
            throw new ItemNotFoundException($"Не найдена запись с id = {appointmentId}");

        if (appointment.WorkerId != workerId)
            throw new NoPermissionException($"У вас нет доступа для удаления данной записи с id = {appointment.Id}");

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllAppointments(Guid workerId)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        var appointments = await _context.Appointments
            .Where(a => a.WorkerId == workerId)
            .ToListAsync();

        foreach (var appointment in appointments)
        {
            _context.Appointments.Remove(appointment);
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task ChangeStatus(Guid workerId, Guid appointmentId, StatusAppointment status)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment == null)
            throw new ItemNotFoundException($"Не найдена запись с id = {appointmentId}");

        appointment.Status = status;
        
        _context.Appointments.Attach(appointment);
        _context.Entry(appointment).State = EntityState.Modified;

        await _context.SaveChangesAsync();
    }

    private static IQueryable<AppointmentEntity> SortingAppointmentsForDate(DateTime? startDate, DateTime? endDate, IQueryable<AppointmentEntity> appointments)
    {
        startDate = startDate?.ToUniversalTime();
        endDate = endDate?.ToUniversalTime();
        
        if (startDate != null )
        {
            appointments = appointments
                .Where(appointment => appointment.StartDateTime >= startDate);
        }
        if (endDate != null)
        {
            appointments = appointments
                .Where(appointment => appointment.StartDateTime <= endDate);
        }

        appointments = appointments.OrderBy(s => s.StartDateTime);

        return appointments;
    }
    
    private static IQueryable<AppointmentEntity> SortingAppointmentsForPrice(double? startPrice, double? endPrice, IQueryable<AppointmentEntity> appointments)
    {
        if (startPrice != null)
        {
            appointments = appointments
                .Where(appointment => appointment.Price >= startPrice);
        }
        if (endPrice != null)
        {
            appointments = appointments
                .Where(appointment => appointment.Price <= endPrice);
        }

        return appointments;
    }

    private async Task CheckSameTimeAppointment(Guid workerId, Guid? appointmentId, DateTime newAppointmentStartDateTime, DateTime newAppointmentEndDateTime)
    {
        var appointments = await _context.Appointments
            .Where(a => a.WorkerId == workerId && 
                        a.Id != appointmentId && 
                        newAppointmentStartDateTime < a.EndDateTime && 
                        newAppointmentEndDateTime > a.StartDateTime)
            .ToListAsync();

        if (appointments.Any())
        {
            throw new IncorrectDataException($"Возникли временные конфликты. Во время новой записи у вас уже есть запись с клиентом {appointments.First().ClientName} в {appointments.First().StartDateTime} до {appointments.First().EndDateTime}");
        }
    }
}