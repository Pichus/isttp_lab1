using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.UseCases.CoworkingBookings.GenerateDocument;

namespace StudentParliamentSystem.Infrastructure.CoworkingBookings;

public class CoworkingDocumentGenerator : ICoworkingDocumentGenerator
{
    public byte[] GenerateDocument(IEnumerable<CoworkingBooking> bookings, string receiver, string documentDate,
        string sender)
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, "CoworkingBookings", "Templates",
            "CoworkingDocumentTemplate.docx");

        using var memoryStream = new MemoryStream();
        if (File.Exists(templatePath))
        {
            var templateBytes = File.ReadAllBytes(templatePath);
            memoryStream.Write(templateBytes, 0, templateBytes.Length);
        }
        else
        {
            using var emptyDoc = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document);
            emptyDoc.AddMainDocumentPart().Document = new Document(new Body(
                new Paragraph(new Run(new Text("{{Receiver}}"))),
                new Paragraph(new Run(new Text("{{MainBody}}"))),
                new Paragraph(new Run(new Text("{{EventName}}"))),
                new Paragraph(new Run(new Text("{{EventStart}}"))),
                new Paragraph(new Run(new Text("{{EventEnd}}"))),
                new Paragraph(new Run(new Text("{{EventOrganizers}}"))),
                new Paragraph(new Run(new Text("{{Date}}"))),
                new Paragraph(new Run(new Text("{{Sender}}")))
            ));
        }

        memoryStream.Position = 0;

        using (var wordDocument = WordprocessingDocument.Open(memoryStream, true))
        {
            var body = wordDocument.MainDocumentPart!.Document.Body!;
            
            ReplaceText(body, "{{Receiver}}", receiver ?? "");
            ReplaceText(body, "{{Date}}", documentDate ?? "");
            ReplaceText(body, "{{Sender}}", sender ?? "");

            var eventNamePara = body.Descendants<Paragraph>()
                .FirstOrDefault(p => p.InnerText.Contains("{{EventName"));
            var eventOrgPara = body.Descendants<Paragraph>()
                .FirstOrDefault(p => p.InnerText.Contains("{{EventOrganizers"));

            if (eventNamePara != null && eventOrgPara != null)
            {
                var templateParagraphs = new List<Paragraph>();
                var current = eventNamePara;
                while (current != null)
                {
                    templateParagraphs.Add(current);
                    if (current == eventOrgPara)
                    {
                        break;
                    }

                    current = current.NextSibling<Paragraph>();
                }
                
                var insertBeforeNode = eventNamePara;

                var coworkingBookings = bookings as CoworkingBooking[] ?? bookings.ToArray();
                if (!coworkingBookings.Any())
                {
                    var noBookings = new Paragraph(new Run(new Text("У цей період заходів не заплановано.")));
                    insertBeforeNode.Parent!.InsertBefore(noBookings, insertBeforeNode);
                }
                else
                {
                    foreach (var booking in coworkingBookings)
                    {
                        var eventDate = booking.StartTimeUtc.ToLocalTime().ToString("dd.MM.yyyy");
                        var startTime = booking.StartTimeUtc.ToLocalTime().ToString("HH:mm");
                        var endTime = booking.EndTimeUtc.ToLocalTime().ToString("HH:mm");
                        var eventName = booking.Event.Title;

                        var organizerName = booking.Event.CreatedByUser.LastName + " " +
                                            booking.Event.CreatedByUser.FirstName;
                        
                        var allOrganizers = new List<string> { organizerName };
                        
                        if (booking.SpaceManager != null)
                        {
                            allOrganizers.Add(booking.SpaceManager.LastName + " " + booking.SpaceManager.FirstName);
                        }

                        if (booking.Event.EventOrganizers.Any())
                        {
                            allOrganizers.AddRange(booking.Event.EventOrganizers.Select(eo => eo.User.LastName + " " + eo.User.FirstName));
                        }

                        var fullOrganizers = string.Join(", ", allOrganizers.Distinct());

                        foreach (var tPara in templateParagraphs)
                        {
                            var clone = (Paragraph)tPara.CloneNode(true);
                            ReplaceText(clone, "{{EventName}}", eventName);
                            ReplaceText(clone, "{{EventName}", eventName); // fallback for typo
                            ReplaceText(clone, "{{EventStart}}", $"{eventDate} {startTime}");
                            ReplaceText(clone, "{{EventStart}", $"{eventDate} {startTime}");
                            ReplaceText(clone, "{{EventEnd}}", $"{eventDate} {endTime}");
                            ReplaceText(clone, "{{EventEnd}", $"{eventDate} {endTime}");
                            ReplaceText(clone, "{{EventOrganizers}}", fullOrganizers);
                            ReplaceText(clone, "{{EventOrganizers}", fullOrganizers);

                            insertBeforeNode.Parent!.InsertBefore(clone, insertBeforeNode);
                        }

                        insertBeforeNode.Parent!.InsertBefore(new Paragraph(new Run(new Text(""))),
                            insertBeforeNode);
                    }
                }

                foreach (var p in templateParagraphs)
                {
                    p.Remove();
                }
            }

            var mainBodyPara = body.Descendants<Paragraph>().FirstOrDefault(p => p.InnerText.Contains("{{MainBody}}"));
            if (mainBodyPara != null)
            {
                mainBodyPara.Remove();
            }

            wordDocument.MainDocumentPart.Document.Save();
        }

        return memoryStream.ToArray();
    }

    private void ReplaceText(OpenXmlElement element, string tag, string replacement)
    {
        var paragraphs = new List<Paragraph>();
        if (element is Paragraph p && p.InnerText.Contains(tag))
        {
            paragraphs.Add(p);
        }
        else
        {
            paragraphs.AddRange(element.Descendants<Paragraph>().Where(para => para.InnerText.Contains(tag)));
        }

        foreach (var para in paragraphs)
        {
            var text = para.InnerText.Replace(tag, replacement);
            var firstRunProperties = para.Elements<Run>().FirstOrDefault()?.RunProperties?.CloneNode(true);
            
            para.RemoveAllChildren<Run>();
            var newRun = new Run();
            if (firstRunProperties != null)
            {
                newRun.AppendChild(firstRunProperties);
            }
            
            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                newRun.AppendChild(new Text(lines[i]) { Space = SpaceProcessingModeValues.Preserve });
                if (i < lines.Length - 1)
                {
                    newRun.AppendChild(new Break());
                }
            }

            para.AppendChild(newRun);
        }
    }
}