using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Inventory_Management_System.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StockTransfersController : ControllerBase
{
    private readonly IStockTransfersService _stockTransferService;
    public StockTransfersController(IStockTransfersService stockTransferService)
    {
        _stockTransferService = stockTransferService;
    }

    [HttpGet("All-Stock-Transfers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<StockTransferResponseDTO>>> GetAllStockTransfers()
    {
        var stockTransfers = await _stockTransferService.GetAllStockTransfersAsync();
        return Ok(stockTransfers);
    }
    [HttpGet("Stock-Transfer/{id}", Name = "GetStockTransferById")]
    public async Task<ActionResult<StockTransferResponseDTO>> GetStockTransferById(int id)
    {
        var stockTransfer = await _stockTransferService.GetStockTransferByIdAsync(id);
        return Ok(stockTransfer);
    }

    [HttpGet("Stock-Transfers/Item/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<StockTransferResponseDTO>>> GetAllStockTransfersForItem(int itemId)
    {
        var stockTransfers = await _stockTransferService.GetAllStockTransfersForItemAsync(itemId);
        return Ok(stockTransfers);
    }

    [HttpGet("Stock-Transfers/Locations/{FromLocationId}/{ToLocationId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<StockTransferResponseDTO>>> GetAllStockTransfersForLocations(int FromLocationId, int ToLocationId)
    {
        var stockTransfers = await _stockTransferService.GetAllStockTransfersForLocationsAsync(FromLocationId, ToLocationId);
        return Ok(stockTransfers);
    }

    [HttpPost("Stock-Transfer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StockTransferResponseDTO>> CreateStockTransfer([FromBody] StockTransferCreateDTO stockTransferRequestDTO)
    {
        var stockTransfer = await _stockTransferService.CreateStockTransferAsync(stockTransferRequestDTO);
        return CreatedAtRoute("GetStockTransferById", new { id = stockTransfer.Id }, stockTransfer);
    }
}