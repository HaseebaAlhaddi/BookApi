using BookApi.Data;
using BookApi.DTOs;
using BookApi.Models;
using BookApi.Repositories;
using Microsoft.EntityFrameworkCore;
namespace BookApi.Services;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<List<Category>> GetCategoriesAsync()
    {
       return await _categoryRepository.GetAllAsync();
    }
    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }
    public async Task<Category> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        var category = new Category
        {
            Name = createCategoryDto.Name
        };
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return category;
    }
    public async Task<bool> UpdateCategoryAsync(int id, Category updatedCategory)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return false;
        }
        category.Name = updatedCategory.Name;
        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return false;
        }
        _categoryRepository.Delete(category);
        await _categoryRepository.SaveChangesAsync();
        return true;
    }


}