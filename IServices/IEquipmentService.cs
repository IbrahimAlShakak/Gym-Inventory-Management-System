using Gym_Inventory_Management_System.DTOs;

namespace Gym_Inventory_Management_System.IServices;

public interface IEquipmentService
{
    public Task<List<EquipmentResponseDTO>> GetAllEquipmentAsync();
    public Task<EquipmentResponseDTO> GetEquipmentByIdAsync(int id);
    public Task<EquipmentResponseDTO> CreateEquipmentAsync(EquipmentCreateRequestDTO dto);
    public Task<EquipmentResponseDTO> UpdateEquipmentAsync(int id, EquipmentUpdateRequestDTO dto);
    public Task<bool> DeleteEquipmentAsync(int id);
}