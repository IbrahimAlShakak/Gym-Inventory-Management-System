using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Gym_Inventory_Management_System.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;
    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EquipmentResponseDTO>>> GetAllEquipmentAsync()
    {
        return Ok(await _equipmentService.GetAllEquipmentAsync());
    }

    [HttpGet("{id}", Name = "GetEquipmentById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipmentResponseDTO>> GetEquipmentByIdAsync(int id)
    {
        var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
        return Ok(equipment);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipmentResponseDTO>> CreateEquipmentAsync(EquipmentCreateRequestDTO dto)
    {
        var createdEquipment = await _equipmentService.CreateEquipmentAsync(dto);
        return CreatedAtAction("GetEquipmentById", new { id = createdEquipment.Id }, createdEquipment);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipmentResponseDTO>> UpdateEquipmentAsync(int id, EquipmentUpdateRequestDTO dto)
    {
        var updatedEquipment = await _equipmentService.UpdateEquipmentAsync(id, dto);
        return Ok(updatedEquipment);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteEquipmentAsync(int id)
    {
        var deleted = await _equipmentService.DeleteEquipmentAsync(id);
        return NoContent();
    }
}

