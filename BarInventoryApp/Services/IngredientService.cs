using BarInventoryApp.DataContexts;
using BarInventoryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BarInventoryApp.Services;

/// <summary>
/// Сервис для работы с ингредиентами.
/// </summary>
public class IngredientService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр класса IngredientService.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    public IngredientService(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Получает все ингредиенты из базы данных.
    /// </summary>
    /// <returns>Список всех ингредиентов.</returns>
    public async Task<List<Ingredient>> GetAllAsync()
    {
        return await _context.Ingredients.ToListAsync();
    }

    /// <summary>
    /// Добавляет новый ингредиент в базу данных.
    /// </summary>
    /// <param name="ingredient">Ингредиент для добавления.</param>
    /// <exception cref="ArgumentNullException">Если ingredient равен null.</exception>
    public async Task AddAsync(Ingredient ingredient)
    {
        if (ingredient == null)
            throw new ArgumentNullException(nameof(ingredient));

        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Обновляет существующий ингредиент в базе данных.
    /// </summary>
    /// <param name="ingredient">Ингредиент для обновления.</param>
    /// <exception cref="ArgumentNullException">Если ingredient равен null.</exception>
    public async Task UpdateAsync(Ingredient ingredient)
    {
        if (ingredient == null)
            throw new ArgumentNullException(nameof(ingredient));

        _context.Ingredients.Update(ingredient);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Удаляет ингредиент по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор ингредиента для удаления.</param>
    /// <returns>True, если ингредиент был удален; иначе false.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);
        if (ingredient == null)
            return false;

        _context.Ingredients.Remove(ingredient);
        await _context.SaveChangesAsync();
        return true;
    }
}
