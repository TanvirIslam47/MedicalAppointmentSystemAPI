using Core.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Shared.DTOs;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace MedicalAppointmentSystem.Infrastructure.Persistence.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> RegisterUserAsync(RegisterUserDto model)
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            var passwordHash = HashPassword(model.Password);

            var parameters = new
            {
                FullName = model.FullName,
                Mobile = model.Mobile,
                PasswordHash = passwordHash,
                Role = model.Role
            };

            var result = await con.ExecuteAsync("SP_RegisterUser", parameters, commandType: CommandType.StoredProcedure);
            return result > 0;
        }

        public async Task<RegisterUserDto?> LoginUserAsync(LoginUserDto model)
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            var passwordHash = HashPassword(model.Password);

            var parameters = new
            {
                Mobile = model.Mobile,
                PasswordHash = passwordHash
            };

            var user = await con.QueryFirstOrDefaultAsync<RegisterUserDto>(
                "SP_LoginUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return user;
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
