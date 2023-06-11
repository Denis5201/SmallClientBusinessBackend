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

        var oldServices = appointment.AppointmentServices;
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
}