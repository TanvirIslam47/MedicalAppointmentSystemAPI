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
    public class DeleteAppointmentCommand : IRequest<bool>
    {
        public int AppointmentId { get; set; }
        public class Handler : IRequestHandler<DeleteAppointmentCommand, bool>
        {
            private readonly IAppointmentRepository _repository;

            public Handler(IAppointmentRepository repository)
            {
                _repository = repository;
            }

            public async Task<bool> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
            {
                var result = await _repository.DeleteAppointmentAsync(request.AppointmentId);
                return result;

            }
        }
    }
}
