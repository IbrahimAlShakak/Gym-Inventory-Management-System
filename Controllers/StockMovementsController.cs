using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Inventory_Management_System.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StockMovementsController : ControllerBase
{
    private readonly IStockMovementsService _stockMovementService;
    public StockMovementsController(IStockMovementsService stockMovementService)
    {
        _stockMovementService = stockMovementService;
    }
    [HttpGet("All-Stock-Movements", Name = "GetAllStockMovementsAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StockMovementResponseDTO>>> GetAllStockMovementsAsync()
    {
        return Ok(await _stockMovementService.GetAllStockMovementsAsync());
    }
    [HttpGet("Stock-Movement/{id}", Name = "GetStockMovementById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StockMovementResponseDTO>> GetStockMovementByIdAsync(int id)
    {
        var stockMovement = await _stockMovementService.GetStockMovementByIdAsync(id);
        return Ok(stockMovement);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StockMovementResponseDTO>> CreateStockMovementAsync(StockMovementCreateDTO dto)
    {
        var stockMovement = await _stockMovementService.CreateStockMovementAsync(dto);
        return CreatedAtAction("GetStockMovementById", new { id = stockMovement.Id }, stockMovement);
    }

    [HttpGet("~/api/Stock-Levels/Item/{itemId}", Name = "GetStockLevelsByItemId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<StockLevelResponseDTO>>> GetItemStockLevelsByItemIdAsync(int itemId)
    {
        var stockLevels = await _stockMovementService.GetItemStockLevelsByItemIdAsync(itemId);
        return Ok(stockLevels);
    }

    [HttpGet("~/api/Stock-Levels/Location/{locationId}", Name = "GetStockLevelsByLocationId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<StockLevelResponseDTO>>> GetStockLevelsByLocationIdAsync(int locationId)
    {
        var stockLevels = await _stockMovementService.GetStockLevelsByLocationIdAsync(locationId);
        return Ok(stockLevels);
    }

    [HttpGet("~/api/Stock-Levels/Item/{itemId}/Location/{locationId}", Name = "GetItemStockLevels")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<StockLevelResponseDTO>>> GetItemStockLevelsAsync(int itemId, int locationId)
    {
        var stockLevels = await _stockMovementService.GetItemStockLevelsAsync(itemId, locationId);
        return Ok(stockLevels);
    }



}
