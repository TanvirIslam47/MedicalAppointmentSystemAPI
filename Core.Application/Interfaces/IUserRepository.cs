using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> RegisterUserAsync(RegisterUserDto model);
        Task<RegisterUserDto?> LoginUserAsync(LoginUserDto model);
    }
}
