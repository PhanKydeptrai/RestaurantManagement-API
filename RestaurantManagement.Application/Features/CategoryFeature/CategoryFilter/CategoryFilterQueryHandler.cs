﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;

namespace RestaurantManagement.Application.Features.CategoryFeature.CategoryFilter;

public class CategoryFilterQueryHandler : IRequestHandler<CategoryFilterQuery, PagedList<CategoryResponse>>
{
    private readonly IApplicationDbContext _context;

    public CategoryFilterQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<CategoryResponse>> Handle(CategoryFilterQuery request, CancellationToken cancellationToken)
    {
        var categoriesQuery = _context.Categories.AsQueryable();

        if (!string.IsNullOrEmpty(request.searchTerm))
        {
            categoriesQuery = categoriesQuery.Where(x => x.CategoryName.Contains(request.searchTerm));
        }

        // var categories = await categoriesQuery
        //     .Skip((request.page - 1) * request.pageSize)
        //     .Take(request.pageSize).Select(a => new CategoryResponse
        //     {
        //         CategoryId = a.CategoryId,
        //         CategoryName = a.CategoryName,
        //         CategoryStatus = a.CategoryStatus,
        //         Image = a.CategoryId.ToString() + ".jpg",
        //         Desciption = a.Desciption
        //     }).ToListAsync();
        
        var categories = categoriesQuery
            .Select(a => new CategoryResponse
            {
                CategoryId = a.CategoryId,
                CategoryName = a.CategoryName,
                CategoryStatus = a.CategoryStatus,
                Image = a.CategoryId.ToString() + ".jpg",
                Desciption = a.Desciption
            });
        var categoriesList = await PagedList<CategoryResponse>.CreateAsync(categories, request.page, request.pageSize);

        return categoriesList;
    }
}
