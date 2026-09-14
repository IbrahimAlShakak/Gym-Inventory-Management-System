using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Models;

namespace Gym_Inventory_Management_System.Mappers;

public static class LocationMapper
{
    public static LocationResponseDTO ToDto(Location entity) =>
        new LocationResponseDTO (entity.Id, entity.Name, entity.Address, entity.ContactNumber);

    public static Location ToEntity(LocationCreateRequestDTO dto) =>
        new Location { Name = dto.Name, Address = dto.Address, ContactNumber = dto.ContactNumber };
}
