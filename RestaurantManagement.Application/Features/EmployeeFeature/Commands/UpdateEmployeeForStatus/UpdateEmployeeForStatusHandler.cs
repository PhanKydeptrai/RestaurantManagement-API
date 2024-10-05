﻿using MediatR;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.UpdateEmployeeForStatus
{
    public class UpdateEmployeeForStatusHandler : IRequestHandler<UpdateEmployeeForStatusCommand, Result<bool>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateEmployeeForStatusHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(UpdateEmployeeForStatusCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<bool>
            {
                ResultValue = false
            };


            return result;
        }
    }
}
