using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Exceptions;
using Gym_Inventory_Management_System.Helpers;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gym_Inventory_Management_System.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _hasher;
    private readonly JwtTokenGenerator _tokenGenerator;
    public AuthService(AppDbContext context, PasswordHasher<User> hasher, JwtTokenGenerator tokenGenerator)
    {
        _context = context;
        _hasher = hasher;
        _tokenGenerator = tokenGenerator;
    }
    public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request)
    {
        var isUsernameTaken = await _context.Users.AnyAsync(u => u.Username == request.Username);
        if (isUsernameTaken)
        {
            throw new ConflictException("Username is already taken.");
        }

        var newUser = new User
        {
            Name = request.Name,
            Email = request.Email,
            Birthdate = request.Birthdate,
            Address = request.Address,
            ContactNumber = request.ContactNumber,
            Username = request.Username,
            PasswordHash = "",
            Role = UserRole.Staff// Default role
        };
        newUser.PasswordHash = _hasher.HashPassword(newUser, request.Password);
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        var (token, expiresAt) = _tokenGenerator.GenerateToken(newUser);
        return new AuthResponseDTO(token, expiresAt, newUser.Username, newUser.Role);
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null)
        {
            throw new InvalidLoginInput("Invalid username or password.");
        }
        var verificationResult = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidLoginInput("Invalid username or password.");
        }
        var (token, expiresAt) = _tokenGenerator.GenerateToken(user);
        return new AuthResponseDTO(token, expiresAt, user.Username, user.Role);
    }
}

