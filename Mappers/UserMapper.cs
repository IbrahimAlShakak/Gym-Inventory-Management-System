using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Models;

namespace Gym_Inventory_Management_System.Mappers;

public static class UserMapper
{
    public static UserResponseDTO ToDto(User entity) =>
        new UserResponseDTO(entity.Id, entity.Name, entity.Email, entity.Birthdate,
            entity.Address, entity.ContactNumber, entity.Username, entity.Role);

    public static User ToEntity(UserCreateRequestDTO dto) =>
        new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Birthdate = dto.Birthdate,
            Address = dto.Address,
            ContactNumber = dto.ContactNumber,
            Username = dto.Username,
            PasswordHash = dto.Password,// Password is not hashed yet but it will be done in the service layer
            Role = UserRole.Staff // Default role for new users
        };

}
