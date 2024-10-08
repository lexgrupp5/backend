using Application.Interfaces;
using Application.Models;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService : ServiceBase<User, UserDto>, IUserService
{
    public UserService(IDataCoordinator dataCoordinator, IMapper mapper)
        : base(dataCoordinator, mapper) { }

    public async Task<IEnumerable<UserDto>?> GetUsersAsync()
    {
        var users = await _data.Users.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> UpdateAsync(UserUpdateDto dto)
    {
        var currentUser = await _data.Users.FindAsync(dto.Id);
        if (currentUser == null)
            NotFound();

        _mapper.Map(dto, currentUser);
        await _data.CompleteAsync();
        return _mapper.Map<UserDto>(currentUser);
    }
    public async Task<T?> FindUserAsync<T>(string username) =>
        await _mapper
            .ProjectTo<T>(_data.Users.GetQuery([u => u.UserName == username]))
            .FirstOrDefaultAsync();

    /* PRIVATE HELPERS
     **********************************************************************/

    private async Task<User?> FindUserAsync(string username) => await FindUserAsync<User>(username);


    /* DEPRECATED
     **********************************************************************/

    public async Task<UserDto?> PatchUser(
        string username,
        JsonPatchDocument<UserUpdateDto> patchDocument
    )
    {
        var currentUser = await _data
            .Users.GetQuery([u => u.UserName == username])
            .FirstOrDefaultAsync();
        if (currentUser == null)
        {
            return null;
        }

        var userToPatch = _mapper.Map<UserUpdateDto>(currentUser);
        patchDocument.ApplyTo(userToPatch);

        currentUser.Name = userToPatch.Name;
        currentUser.Email = userToPatch.Email;
        currentUser.UserName = userToPatch.Username;
        await _data.CompleteAsync();

        var updatedUser = _mapper.Map<UserDto>(currentUser);
        return updatedUser;
    }

    public async Task<UserDto?> CreateNewUserAsync(
        UserCreateDto newUser,
        UserManager<User> userManager,
        IIdentityService identityService
    )
    {
        var user = _mapper.Map<User>(newUser);
        var result = await userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join("\n", result.Errors));
        }

        var createdUser = await _data
            .Users.GetByConditionAsync(u => u.Name == newUser.Name)
            .FirstAsync();
        var finalUser = await _data.Users.CreateNewUserAsync(createdUser);
        var finalDto = _mapper.Map<UserDto>(finalUser);
        return finalDto;
    }
}
