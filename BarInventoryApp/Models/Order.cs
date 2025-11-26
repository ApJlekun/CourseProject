namespace BarInventoryApp.Models;

/// <summary>
/// Представляет заказ на пополнение запасов ингредиента.
/// </summary>
public partial class Order
{
    /// <summary>
    /// Уникальный идентификатор заказа.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор ингредиента, для которого создан заказ.
    /// </summary>
    public int IngredientId { get; set; }

    /// <summary>
    /// Количество ингредиента в заказе.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Дата и время создания заказа.
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Идентификатор пользователя, создавшего заказ.
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// Пользователь, создавший заказ.
    /// </summary>
    public virtual User CreatedByNavigation { get; set; } = null!;

    /// <summary>
    /// Ингредиент, для которого создан заказ.
    /// </summary>
    public virtual Ingredient Ingredient { get; set; } = null!;
}
