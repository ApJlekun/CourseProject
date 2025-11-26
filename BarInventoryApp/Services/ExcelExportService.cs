using BarInventoryApp.Models;
using ClosedXML.Excel;
using Microsoft.Win32;
using System.Windows;
using BarInventoryApp.Constants;

namespace BarInventoryApp.Services;

/// <summary>
/// Сервис для экспорта данных в Excel.
/// </summary>
public class ExcelExportService
{
    private const string ExcelFileFilter = "Excel files (*.xlsx)|*.xlsx";
    private const string WorksheetName = "Заказы";
    private const string DateFormat = "dd.MM.yyyy HH:mm";
    private const string FileNamePrefix = "Заказы_";
    private const string FileNameDateFormat = "yyyy-MM-dd_HH-mm";
    private const string SuccessTitle = "Успех";

    /// <summary>
    /// Экспортирует список заказов в файл Excel.
    /// </summary>
    /// <param name="orders">Список заказов для экспорта.</param>
    /// <exception cref="ArgumentNullException">Если orders равен null.</exception>
    public void ExportOrders(List<Order> orders)
    {
        if (orders == null)
            throw new ArgumentNullException(nameof(orders));

        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(WorksheetName);

        // Заголовки колонок
        worksheet.Cell(1, 1).Value = "Ингредиент";
        worksheet.Cell(1, 2).Value = "Количество";
        worksheet.Cell(1, 3).Value = "Ед.";
        worksheet.Cell(1, 4).Value = "Дата";
        worksheet.Cell(1, 5).Value = "Автор";

        // Заполнение данных
        for (int i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            var row = i + 2;

            worksheet.Cell(row, 1).Value = order.Ingredient?.Name ?? string.Empty;
            worksheet.Cell(row, 2).Value = order.Quantity;
            worksheet.Cell(row, 3).Value = order.Ingredient?.Unit ?? string.Empty;
            worksheet.Cell(row, 4).Value = order.OrderDate.ToString(DateFormat);
            worksheet.Cell(row, 5).Value = order.CreatedByNavigation?.Login ?? string.Empty;
        }

        // Диалог сохранения файла
        var dialog = new SaveFileDialog
        {
            Filter = ExcelFileFilter,
            FileName = $"{FileNamePrefix}{DateTime.Now:FileNameDateFormat}.xlsx"
        };

        if (dialog.ShowDialog() == true)
        {
            workbook.SaveAs(dialog.FileName);
            MessageBox.Show(
                ApplicationConstants.Messages.ExcelExportSuccess,
                SuccessTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
