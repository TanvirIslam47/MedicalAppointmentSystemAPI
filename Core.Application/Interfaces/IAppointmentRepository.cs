using Shared.DTOs;

namespace Core.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<(bool IsSuccess, int AppointmentId)> SaveAppointmentAsync(AppointmentDto model);
        Task<bool> UpdateAppointmentAsync(int appointmentId,AppointmentDto model);
        Task<bool> DeleteAppointmentAsync(int appointmentId);
        Task<PagedResult<AppointmentListDto>> GetAppointmentsAsync(int pageNumber, int pageSize, string role, string? contactNumber);

        Task<List<PatientDto>> GetPatientsAsync();
        Task<AppointmentListDto?> GetAppointmentByIdAsync(int appointmentId);
        Task<List<DoctorDto>> GetDoctorsAsync();
        Task<List<MedicineDto>> GetMedicinesAsync();
        Task<List<DepartmentDto>> GetDepartmentAsync();
        Task<int> CreateAppointmentAsync(AppointmentSaveDto model);
    }
}
