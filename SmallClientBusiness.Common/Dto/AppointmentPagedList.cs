using System.Diagnostics.CodeAnalysis;
using SmallClientBusiness.Common.Interfaces;

namespace SmallClientBusiness.Common.Dto;

public class AppointmentPagedList
{
    [MaybeNull]
    public List<Appointment> Appointments { get; set; }
    
    public PageInfo PageInfo { get; set; }
}