using Core.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Shared.DTOs;

namespace Core.Application.Commands
{
    public class CreateAppointmentCommand : IRequest<(bool IsSuccess, int AppointmentId)>
    {
        public  AppointmentDto Model { get; set; } // Updated namespace to match the correct type

        public class Handler : IRequestHandler<CreateAppointmentCommand, (bool IsSuccess, int AppointmentId)>
        {
            private readonly IAppointmentRepository _repository;

            public Handler(IAppointmentRepository repository)
            {
                _repository = repository;
            }

            public async Task<(bool IsSuccess, int AppointmentId)> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
            {
                return await _repository.SaveAppointmentAsync(request.Model);
            }
        }
    }

}
