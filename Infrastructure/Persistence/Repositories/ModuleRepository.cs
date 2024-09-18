﻿using Domain.DTOs;
using Domain.Entities;

using Infrastructure.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ModuleRepository : RepositoryBase<Module>, IModuleRepository
{
    public ModuleRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Module?>?> GetModulesOfCourseAsync(int id)
    {
        var course = await _context.Courses.Where(c => c.Id.Equals(id)).Include(c => c.Modules).FirstOrDefaultAsync();
        if (course == null) { return null; }
        return course.Modules.ToList();
    }

    public async Task<bool> CheckModuleExistsAsync(Module module)
    {
        return await _context.Modules.AnyAsync(m => m.Name == module.Name);
    }
}