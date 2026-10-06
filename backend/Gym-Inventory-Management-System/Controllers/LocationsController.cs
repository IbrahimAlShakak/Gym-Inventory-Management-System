using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Inventory_Management_System.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{

    private readonly ILocationsService _LocationsService;
    public LocationsController(ILocationsService LocationsService)
    {
        _LocationsService = LocationsService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LocationResponseDTO>>> GetAllLocationsAsync()
    {
        return Ok(await _LocationsService.GetAllLocationsAsync());
    }

    [HttpGet("{id}", Name = "GetLocationById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocationResponseDTO>> GetLocationByIdAsync(int id)
    {
        var location = await _LocationsService.GetLocationByIdAsync(id);
        return Ok(location);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LocationResponseDTO>> CreateLocationAsync(LocationCreateRequestDTO dto)
    {
        var createdLocation = await _LocationsService.CreateLocationAsync(dto);
        return CreatedAtAction("GetLocationById", new { id = createdLocation.Id }, createdLocation);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocationResponseDTO>> UpdateLocationAsync(int id, LocationUpdateRequestDTO dto)
    {
        var updatedLocation = await _LocationsService.UpdateLocationAsync(id, dto);
        return Ok(updatedLocation);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteLocationAsync(int id)
    {
        var deleted = await _LocationsService.DeleteLocationAsync(id);
        return NoContent();
    }
}

