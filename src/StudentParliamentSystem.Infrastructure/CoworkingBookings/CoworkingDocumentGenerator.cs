using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.UseCases.CoworkingBookings.GenerateDocument;

namespace StudentParliamentSystem.Infrastructure.CoworkingBookings;

public class CoworkingDocumentGenerator : ICoworkingDocumentGenerator
{
    public byte[] GenerateDocument(IEnumerable<CoworkingBooking> bookings, DateTime spanStart, DateTime spanEnd)
    {
        using var memoryStream = new MemoryStream();
        using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
        {
            var mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            // Add Header
            AddParagraph(body, "Виконуючому обов'язки декана факультету комп'ютерних наук та кібернетики", JustificationValues.Right);
            AddParagraph(body, "Київського національного університету імені Тараса Шевченка", JustificationValues.Right);
            AddParagraph(body, "Людмилі ОМЕЛЬЧУК", JustificationValues.Right);
            AddEmptyLine(body);
            AddEmptyLine(body);

            if (!bookings.Any())
            {
                AddParagraph(body, $"У період з {spanStart:dd.MM.yyyy} по {spanEnd:dd.MM.yyyy} заходів не заплановано.", JustificationValues.Left);
            }
            else
            {
                foreach (var booking in bookings)
                {
                    var eventDate = booking.StartTimeUtc.ToLocalTime().ToString("dd.MM.yyyy");
                    var startTime = booking.StartTimeUtc.ToLocalTime().ToString("HH:mm");
                    var endTime = booking.EndTimeUtc.ToLocalTime().ToString("HH:mm");
                    var eventName = booking.Event.Title;

                    var organizerName = booking.Event.CreatedByUser.LastName + " " + booking.Event.CreatedByUser.FirstName;
                    var spaceManagerName = booking.SpaceManager != null
                        ? booking.SpaceManager.LastName + " " + booking.SpaceManager.FirstName
                        : "Не призначено";

                    var others = booking.Event.EventOrganizers != null && booking.Event.EventOrganizers.Any()
                        ? string.Join(", ", booking.Event.EventOrganizers.Select(eo => eo.User.LastName + " " + eo.User.FirstName))
                        : "Немає";

                    AddParagraph(body, $"Прошу дозволити використати приміщення читалки (ауд. 01) на {eventDate} з {startTime} до {endTime} для проведення заходу \"{eventName}\".", JustificationValues.Both);
                    AddParagraph(body, $"Відповідальний за проведення заходу: {organizerName}.", JustificationValues.Both);
                    AddParagraph(body, "Бере на себе відповідальність відкрити аудиторію та підтримувати порядок:", JustificationValues.Both);
                    AddParagraph(body, $"Організатор {organizerName}. Додаткові відповідальні люди: {spaceManagerName}. Інші організатори: {others}.", JustificationValues.Both);
                    AddEmptyLine(body);
                }
            }

            AddEmptyLine(body);
            AddEmptyLine(body);

            // Add Footer
            var footerTable = new Table();
            var tr = new TableRow();
            
            var tcDate = new TableCell(new Paragraph(new Run(new Text(DateTime.Now.ToString("dd.MM.yyyy")))));
            tcDate.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
            
            var tcSign = new TableCell(
                new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Right }),
                    new Run(new Text("Представник СП ФКНК___________________"))
                ));
            tcSign.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "5000" }));

            tr.Append(tcDate, tcSign);
            footerTable.Append(tr);
            
            body.Append(footerTable);
        }

        return memoryStream.ToArray();
    }

    private static void AddParagraph(Body body, string text, JustificationValues justification)
    {
        var paragraph = body.AppendChild(new Paragraph());
        var paragraphProperties = paragraph.AppendChild(new ParagraphProperties());
        var justificationElement = new Justification() { Val = justification };
        paragraphProperties.AppendChild(justificationElement);

        var run = paragraph.AppendChild(new Run());
        var textElement = new Text(text) { Space = SpaceProcessingModeValues.Preserve };
        run.AppendChild(textElement);
    }

    private static void AddEmptyLine(Body body)
    {
        body.AppendChild(new Paragraph(new Run(new Text(""))));
    }
}
