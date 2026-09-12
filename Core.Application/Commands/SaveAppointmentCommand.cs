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
    public class SaveAppointmentCommand : IRequest<int>
    {
        public AppointmentSaveDto Model { get; set; }

        public class Handler : IRequestHandler<SaveAppointmentCommand, int>
        {
            private readonly IAppointmentRepository _repo;
            public Handler(IAppointmentRepository repo)
            {
                _repo = repo;
            }

            public async Task<int> Handle(SaveAppointmentCommand request, CancellationToken cancellationToken)
            {
                return await _repo.CreateAppointmentAsync(request.Model);
            }
        }
    }
}