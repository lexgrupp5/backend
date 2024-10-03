using System.Linq.Expressions;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Models;

namespace Application.Interfaces;

public interface IActivityService : IServiceBase<Activity, ActivityDto>
{
    Task<IEnumerable<ActivityDto?>> GetActivitiesAsync(
        ICollection<Expression<Func<Activity, bool>>>? filters = null,
        ICollection<SortParams>? sorting = null,
        PageParams? paging = null
    );
    Task<ActivityDto?> GetActivityByIdAsync<TDto>(int id);

    /* DEPRECATED
     **********************************************************************/

     Task<ActivityDto> PatchActivity(ActivityDto dto);
}
