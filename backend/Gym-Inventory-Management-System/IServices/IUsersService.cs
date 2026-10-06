using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Models;

namespace Gym_Inventory_Management_System.IServices;

public interface IUsersService
{
    public Task<List<UserResponseDTO>> GetAllUsersAsync();
    public Task<UserResponseDTO> GetUserByIdAsync(int id);
    public Task<UserResponseDTO> CreateUserAsync(UserCreateRequestDTO dto);
    public Task<UserResponseDTO> UpdateUserAsync(int id, UserUpdateRequestDTO dto);
    public Task<UserResponseDTO> PromoteUserRoleAsync(int id, UserRole role);
    public Task<bool> DeleteUserAsync(int id);
    public Task<bool> UpdateUserPasswordAsync(int id, UpdatePasswordRequestDTO dto);


}
