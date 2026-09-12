using Core.Application.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Shared.DTOs;
using System.Data;

namespace MedicalAppointmentSystem.Infrastructure.Persistence.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly string _connectionString;

        public AppointmentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }



        public async Task<(bool IsSuccess, int AppointmentId)> SaveAppointmentAsync(AppointmentDto model)
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            await using var tran = await con.BeginTransactionAsync();
            try
            {

                var appointmentParam = new
                {
                    model.PatientId,
                    model.DoctorId,
                    model.AppointmentDate,
                    model.VisitType,
                    model.Diagnosis,
                    model.Notes
                };

                int appointmentId = await con.QuerySingleAsync<int>(
                    "SP_SaveAppointment",
                    param: appointmentParam,
                    transaction: tran,
                    commandType: CommandType.StoredProcedure
                );


                foreach (var prescription in model.Prescriptions)
                {
                    var prescriptionParam = new
                    {
                        AppointmentId = appointmentId,
                        prescription.MedicineId,
                        prescription.Dosage,
                        prescription.StartDate,
                        prescription.EndDate,
                        prescription.Notes
                    };

                    await con.ExecuteAsync(
                        "SP_SavePrescriptionDetail",
                        param: prescriptionParam,
                        transaction: tran,
                        commandType: CommandType.StoredProcedure
                    );
                }

                await tran.CommitAsync();
                return (true, appointmentId);
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                throw new Exception($"Error saving appointment: {ex.Message}");
            }
        }



        public async Task<bool> UpdateAppointmentAsync(int appointmentId, AppointmentDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.Prescriptions == null)
                model.Prescriptions = new List<PrescriptionDto>();

            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            await using var tran = await con.BeginTransactionAsync();
            try
            {

                var appointmentParam = new
                {
                    AppointmentId = appointmentId,
                    model.PatientId,
                    model.DoctorId,
                    model.AppointmentDate,
                    model.VisitType,
                    model.Diagnosis,
                    model.Notes
                };

                await con.ExecuteAsync(
                    "SP_UpdateAppointment",
                    param: appointmentParam,
                    transaction: tran,
                    commandType: CommandType.StoredProcedure
                );


                await con.ExecuteAsync(
                    "SP_DeletePrescriptionsByAppointmentId",
                    param: new { AppointmentId = appointmentId },
                    transaction: tran,
                    commandType: CommandType.StoredProcedure
                );


                foreach (var prescription in model.Prescriptions)
                {
                    var prescriptionParam = new
                    {
                        AppointmentId = appointmentId,
                        prescription.MedicineId,
                        prescription.Dosage,
                        prescription.StartDate,
                        prescription.EndDate,
                        prescription.Notes
                    };

                    await con.ExecuteAsync(
                        "SP_SavePrescriptionDetail",
                        param: prescriptionParam,
                        transaction: tran,
                        commandType: CommandType.StoredProcedure
                    );
                }

                // Commit transaction
                await tran.CommitAsync();
                return true;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            await using var tran = await con.BeginTransactionAsync();
            try
            {
                await con.ExecuteAsync(
                    "SP_DeletePrescriptionsByAppointmentId",
                    param: new { AppointmentId = appointmentId },
                    transaction: tran,
                    commandType: CommandType.StoredProcedure
                );


                int rows = await con.ExecuteAsync(
                    "SP_DeleteAppointment",
                    param: new { AppointmentId = appointmentId },
                    transaction: tran,
                    commandType: CommandType.StoredProcedure
                );

                await tran.CommitAsync();
                return rows > 0;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }


        //public async Task<PagedResult<AppointmentListDto>> GetAppointmentsAsync(int pageNumber, int pageSize)
        //{
        //    await using var con = new SqlConnection(_connectionString);
        //    await con.OpenAsync();

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@PageNumber", pageNumber);
        //    parameters.Add("@PageSize", pageSize);

        //    using var multi = await con.QueryMultipleAsync(
        //        "SP_GetAppointments",
        //        parameters,
        //        commandType: CommandType.StoredProcedure
        //    );

        //    var appointments = (await multi.ReadAsync<AppointmentListDto>()).ToList();
        //    var totalCount = await multi.ReadSingleAsync<int>();

        //    foreach (var appointment in appointments)
        //    {
        //        var prescriptions = await con.QueryAsync<PrescriptionDto>(
        //            "SP_GetPrescriptionsByAppointmentId",
        //            new { AppointmentId = appointment.AppointmentId },
        //            commandType: CommandType.StoredProcedure
        //        );
        //        appointment.Prescriptions = prescriptions.ToList();
        //    }

        //    return new PagedResult<AppointmentListDto>
        //    {
        //        Results = appointments,
        //        TotalCount = totalCount,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize
        //    };
        //}

        public async Task<PagedResult<AppointmentListDto>> GetAppointmentsAsync(int pageNumber, int pageSize, string role, string? contactNumber)
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@Role", role);
            parameters.Add("@ContactNumber", contactNumber);

            using var multi = await con.QueryMultipleAsync(
                "SP_GetAppointments",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var appointments = (await multi.ReadAsync<AppointmentListDto>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            foreach (var appointment in appointments)
            {
                var prescriptions = await con.QueryAsync<PrescriptionDto>(
                    "SP_GetPrescriptionsByAppointmentId",
                    new { AppointmentId = appointment.AppointmentId },
                    commandType: CommandType.StoredProcedure
                );
                appointment.Prescriptions = prescriptions.ToList();
            }

            return new PagedResult<AppointmentListDto>
            {
                Results = appointments,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }




        public async Task<AppointmentListDto?> GetAppointmentByIdAsync(int appointmentId)
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();


            var appointment = await con.QuerySingleOrDefaultAsync<AppointmentListDto>(
                "SP_GetAppointmentById",
                new { AppointmentId = appointmentId },
                commandType: CommandType.StoredProcedure
            );

            if (appointment == null)
                return null;


            var prescriptions = await con.QueryAsync<PrescriptionDto>(
                "SP_GetPrescriptionsByAppointmentId",
                new { AppointmentId = appointmentId },
                commandType: CommandType.StoredProcedure
            );

            appointment.Prescriptions = prescriptions.ToList();

            return appointment;
        }


        public async Task<List<PatientDto>> GetPatientsAsync()
        {
            await using var con = new SqlConnection(_connectionString);

            const string sql = @"
        SELECT 
            Patient_Id AS PatientId, 
            Full_Name AS FullName, 
            DateOfBirth, 
            Gender, 
            Contact_Number AS ContactNumber, 
            Address 
        FROM Patients";

            var result = await con.QueryAsync<PatientDto>(sql);
            return result.ToList();
        }

        public async Task<List<DoctorDto>> GetDoctorsAsync()
        {
            const string sql = @"
            SELECT 
                Doctor_Id AS DoctorId,
                Full_Name AS FullName,
                Specialization,
                Contact_Number AS ContactNumber
            FROM Doctors";

            await using var con = new SqlConnection(_connectionString);
            var result = await con.QueryAsync<DoctorDto>(sql);
            return result.ToList();
        }


        public async Task<List<MedicineDto>> GetMedicinesAsync()
        {
            const string sql = @"
            SELECT 
                Medicine_Id AS MedicineId,
                Name,
                Description
            FROM Medicines";

            await using var con = new SqlConnection(_connectionString);
            var result = await con.QueryAsync<MedicineDto>(sql);
            return result.ToList();
        }

        public async Task<int> CreateAppointmentAsync(AppointmentSaveDto model)
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            var parameters = new
            {
                model.FullName,
                model.DateOfBirth,
                model.Gender,
                model.ContactNumber,
                model.Address,
                model.PatientEmail,
                model.DepartmentId,
                model.DoctorId,
                model.AppointmentDate,
                model.Notes
            };

            return await con.QuerySingleAsync<int>(
                "SP_SaveAppointmentWithDetails",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<List<DepartmentDto>> GetDepartmentAsync()
        {
            const string sql = @"
            SELECT *
            FROM Department";

            await using var con = new SqlConnection(_connectionString);
            var result = await con.QueryAsync<DepartmentDto>(sql);
            return result.ToList();
        }
    }

}
