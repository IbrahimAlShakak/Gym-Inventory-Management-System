using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Inventory_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesService _categoriesService;
    public CategoriesController(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryResponseDTO>>> GetAllCategoriesAsync()
    {
        return Ok(await _categoriesService.GetAllCategoriesAsync());
    }

    [HttpGet("{id}", Name = "GetCategoryById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDTO>> GetCategoryByIdAsync(int id)
    {
        var category = await _categoriesService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryResponseDTO>> CreateCategoryAsync(CategoryCreateRequestDTO dto)
    {
        var createdCategory = await _categoriesService.CreateCategoryAsync(dto);
        return CreatedAtAction("GetCategoryById", new { id = createdCategory.Id }, createdCategory);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDTO>> UpdateCategoryAsync(int id, CategoryUpdateRequestDTO dto)
    {
        var updatedCategory = await _categoriesService.UpdateCategoryAsync(id, dto);
        if (updatedCategory == null)
        {
            return NotFound();
        }
        return Ok(updatedCategory);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCategoryAsync(int id)
    {
        var deleted = await _categoriesService.DeleteCategoryAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}

