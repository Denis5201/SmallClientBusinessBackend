using System.Diagnostics.CodeAnalysis;
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

    public async Task<List<Appointment>> GetAppointments(Guid workerId, DateTime startDate, DateTime endDate)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        var appointments = await _context.Appointments
            .Where(e => e.WorkerId == workerId)
            .Select(e => new Appointment
            {
                Id = e.Id,
                ClientName = e.ClientName,
                Price = e.Price,
                StartDateTime = e.StartDateTime,
                EndDateTime = e.EndDateTime,
                Status = e.Status
            })
            .ToListAsync();

        appointments = SortingAppointmentsForDate(startDate, endDate, appointments);

        return appointments;
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

        var appointments = await _context.Appointments
            .Where(e => e.WorkerId == workerId)
            .Select(e => new Appointment
            {
                Id = e.Id,
                ClientName = e.ClientName,
                Price = e.Price,
                StartDateTime = e.StartDateTime,
                EndDateTime = e.EndDateTime,
                Status = e.Status
            })
            .ToListAsync();

        appointments = SortingAppointmentsForDate(startDate, endDate, appointments);
        appointments = SortingAppointmentsForPrice(startPrice, endPrice, appointments);
        if (servicesId.Any())
            appointments = await SortingAppointmentsForServices(servicesId, appointments);

        const int pageSize = 5;
        var countDishes = appointments.Count;
        var count = countDishes % pageSize < pageSize && countDishes % pageSize != 0 
            ? countDishes / 5 + 1 
            : countDishes / 5;

        if (page > count && appointments.Any())
        {
            throw new ItemNotFoundException(message: "Invalid value for attribute page");
        }

        var itemsAppointment = appointments.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
        
        var appointments = await _context.Appointments
            .Where(e => e.WorkerId == workerId)
            .Select(e => new Appointment
            {
                Id = e.Id,
                ClientName = e.ClientName,
                Price = e.Price,
                StartDateTime = e.StartDateTime,
                EndDateTime = e.EndDateTime,
                Status = e.Status
            })
            .ToListAsync();

        appointments = SortingAppointmentsForDate(startDate, endDate, appointments);
        appointments = SortingAppointmentsForPrice(startPrice, endPrice, appointments);
        if (servicesId.Any())
            appointments = await SortingAppointmentsForServices(servicesId, appointments);

        return appointments;
    }

    public async Task<Appointment> GetAppointment(Guid workerId, Guid appointmentId)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null)
            throw new ItemNotFoundException($"Не найден пользователь-работник с id = {workerId}");

        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment == null)
            throw new ItemNotFoundException($"Не найдена запись с id = {appointmentId}");

        return new Appointment
        {
            Id = appointment.Id,
            ClientName = appointment.ClientName,
            Price = appointment.Price,
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

        appointment.AppointmentServices = await _context.AppointmentService
            .Where(e => e.AppointmentId == appointment.Id)
            .ToListAsync();
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
        
        appointment = new AppointmentEntity
        {
            Id = appointment.Id,
            ClientName = model.ClientName,
            WorkerId = appointment.WorkerId,
            StartDateTime = model.StartDateTime,
            Worker = worker
        };
        
        var priceAppointment = new double();
        var endDateTime = appointment.StartDateTime;
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
        
        appointment.AppointmentServices = await _context.AppointmentService
            .Where(e => e.AppointmentId == appointment.Id)
            .ToListAsync();
        appointment.EndDateTime = endDateTime;
        appointment.Price = priceAppointment;

        _context.Appointments.Attach(appointment);
        _context.Entry(appointment).State = EntityState.Modified;

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

    private static List<Appointment> SortingAppointmentsForDate(DateTime? startDate, DateTime? endDate, List<Appointment> appointments)
    {
        if (startDate != null && endDate == null)
        {
            appointments = appointments
                .Where(appointment => appointment.StartDateTime >= startDate)
                .ToList();
        }
        else if (startDate == null && endDate != null)
        {
            appointments = appointments
                .Where(appointment => appointment.StartDateTime <= endDate)
                .ToList();
        }
        else if (startDate != null && endDate != null)
        {
            appointments = appointments
                .Where(appointment => appointment.StartDateTime <= endDate && appointment.StartDateTime >= startDate)
                .ToList();
        }

        return appointments;
    }
    
    private static List<Appointment> SortingAppointmentsForPrice(double? startPrice, double? endPrice, List<Appointment> appointments)
    {
        if (startPrice != null && endPrice == null)
        {
            appointments = appointments
                .Where(appointment => appointment.Price >= startPrice)
                .ToList();
        }
        else if (startPrice == null && endPrice != null)
        {
            appointments = appointments
                .Where(appointment => appointment.Price <= endPrice)
                .ToList();
        }
        else if (startPrice != null && endPrice != null)
        {
            appointments = appointments
                .Where(appointment => appointment.Price <= endPrice && appointment.Price >= startPrice)
                .ToList();
        }

        return appointments;
    }

    private async Task<List<Appointment>> SortingAppointmentsForServices(List<Guid>? servicesId, List<Appointment> appointments)
    {
        var sortingAppointments = new List<Appointment>();
        foreach (var appointment in appointments)
        {
            var appointmentsServices = await _context.AppointmentService
                .Where(e => e.AppointmentId == appointment.Id && servicesId.Contains(e.ServiceId))
                .ToListAsync();

            foreach (var appointmentsService in appointmentsServices)
            {
                var newAppointment = await _context.Appointments
                    .Where(e => e.Id == appointmentsService.AppointmentId)
                    .Select(e => new Appointment
                    {
                        Id = e.Id,
                        ClientName = e.ClientName,
                        Price = e.Price,
                        StartDateTime = e.StartDateTime,
                        EndDateTime = e.EndDateTime,
                        Status = e.Status
                    })
                    .FirstOrDefaultAsync();
                
                if (newAppointment != null)
                    sortingAppointments.Add(newAppointment);
            }
        }

        return sortingAppointments;
    }
}