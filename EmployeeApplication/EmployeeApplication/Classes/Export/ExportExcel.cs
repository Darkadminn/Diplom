using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EmployeeWPF
{
    public class ExportExcel
    {
        public ExportExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Your Name Here");
        }

        public void ExportToExcel(DataGrid dataGrid, string filePath)
        {
            try
            {
                using (SpreadsheetDocument spreadsheetDocument =
                       SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    SheetData sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet()
                    {
                        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Данные"
                    };
                    sheets.Append(sheet);

                    uint rowIndex = 1;

                    // Заголовки столбцов
                    Row headerRow = new Row() { RowIndex = rowIndex++ };
                    int columnIndex = 0;

                    foreach (DataGridColumn column in dataGrid.Columns)
                    {
                        string header = GetColumnHeader(column);
                        string cellReference = GetExcelColumnName(columnIndex + 1) + (rowIndex - 1);
                        Cell headerCell = new Cell()
                        {
                            CellReference = cellReference,
                            DataType = CellValues.String,
                            CellValue = new CellValue(header),
                            StyleIndex = 1 // Стиль для заголовков
                        };
                        headerRow.Append(headerCell);
                        columnIndex++;
                    }
                    sheetData.Append(headerRow);

                    // Данные
                    foreach (var item in dataGrid.Items)
                    {
                        Row dataRow = new Row() { RowIndex = rowIndex++ };
                        columnIndex = 0;

                        foreach (DataGridColumn column in dataGrid.Columns)
                        {
                            string cellValue = GetCellValue(column, item);
                            string cellReference = GetExcelColumnName(columnIndex + 1) + (rowIndex - 1);

                            Cell dataCell = new Cell()
                            {
                                CellReference = cellReference,
                                DataType = CellValues.String,
                                CellValue = new CellValue(cellValue)
                            };
                            dataRow.Append(dataCell);
                            columnIndex++;
                        }
                        sheetData.Append(dataRow);
                    }

                    // Автоширина столбцов
                    Columns columns = new Columns();
                    for (int i = 0; i < dataGrid.Columns.Count; i++)
                    {
                        columns.Append(new Column()
                        {
                            Min = (uint)(i + 1),
                            Max = (uint)(i + 1),
                            Width = 20,
                            CustomWidth = true
                        });
                    }
                    worksheetPart.Worksheet.InsertBefore(columns, sheetData);

                    // Создаем простые стили
                    WorkbookStylesPart stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = CreateSimpleStylesheet();
                    stylesPart.Stylesheet.Save();

                    worksheetPart.Worksheet.Save();
                }

                MessageBox.Show($"Данные успешно экспортированы в Excel:\n{filePath}",
                               "Экспорт завершен",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}",
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        private string GetColumnHeader(DataGridColumn column)
        {
            return column.Header?.ToString() ?? $"Column {column.DisplayIndex + 1}";
        }

        private string GetCellValue(DataGridColumn column, object item)
        {
            try
            {
                if (item == null) return string.Empty;

                if (column is DataGridTextColumn textColumn)
                {
                    if (textColumn.Binding is System.Windows.Data.Binding binding)
                    {
                        string propertyName = binding.Path.Path;
                        var property = item.GetType().GetProperty(propertyName);
                        if (property != null)
                        {
                            var value = property.GetValue(item);
                            if (value is DateTime dateTime && !string.IsNullOrEmpty(binding.StringFormat))
                            {
                                return dateTime.ToString(binding.StringFormat);
                            }
                            return value?.ToString() ?? string.Empty;
                        }
                    }
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }

        private Stylesheet CreateSimpleStylesheet()
        {
            return new Stylesheet(
                new Fonts(
                    new DocumentFormat.OpenXml.Spreadsheet.Font(
                        new DocumentFormat.OpenXml.Spreadsheet.FontSize() { Val = 11 },
                        new DocumentFormat.OpenXml.Spreadsheet.FontName() { Val = "Calibri" }
                    ),
                    new DocumentFormat.OpenXml.Spreadsheet.Font(
                        new DocumentFormat.OpenXml.Spreadsheet.FontSize() { Val = 11 },
                        new DocumentFormat.OpenXml.Spreadsheet.FontName() { Val = "Calibri" },
                        new Bold()
                    )
                ),
                new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }),
                    new Fill(new PatternFill()
                    {
                        PatternType = PatternValues.Solid,
                        ForegroundColor = new ForegroundColor() { Rgb = "bef574" }
                    })
                ),
                new Borders(
                    new DocumentFormat.OpenXml.Spreadsheet.Border()
                ),
                new CellFormats(
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 },
                    new CellFormat() { FontId = 1, FillId = 1, BorderId = 0, ApplyFill = true }
                )
            );
        }

        public void ExportGraphToExcel(byte[] graphImage, string filePath, string title = "График статистики")
        {

            try
            {
                if (graphImage == null || graphImage.Length == 0)
                {
                    MessageBox.Show("Нет данных графика для экспорта", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("График");

                    // Добавляем заголовок
                    worksheet.Cells["A1"].Value = title;
                    worksheet.Cells["A1"].Style.Font.Size = 14;
                    worksheet.Cells["A1"].Style.Font.Bold = true;

                    // Добавляем дату
                    worksheet.Cells["A2"].Value = $"Дата экспорта: {DateTime.Now:dd.MM.yyyy HH:mm}";

                    // Добавляем изображение
                    if (graphImage != null && graphImage.Length > 0)
                    {
                        using (MemoryStream stream = new MemoryStream(graphImage))
                        {
                            ExcelPicture picture = worksheet.Drawings.AddPicture("Graph", stream);
                            picture.SetPosition(4, 0, 0, 0); // Строка 5 (0-based: 4)
                            picture.SetSize(600, 450); // Размер в пикселях
                        }
                    }

                    // Автоширина колонок
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Сохраняем файл
                    excelPackage.SaveAs(new FileInfo(filePath));
                }

                MessageBox.Show($"График успешно экспортирован в Excel:\n{filePath}",
                               "Экспорт завершен",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте графика в Excel: {ex.Message}",
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }
    }
}
