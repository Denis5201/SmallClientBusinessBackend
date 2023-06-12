using System.Diagnostics.CodeAnalysis;

namespace SmallClientBusiness.Common.Dto;

public class SortingAppointmentForDate
{
    public DateOnly? startDate { get; set; }
    public DateOnly? endDate { get; set; }
}