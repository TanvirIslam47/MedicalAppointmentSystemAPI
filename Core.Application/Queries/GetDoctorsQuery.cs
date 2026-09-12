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
    public class GetDoctorsQuery : IRequest<List<DoctorDto>>
    {

        public class Handler : IRequestHandler<GetDoctorsQuery, List<DoctorDto>>
        {
            private readonly IAppointmentRepository _repository;
            public Handler(IAppointmentRepository repository)
            {
                _repository = repository;
            }
            public async Task<List<DoctorDto>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
            {
                var result = await _repository.GetDoctorsAsync();
                return result;
            }
        }
    }
}
