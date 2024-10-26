using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.EmployeeDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;
using System.Linq.Expressions;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetAllEmployee
{
    public class GetAllEmployeeQueryHandler : IQueryHandler<GetAllEmployeeQuery, PagedList<EmployeeResponse>>
    {
        private readonly IApplicationDbContext _context;
        public GetAllEmployeeQueryHandler(
            IApplicationDbContext context, IEmployeeRepository employeeRepository)
        {
            _context = context;
        }

        public async Task<Result<PagedList<EmployeeResponse>>> Handle(GetAllEmployeeQuery request, CancellationToken cancellationToken)
        {
            var employeeQuery = _context.Employees.Include(a => a.User).AsQueryable();
            if (!string.IsNullOrEmpty(request.searchTerm))
            {
                employeeQuery = employeeQuery.Where(x => x.User.FirstName.Contains(request.searchTerm) ||
                                                        x.User.LastName.Contains(request.searchTerm) ||
                                                        x.User.Email.Contains(request.searchTerm) ||
                                                        x.User.Phone.Contains(request.searchTerm));
            }

            //filter trạng thái nhân viên
            if (!string.IsNullOrEmpty(request.filterStatus))
            {
                employeeQuery = employeeQuery.Where(x => x.EmployeeStatus == request.filterStatus);
            }

            //filter theo role
            if (!string.IsNullOrEmpty(request.filterRole))
            {
                employeeQuery = employeeQuery.Where(x => x.Role == request.filterRole);
            }
            //filter theo giới tính
            if (!string.IsNullOrEmpty(request.filterGender))
            {
                employeeQuery = employeeQuery.Where(x => x.User.Gender == request.filterGender);
            }


            //sort
            Expression<Func<Employee, object>> keySelector = request.sortColumn?.ToLower() switch
            {
                "employeename" => x => x.User.FirstName, //Sắp xếp theo tên nhân viên
                "employeeid" => x => x.UserId, //Sắp xếp theo id nhân viên
                _ => x => x.UserId
            };

            if (request.sortOrder?.ToLower() == "desc")
            {
                employeeQuery = employeeQuery.OrderByDescending(keySelector);
            }
            else
            {
                employeeQuery = employeeQuery.OrderBy(keySelector);
            }

            var employees = employeeQuery.Select(a => new EmployeeResponse(
                    a.UserId,
                    a.User.FirstName,
                    a.User.LastName,
                    a.User.Email,
                    a.User.Phone,
                    a.User.Gender,
                    a.User.Status,
                    a.EmployeeStatus,
                    a.Role,
                    a.User.ImageUrl)).AsQueryable();
            var employeelist = await PagedList<EmployeeResponse>
            .CreateAsync(employees, request.page ?? 1, request.pageSize ?? 10);

            return Result<PagedList<EmployeeResponse>>.Success(employeelist);
        }
    }
}
