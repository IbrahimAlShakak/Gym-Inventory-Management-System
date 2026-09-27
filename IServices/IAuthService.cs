using Gym_Inventory_Management_System.DTOs;

namespace Gym_Inventory_Management_System.IServices;

public interface IAuthService
{
    Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
    Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request);
}
