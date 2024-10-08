using AutoMapper;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Infrastructure.Persistence.Repositories;

public class CourseRepository : RepositoryBase<Course>, ICourseRepository
{
    public CourseRepository(AppDbContext context)
        : base(context)
    {
    }

    /* DEPRECATED
     **********************************************************************/
    /*
        public async Task<ICollection<CourseDto>?> GetCoursesAsync() =>
            await _db
                .Courses.Include(c => c.Modules)
                .ProjectTo<CourseDto>(_mapper.ConfigurationProvider)
                .ToListAsync(); */

    /*  public async Task<ICollection<CourseDto>?> GetCoursesAsync(SearchFilterDTO searchFilterDTO)
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
     } */

    /* public async Task<ICollection<Module>?> GetModulesOfCourseAsync(
        int id,
        SearchFilterDTO searchFilterDto
    )
    {
        IQueryable<Module> query = _db
            .Courses.Where(c => c.Id == id)
            .Include(c => c.Modules)
            .ThenInclude(m => m.Activities)
            .ThenInclude(a => a.ActivityType)
            .SelectMany(c => c.Modules);

        if (searchFilterDto.SearchText != null)
        {
            query = query.Where(m => m.Name.Contains(searchFilterDto.SearchText));
        }

        var modules = await query.ToListAsync();

        return modules;
    } */

    /*
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
    } */

    /* public async Task<bool> CheckCourseExistsAsync(Course course)
    {
        return await _db.Courses.AnyAsync(c => c.Name == course.Name);
    } */
}
