using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Inventory_Management_System.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("all", Name = "GetAllUsersAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserResponseDTO>>> GetAllUsersAsync()
    {
        return Ok(await _usersService.GetAllUsersAsync());
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("{id}", Name = "GetUserById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDTO>> GetUserByIdAsync(int id)
    {
        var user = await _usersService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDTO>> CreateUserAsync(UserCreateRequestDTO dto)
    {
        var user = await _usersService.CreateUserAsync(dto);
        return CreatedAtAction("GetUserById", new { id = user.Id }, user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update/{id}", Name = "UpdateUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDTO>> UpdateUserAsync(int id, UserUpdateRequestDTO dto)
    {
        var user = await _usersService.UpdateUserAsync(id, dto);
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("promote/{id}", Name = "PromoteUserRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDTO>> PromoteUserRoleAsync(int id, [FromQuery]  UserRole role)
    {
        var user = await _usersService.PromoteUserRoleAsync(id, role);
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}", Name = "DeleteUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUserAsync(int id)
    {
        var result = await _usersService.DeleteUserAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/password", Name = "UpdateUserPassword")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateUserPasswordAsync(int id, UpdatePasswordRequestDTO dto)
    {
        var result = await _usersService.UpdateUserPasswordAsync(id, dto);
        return NoContent();
    }
}