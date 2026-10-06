using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Exceptions;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Mappers;
using Gym_Inventory_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gym_Inventory_Management_System.Services;

public class UsersService : IUsersService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _hasher;
    public UsersService(AppDbContext context, PasswordHasher<User> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public async Task<List<UserResponseDTO>> GetAllUsersAsync()
    {
        var users = await _context.Users.Select(u => UserMapper.ToDto(u)).AsNoTracking().ToListAsync();
        return users;
    }

    public async Task<UserResponseDTO> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) throw new NotFoundException("User not found with the specified ID");
        return UserMapper.ToDto(user);
    }

    public async Task<UserResponseDTO> CreateUserAsync(UserCreateRequestDTO dto)
    {
        var isUsernameTaken = await _context.Users.AnyAsync(u => u.Username == dto.Username);
        if (isUsernameTaken)
        {
            throw new ConflictException("Username is already taken.");
        }

        var newUser = UserMapper.ToEntity(dto);
        newUser.PasswordHash = _hasher.HashPassword(newUser, dto.Password);
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return UserMapper.ToDto(newUser);
    }

    public async Task<UserResponseDTO> UpdateUserAsync(int id, UserUpdateRequestDTO dto)
    {
        var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (userToUpdate == null) throw new NotFoundException("User not found with the specified ID");

        userToUpdate.Name = dto.Name;
        userToUpdate.Email = dto.Email;
        userToUpdate.Birthdate = dto.Birthdate;
        userToUpdate.Address = dto.Address;
        userToUpdate.ContactNumber = dto.ContactNumber;

        await _context.SaveChangesAsync();
        return UserMapper.ToDto(userToUpdate);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var userToDelete = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (userToDelete == null) throw new NotFoundException("User not found with the specified ID");
        _context.Users.Remove(userToDelete);
        await _context.SaveChangesAsync();
        return true;
    }



    public async Task<UserResponseDTO> PromoteUserRoleAsync(int id, UserRole role)
    {
        var userToPromote = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (userToPromote == null) throw new NotFoundException("User not found with the specified ID");
        userToPromote.Role = role;
        await _context.SaveChangesAsync();
        return UserMapper.ToDto(userToPromote);
    }

    public async Task<bool> UpdateUserPasswordAsync(int id, UpdatePasswordRequestDTO dto)
    {
        var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (userToUpdate == null) throw new NotFoundException("User not found with the specified ID");
        userToUpdate.PasswordHash = _hasher.HashPassword(userToUpdate, dto.NewPassword);
        await _context.SaveChangesAsync();
        return true;
    }
}
