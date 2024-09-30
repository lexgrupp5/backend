using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CourseRepository(AppDbContext context, IMapper mapper)
    : RepositoryBase<Course>(context),
        ICourseRepository
{
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<CourseDto>> GetCoursesAsync() =>
        await _db
            .Courses.Include(c => c.Modules)
            .ProjectTo<CourseDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

    public async Task<IEnumerable<CourseDto?>> GetCoursesAsync(SearchFilterDTO searchFilterDTO)
    {
        var emptySearchText = searchFilterDTO.SearchText == string.Empty;

        var query = GetByConditionAsync(course =>
            course.StartDate >= searchFilterDTO.StartDate
            && course.EndDate <= searchFilterDTO.EndDate
        );

        if (!emptySearchText)
        {
            query = query.Where(course =>
                course.Name.Contains(searchFilterDTO.SearchText)
                || course.Description.Contains(searchFilterDTO.SearchText)
            );
        }

        return await query.ProjectTo<CourseDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        var result = await GetByConditionAsync(m => m.Id.Equals(id))
            .Include(m => m.Modules)
            .FirstOrDefaultAsync();
        if (result == null)
        {
            return null;
        }
        return result;
    }

    public async Task<bool> CheckCourseExistsAsync(Course course)
    {
        return await _db.Courses.AnyAsync(c => c.Name == course.Name);
    }
}
