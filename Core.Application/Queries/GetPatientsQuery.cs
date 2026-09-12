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
    public class GetPatientsQuery : IRequest<List<PatientDto>>
    {

        public class Handler : IRequestHandler<GetPatientsQuery, List<PatientDto>>
        {
            private readonly IAppointmentRepository _repository;
            public Handler(IAppointmentRepository repository)
            {
                _repository = repository;
            }
            public async Task<List<PatientDto>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
            {
                var result = await _repository.GetPatientsAsync();
                return result;
            }
        }
    }
}