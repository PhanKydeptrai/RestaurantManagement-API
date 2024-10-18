﻿using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.UpdateEmployeeForStatus;

public class UpdateEmployeeForStatusHandler : ICommand<UpdateEmployeeForStatusCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateEmployeeForStatusHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<Result> Handle(UpdateEmployeeForStatusCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
