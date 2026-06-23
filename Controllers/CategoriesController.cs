using BookApi.Data;
using BookApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookApi.DTOs;
using BookApi.Services;
namespace BookApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CategoryService _categoryService;
    public CategoriesController(AppDbContext context, CategoryService categoryService)
    {
        _context = context;
        _categoryService = categoryService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        var categories = await _categoryService.GetCategoriesAsync();
        return Ok(categories);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }
    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory(CreateCategoryDto createCategoryDto)
    {
        var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }
   }
