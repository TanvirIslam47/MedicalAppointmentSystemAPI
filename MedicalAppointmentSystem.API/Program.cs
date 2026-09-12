using MedicalAppointmentSystem.Infrastructure.DependencyInjection;
namespace MedicalAppointmentSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
             builder.Services.AddRepositoryServices(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AppointmentApp",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            app.UseCors("AppointmentApp");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
