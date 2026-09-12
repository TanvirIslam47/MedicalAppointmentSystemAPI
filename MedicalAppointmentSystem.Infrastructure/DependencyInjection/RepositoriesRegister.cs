using Core.Application.Commands;
using Core.Application.Interfaces;
using Core.Application.Services;
using MedicalAppointmentSystem.Infrastructure.Persistence.Context;
using MedicalAppointmentSystem.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointmentSystem.Infrastructure.DependencyInjection
{
    public static class RepositoriesRegister
    {
        public static void AddRepositoryServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblyContaining<CreateAppointmentCommand>());

        }
    }
}
