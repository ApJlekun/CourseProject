using System.Windows.Input;

namespace BarInventoryApp.Utils;

/// <summary>
/// Команда для выполнения действия без параметров.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    /// <summary>
    /// Инициализирует новый экземпляр класса RelayCommand.
    /// </summary>
    /// <param name="execute">Действие для выполнения.</param>
    /// <param name="canExecute">Функция, определяющая возможность выполнения команды.</param>
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Определяет, может ли команда выполняться.
    /// </summary>
    /// <param name="parameter">Параметр команды (не используется).</param>
    /// <returns>True, если команда может выполняться; иначе false.</returns>
    public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();

    /// <summary>
    /// Выполняет команду.
    /// </summary>
    /// <param name="parameter">Параметр команды (не используется).</param>
    public void Execute(object? parameter) => _execute();

    /// <summary>
    /// Событие, возникающее при изменении возможности выполнения команды.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}

/// <summary>
/// Команда для выполнения действия с параметром типа T.
/// </summary>
/// <typeparam name="T">Тип параметра команды.</typeparam>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool>? _canExecute;

    /// <summary>
    /// Инициализирует новый экземпляр класса RelayCommand.
    /// </summary>
    /// <param name="execute">Действие для выполнения.</param>
    /// <param name="canExecute">Функция, определяющая возможность выполнения команды.</param>
    public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Определяет, может ли команда выполняться.
    /// </summary>
    /// <param name="parameter">Параметр команды.</param>
    /// <returns>True, если команда может выполняться; иначе false.</returns>
    public bool CanExecute(object? parameter) => _canExecute == null || _canExecute((T)parameter!);

    /// <summary>
    /// Выполняет команду.
    /// </summary>
    /// <param name="parameter">Параметр команды.</param>
    public void Execute(object? parameter) => _execute((T)parameter!);

    /// <summary>
    /// Событие, возникающее при изменении возможности выполнения команды.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
