using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Inventory_Management_System.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ConsumablesController : ControllerBase
{
    private readonly IConsumablesService _consumableService;
    public ConsumablesController(IConsumablesService consumableService)
    {
        _consumableService = consumableService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ConsumableResponseDTO>>> GetAllConsumablesAsync()
    {
        return Ok(await _consumableService.GetAllConsumablesAsync());
    }

    [HttpGet("{id}", Name = "GetConsumableById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsumableResponseDTO>> GetConsumableByIdAsync(int id)
    {
        var consumable = await _consumableService.GetConsumableByIdAsync(id);
        return Ok(consumable);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsumableResponseDTO>> CreateConsumableAsync(ConsumableCreateRequestDTO dto)
    {
        var createdConsumable = await _consumableService.CreateConsumableAsync(dto);
        return CreatedAtAction("GetConsumableById", new { id = createdConsumable.Id }, createdConsumable);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsumableResponseDTO>> UpdateConsumableAsync(int id, ConsumableUpdateRequestDTO dto)
    {
        var updatedConsumable = await _consumableService.UpdateConsumableAsync(id, dto);
        return Ok(updatedConsumable);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteConsumableAsync(int id)
    {
        var deleted = await _consumableService.DeleteConsumableAsync(id);
        return NoContent();
    }


    [HttpGet("low-stock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ConsumableResponseDTO>>> GetLowStockConsumablesAsync()
    {
        return Ok(await _consumableService.GetLowStockConsumablesAsync());
    }

}
