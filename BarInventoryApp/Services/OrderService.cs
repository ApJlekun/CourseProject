using BarInventoryApp.DataContexts;
using BarInventoryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BarInventoryApp.Services;

/// <summary>
/// Сервис для работы с заказами.
/// </summary>
public class OrderService
{
    #region Поля

    private readonly AppDbContext _context;

    #endregion

    #region Конструктор

    /// <summary>
    /// Инициализирует новый экземпляр класса OrderService.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    public OrderService(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получает все заказы с загруженными связанными данными (ингредиент и создатель с ролью).
    /// </summary>
    /// <returns>Список всех заказов с деталями.</returns>
    public async Task<List<Order>> GetAllWithDetailsAsync()
    {
        return await _context.Orders
            .Include(o => o.Ingredient)
            .Include(o => o.CreatedByNavigation)
                .ThenInclude(u => u.Role)
            .ToListAsync();
    }

    /// <summary>
    /// Добавляет новый заказ в базу данных.
    /// </summary>
    /// <param name="order">Заказ для добавления.</param>
    /// <exception cref="ArgumentNullException">Если order равен null.</exception>
    public async Task AddAsync(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Обновляет существующий заказ в базе данных.
    /// </summary>
    /// <param name="order">Заказ для обновления.</param>
    /// <exception cref="ArgumentNullException">Если order равен null.</exception>
    public async Task UpdateAsync(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Удаляет заказ по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор заказа для удаления.</param>
    /// <returns>True, если заказ был удален; иначе false.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion
}
