using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EmployeeWPF
{
    public class ExportPDF
    {
        private BaseFont _russianFont;

        public ExportPDF()
        {
            RegisterRussianFont();
        }

        private void RegisterRussianFont()
        {
            try
            {
                string[] possibleFonts = {
                    "arial.ttf",
                    "times.ttf",
                    "cour.ttf",
                    "tahoma.ttf",
                    "verdana.ttf"
                };

                string fontsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

                foreach (string fontFile in possibleFonts)
                {
                    string fontPath = Path.Combine(fontsFolder, fontFile);
                    if (File.Exists(fontPath))
                    {
                        _russianFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        break;
                    }
                }

                if (_russianFont == null)
                {
                    _russianFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                }
            }
            catch
            {
                _russianFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
            }
        }

        public void ExportToPdf(DataGrid dataGrid, string filePath, string tableName)
        {
            try
            {
                if (dataGrid.Items.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Document document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                Font titleFont = new Font(_russianFont, 16, Font.BOLD, BaseColor.BLACK);
                Font dateFont = new Font(_russianFont, 10, Font.ITALIC, BaseColor.BLACK);
                Font headerFont = new Font(_russianFont, 10, Font.BOLD, BaseColor.BLACK);
                Font cellFont = new Font(_russianFont, 9, Font.NORMAL, BaseColor.BLACK);

                Paragraph title = new Paragraph("Экспорт данных", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20;
                document.Add(title);

                Paragraph date = new Paragraph($"Дата экспорта: {DateTime.Now:dd.MM.yyyy HH:mm}", dateFont);
                date.Alignment = Element.ALIGN_RIGHT;
                date.SpacingAfter = 20;
                document.Add(date);

                PdfPTable table = new PdfPTable(dataGrid.Columns.Count());
                table.WidthPercentage = 100;

                float[] columnWidths;

                if(dataGrid.Columns.Count() == 8) columnWidths = new float[] { 1f, 0.8f, 1.5f, 1f, 2f, 2f, 1.5f, 2f };
                else columnWidths = new float[] { 1f, 0.8f, 1.5f, 1f, 2f, 2f};


                table.SetWidths(columnWidths);

                string[] headers;

                if(tableName == "Приемы")
                {
                    headers = new string[]{
                        "Дата",
                        "Время",
                        "Пациент",
                        "Статус приема",
                        "Состояние пациента",
                        "Жалобы",
                        "Диагноз",
                        "Рекомендации"
                    };
                }
                else if(tableName == "Услуги")
                {
                    headers = new string[]{
                        "Дата",
                        "Пациент",
                        "Услуга",
                        "Тип",
                        "Количество",
                        "Количество пройденных"
                    };
                }
                else
                {
                    headers = new string[]{
                        "Дата",
                        "Пациент",
                        "Услуга",
                        "Тип",
                        "Количество",
                        "Количество пройденных"
                    };
                }


                foreach (string header in headers)
                {
                    Phrase headerPhrase = new Phrase(header, headerFont);

                    PdfPCell headerCell = new PdfPCell(headerPhrase);
                    headerCell.BackgroundColor = new BaseColor(190, 245, 116);
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    headerCell.Padding = 8;
                    headerCell.BorderColor = BaseColor.BLACK;
                    headerCell.BorderWidth = 1f;
                    headerCell.Border = Rectangle.BOX;

                    table.AddCell(headerCell);
                }

                string[] propertyNames;
                string[] formats;

                foreach (var item in dataGrid.Items)
                {
                    if (item != null)
                    {
                        var itemType = item.GetType();

                        if(tableName == "Приемы")
                        {
                            propertyNames = new string[]{
                                "date",
                                "date",
                                "patient",
                                "status",
                                "objective",
                                "subjective",
                                "diagnosis",
                                "recommendation"
                            };

                            formats = new string[]{
                                "dd.MM.yyyy",
                                "HH:mm",
                                null, null, null, null, null, null
                            };
                        }
                        else if(tableName == "Услуги")
                        {
                            propertyNames = new string[]{
                                "date",
                                "patient",
                                "medicalService",
                                "type",
                                "count",
                                "countСompleted"
                            };

                            formats = new string[]{
                                "dd.MM.yyyy",
                                null, null, null, null, null
                            };
                        }
                        else
                        {
                            propertyNames = new string[]{
                                "date",
                                "date",
                                "patient",
                                "status",
                                "objective",
                                "subjective",
                                "diagnosis",
                                "recommendation"
                            };

                            formats = new string[]{
                                "dd.MM.yyyy",
                                "HH:mm",
                                null, null, null, null, null, null
                            };
                        }


                        for (int i = 0; i < propertyNames.Length; i++)
                        {
                            string propertyName = propertyNames[i];
                            string format = formats[i];

                            string cellValue = GetPropertyValue(item, itemType, propertyName, format);

                            PdfPCell cell = new PdfPCell(new Phrase(cellValue, cellFont));
                            cell.Padding = 4;
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.BorderColor = BaseColor.LIGHT_GRAY;
                            cell.BorderWidth = 0.5f;
                            cell.Border = Rectangle.BOX;

                            table.AddCell(cell);
                        }
                    }
                }

                document.Add(table);
                document.Close();

                MessageBox.Show($"Данные успешно экспортированы в PDF:\n{filePath}",
                               "Экспорт завершен",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в PDF: {ex.Message}",
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        private string GetPropertyValue(object item, Type itemType, string propertyName, string format)
        {
            try
            {
                if (string.IsNullOrEmpty(propertyName))
                    return string.Empty;

                var property = itemType.GetProperty(propertyName);
                if (property == null)
                    return string.Empty;

                var value = property.GetValue(item);
                if (value == null)
                    return string.Empty;

                if (value is DateTime dateTime && !string.IsNullOrEmpty(format))
                {
                    return dateTime.ToString(format);
                }

                return value.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        // Метод для экспорта графика в PDF
        public void ExportGraphToPdf(byte[] graphImage, string filePath, string title = "График статистики")
        {
            try
            {
                if (graphImage == null || graphImage.Length == 0)
                {
                    MessageBox.Show("Нет данных графика для экспорта", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Document document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                Font titleFont = new Font(_russianFont, 16, Font.BOLD, BaseColor.BLACK);
                Font dateFont = new Font(_russianFont, 10, Font.ITALIC, BaseColor.BLACK);

                Paragraph pdfTitle = new Paragraph(title, titleFont);
                pdfTitle.Alignment = Element.ALIGN_CENTER;
                pdfTitle.SpacingAfter = 20;
                document.Add(pdfTitle);

                Paragraph date = new Paragraph($"Дата экспорта: {DateTime.Now:dd.MM.yyyy HH:mm}", dateFont);
                date.Alignment = Element.ALIGN_RIGHT;
                date.SpacingAfter = 20;
                document.Add(date);

                // Добавляем изображение графика
                using (MemoryStream imageStream = new MemoryStream(graphImage))
                {
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageStream);

                    // Масштабируем изображение под размер страницы
                    if (image.Width > document.PageSize.Width - 20)
                    {
                        image.ScaleToFit(document.PageSize.Width - 20, document.PageSize.Height - 100);
                    }

                    image.Alignment = Element.ALIGN_CENTER;
                    document.Add(image);
                }

                document.Close();

                MessageBox.Show($"График успешно экспортирован в PDF:\n{filePath}",
                               "Экспорт завершен",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте графика в PDF: {ex.Message}",
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }
    }
}
