using Core.Application.Interfaces;
using MediatR;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Queries
{
    public class GetAppointmentByIdQuery : IRequest<AppointmentListDto?>
    {
        public int AppointmentId { get; }

        public GetAppointmentByIdQuery(int appointmentId)
        {
            AppointmentId = appointmentId;
        }

        public class Handler : IRequestHandler<GetAppointmentByIdQuery, AppointmentListDto?>
        {
            private readonly IAppointmentRepository _repository;

            public Handler(IAppointmentRepository repository)
            {
                _repository = repository;
            }

            public async Task<AppointmentListDto?> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
            {
                return await _repository.GetAppointmentByIdAsync(request.AppointmentId);
            }
        }
    }

}
