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
    public class GetAppointmentsQuery : IRequest<PagedResult<AppointmentListDto>>
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public string Role { get; }
        public string? ContactNumber { get; }

        public GetAppointmentsQuery(int pageNumber, int pageSize, string role, string? contactNumber)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Role = role;
            ContactNumber = contactNumber;
        }

        public class Handler : IRequestHandler<GetAppointmentsQuery, PagedResult<AppointmentListDto>>
        {
            private readonly IAppointmentRepository _repository;
            public Handler(IAppointmentRepository repository)
            {
                _repository = repository;
            }

            public async Task<PagedResult<AppointmentListDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
            {
                return await _repository.GetAppointmentsAsync(request.PageNumber, request.PageSize,request.Role,request.ContactNumber);
            }
        }
    }
}
