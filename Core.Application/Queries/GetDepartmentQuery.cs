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
    public class GetDepartmentQuery : IRequest<List<DepartmentDto>>
    {

        public class Handler : IRequestHandler<GetDepartmentQuery, List<DepartmentDto>>
        {
            private readonly IAppointmentRepository _repository;
            public Handler(IAppointmentRepository repository)
            {
                _repository = repository;
            }
            public async Task<List<DepartmentDto>> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
            {
                var result = await _repository.GetDepartmentAsync();
                return result;
            }
        }
    }
}