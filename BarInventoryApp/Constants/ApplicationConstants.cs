namespace BarInventoryApp.Constants;

/// <summary>
/// Константы приложения, используемые во всем проекте.
/// </summary>
public static class ApplicationConstants
{
    /// <summary>
    /// Константы ролей пользователей системы.
    /// </summary>
    public static class Roles
    {
        /// <summary>
        /// Роль бармена - может просматривать остатки ингредиентов.
        /// </summary>
        public const string Barmen = "Barmen";

        /// <summary>
        /// Роль менеджера - может просматривать остатки, создавать и редактировать заказы, экспортировать данные.
        /// </summary>
        public const string Manager = "Manager";

        /// <summary>
        /// Роль администратора - имеет полный доступ ко всем функциям.
        /// </summary>
        public const string Admin = "Admin";
    }

    /// <summary>
    /// Идентификаторы ролей в базе данных.
    /// </summary>
    public static class RoleIds
    {
        /// <summary>
        /// Идентификатор роли бармена.
        /// </summary>
        public const int Barmen = 1;

        /// <summary>
        /// Идентификатор роли менеджера.
        /// </summary>
        public const int Manager = 2;

        /// <summary>
        /// Идентификатор роли администратора.
        /// </summary>
        public const int Admin = 3;
    }

    /// <summary>
    /// Сообщения для пользователя.
    /// </summary>
    public static class Messages
    {
        public const string LoginPasswordRequired = "Введите логин и пароль.";
        public const string InvalidCredentials = "Неверный логин или пароль.";
        public const string UnknownRole = "Неизвестная роль.";
        public const string LoginError = "Ошибка входа: {0}";
        public const string LoadingIngredientsError = "Ошибка загрузки ингредиентов: {0}";
        public const string LoadingOrdersError = "Ошибка загрузки заказов: {0}";
        public const string LoadingUsersError = "Ошибка загрузки пользователей: {0}";
        public const string SelectIngredientForEdit = "Выберите ингредиент для редактирования.";
        public const string SelectOrderForEdit = "Выберите заказ для редактирования.";
        public const string DeleteIngredientConfirmation = "Удалить ингредиент '{0}'?";
        public const string DeleteOrderConfirmation = "Удалить заказ от {0}?";
        public const string RoleUpdated = "Роль обновлена.";
        public const string UserCreated = "Пользователь создан.";
        public const string LoginPasswordRequiredForUser = "Заполните логин и пароль.";
        public const string UserExists = "Пользователь с таким логином уже существует.";
        public const string ExcelExportSuccess = "Файл успешно сохранён!";
    }
}

