using Application.Interfaces;
using Application.Models;
using AutoMapper;

using Domain.Configuration;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    ITokenService tokenService,
    AuthTokenOptions tokenOptions,
    IDataCoordinator dataCoordinator,
    IMapper mapper
) : IUserService
{
    private readonly IDataCoordinator _dataCoordinator = dataCoordinator;
    private readonly IMapper _mapper = mapper;
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly AuthTokenOptions _tokenOptions = tokenOptions;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<IEnumerable<UserDto?>> GetUsersOfCourseByIdAsync(int courseId)
    {
        var users = await _dataCoordinator.Users.GetUsersFromCourseByIdAsync(courseId);
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

        return userDtos;
    }

    public async Task<User?> GetUserByUsername(string name)
    {
        return await _dataCoordinator.Users.GetUserByUsername(name);
    }

    public async Task<UserDto> PatchUser(
        User user,
        JsonPatchDocument<UserForUpdateDto> patchDocument
    )
    {
        var userToPatch = _mapper.Map<UserForUpdateDto>(user);
        patchDocument.ApplyTo(userToPatch);

        user.Name = userToPatch.Name;
        user.Email = userToPatch.Email;
        user.UserName = userToPatch.Username;
        user.Course = userToPatch.Course;
        await _dataCoordinator.CompleteAsync();

        var updatedUser = _mapper.Map<UserDto>(user);
        return updatedUser;
    }

    public async Task<UserDto?> CreateNewUserAsync(
        UserForCreationDto newUser,
        UserManager<User> userManager,
        IIdentityService identityService
    )
    {
        //var userCreateModel = _mapper.Map<UserCreateModel>(newUser);
        UserCreateModel userCreateModel = new UserCreateModel(
            newUser.Name,
            newUser.Username,
            newUser.Email,
            "Qwerty1234"
        );
        
        var user = _mapper.Map<User>(userCreateModel);
        var result = await userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join("\n", result.Errors));
        }
        var createdUser = await _dataCoordinator
            .Users.GetByConditionAsync(u => u.Name == newUser.Name)
            .FirstAsync();
        var finalUser = await _dataCoordinator.Users.CreateNewUserAsync(createdUser);
        var finalDto = _mapper.Map<UserDto>(finalUser);
        return finalDto;
    }
}
