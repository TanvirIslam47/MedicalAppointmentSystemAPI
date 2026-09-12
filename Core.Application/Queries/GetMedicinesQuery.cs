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
    public class GetMedicinesQuery : IRequest<List<MedicineDto>>
    {

        public class Handler : IRequestHandler<GetMedicinesQuery, List<MedicineDto>>
        {
            private readonly IAppointmentRepository _repository;
            public Handler(IAppointmentRepository repository)
            {
                _repository = repository;
            }
            public async Task<List<MedicineDto>> Handle(GetMedicinesQuery request, CancellationToken cancellationToken)
            {
                var result = await _repository.GetMedicinesAsync();
                return result;
            }
        }
    }
}