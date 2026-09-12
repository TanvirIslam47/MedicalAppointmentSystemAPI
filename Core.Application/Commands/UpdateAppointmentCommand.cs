using Core.Application.Interfaces;
using MediatR;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Commands
{
    public class UpdateAppointmentCommand : IRequest<bool>
    {
        public int AppointmentId { get; set; }
        public AppointmentDto Model { get; set; }
        public class Handler : IRequestHandler<UpdateAppointmentCommand, bool>
        {
            private readonly IAppointmentRepository _repository;

            public Handler(IAppointmentRepository repository)
            {
                _repository = repository;
            }

            public async Task<bool> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
            {
                var result = await _repository.UpdateAppointmentAsync(request.AppointmentId,request.Model);
                return result;

            }
        }
    }
}
