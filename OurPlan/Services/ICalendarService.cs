using OurPlan.DTO;
using System.Text;

namespace OurPlan.Services
{
    public static class ICalendarService
    {
        /// <summary>
        /// Convertește o listă de evenimente în format iCal (RFC 5545)
        /// </summary>
        public static string ConvertEventsToICal(List<EventModel> events)
        {
            var sb = new StringBuilder();
            
            // Header calendar
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//OurPlan//EN");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");
            
            foreach (var evt in events)
            {
                sb.AppendLine("BEGIN:VEVENT");
                
                // UID - identificator unic
                sb.AppendLine($"UID:ourplan-{evt.Id}@ourplan.app");
                
                // DTSTART - data și ora de început
                sb.AppendLine($"DTSTART:{FormatDateTime(evt.StartDate)}");
                
                // DTEND - data și ora de sfârșit
                sb.AppendLine($"DTEND:{FormatDateTime(evt.EndDate)}");
                
                // DTSTAMP - timestamp când a fost creat/actualizat
                sb.AppendLine($"DTSTAMP:{FormatDateTime(DateTime.UtcNow)}");
                
                // SUMMARY - titlul evenimentului
                sb.AppendLine($"SUMMARY:{EscapeText(evt.Title)}");
                
                // DESCRIPTION - descrierea (dacă există)
                if (!string.IsNullOrWhiteSpace(evt.Description))
                {
                    sb.AppendLine($"DESCRIPTION:{EscapeText(evt.Description)}");
                }
                
                // LOCATION - locația (dacă există)
                if (!string.IsNullOrWhiteSpace(evt.Location))
                {
                    sb.AppendLine($"LOCATION:{EscapeText(evt.Location)}");
                }
                
                // STATUS
                sb.AppendLine("STATUS:CONFIRMED");
                
                // SEQUENCE - pentru tracking modificări
                sb.AppendLine("SEQUENCE:0");
                
                // CREATED - data creării
                sb.AppendLine($"CREATED:{FormatDateTime(DateTime.UtcNow)}");
                
                // LAST-MODIFIED
                sb.AppendLine($"LAST-MODIFIED:{FormatDateTime(DateTime.UtcNow)}");
                
                // TRANSP - disponibilitate
                sb.AppendLine("TRANSP:OPAQUE");
                
                sb.AppendLine("END:VEVENT");
            }
            
            sb.AppendLine("END:VCALENDAR");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Parsează un fișier iCal și returnează o listă de evenimente
        /// </summary>
        public static List<EventModel> ParseICalToEvents(string icalContent, int? defaultGroupId = null)
        {
            var events = new List<EventModel>();
            var lines = icalContent.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            
            EventModel? currentEvent = null;
            bool inEvent = false;
            var currentLine = new StringBuilder();
            
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                
                // Continuă linia dacă următoarea începe cu spațiu sau tab (folding în iCal)
                if (i + 1 < lines.Length && (lines[i + 1].StartsWith(" ") || lines[i + 1].StartsWith("\t")))
                {
                    currentLine.Append(line.TrimEnd());
                    continue;
                }
                
                // Dacă linia curentă începe cu spațiu sau tab, o adăugăm la linia anterioară
                if ((line.StartsWith(" ") || line.StartsWith("\t")) && currentLine.Length > 0)
                {
                    currentLine.Append(line.TrimStart());
                    line = currentLine.ToString();
                    currentLine.Clear();
                }
                else if (currentLine.Length > 0)
                {
                    line = currentLine.ToString() + line;
                    currentLine.Clear();
                }
                
                if (line.StartsWith("BEGIN:VEVENT"))
                {
                    inEvent = true;
                    currentEvent = new EventModel
                    {
                        IsShared = true,
                        CreatedByUserId = 0 // Va fi setat de serviciu
                    };
                }
                else if (line.StartsWith("END:VEVENT") && inEvent && currentEvent != null)
                {
                    // Validăm că evenimentul are date valide
                    if (currentEvent.StartDate != default(DateTime) && 
                        currentEvent.EndDate != default(DateTime) &&
                        !string.IsNullOrWhiteSpace(currentEvent.Title))
                    {
                        events.Add(currentEvent);
                    }
                    inEvent = false;
                    currentEvent = null;
                }
                else if (inEvent && currentEvent != null)
                {
                    ParseEventLine(line, currentEvent);
                }
            }
            
            return events;
        }
        
        private static void ParseEventLine(string line, EventModel eventModel)
        {
            if (line.Contains(":"))
            {
                var parts = line.Split(new[] { ':' }, 2);
                if (parts.Length != 2) return;
                
                var key = parts[0].ToUpper();
                var value = parts[1];
                
                // Elimină parametrii din key (ex: DTSTART;VALUE=DATE:20240101)
                var keyOnly = key.Split(';')[0];
                
                switch (keyOnly)
                {
                    case "SUMMARY":
                        eventModel.Title = UnescapeText(value);
                        break;
                    case "DESCRIPTION":
                        eventModel.Description = UnescapeText(value);
                        break;
                    case "LOCATION":
                        eventModel.Location = UnescapeText(value);
                        break;
                    case "DTSTART":
                        eventModel.StartDate = ParseDateTime(value);
                        break;
                    case "DTEND":
                        eventModel.EndDate = ParseDateTime(value);
                        break;
                }
            }
        }
        
        private static DateTime ParseDateTime(string dateTimeString)
        {
            // Format iCal: 20240101T120000Z sau 20240101T120000
            try
            {
                if (dateTimeString.Length == 8)
                {
                    // Doar dată: YYYYMMDD
                    var year = int.Parse(dateTimeString.Substring(0, 4));
                    var month = int.Parse(dateTimeString.Substring(4, 2));
                    var day = int.Parse(dateTimeString.Substring(6, 2));
                    return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
                }
                else if (dateTimeString.Length >= 15 && dateTimeString.Contains('T'))
                {
                    // Dată și oră: YYYYMMDDTHHMMSS sau YYYYMMDDTHHMMSSZ
                    var datePart = dateTimeString.Substring(0, 8);
                    var timePart = dateTimeString.Substring(9, 6);
                    
                    var year = int.Parse(datePart.Substring(0, 4));
                    var month = int.Parse(datePart.Substring(4, 2));
                    var day = int.Parse(datePart.Substring(6, 2));
                    var hour = int.Parse(timePart.Substring(0, 2));
                    var minute = int.Parse(timePart.Substring(2, 2));
                    var second = int.Parse(timePart.Substring(4, 2));
                    
                    var isUtc = dateTimeString.EndsWith("Z");
                    var kind = isUtc ? DateTimeKind.Utc : DateTimeKind.Unspecified;
                    
                    return new DateTime(year, month, day, hour, minute, second, kind);
                }
            }
            catch
            {
                // Dacă parsing-ul eșuează, returnează DateTime.MinValue
            }
            
            return DateTime.UtcNow;
        }
        
        private static string FormatDateTime(DateTime dateTime)
        {
            // Convertește la UTC dacă nu este deja
            var utc = dateTime.Kind == DateTimeKind.Utc 
                ? dateTime 
                : dateTime.ToUniversalTime();
            
            // Format: YYYYMMDDTHHMMSSZ
            return utc.ToString("yyyyMMddTHHmmss") + "Z";
        }
        
        private static string EscapeText(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            return text
                .Replace("\\", "\\\\")
                .Replace(",", "\\,")
                .Replace(";", "\\;")
                .Replace("\n", "\\n")
                .Replace("\r", "");
        }
        
        private static string UnescapeText(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            return text
                .Replace("\\n", "\n")
                .Replace("\\,", ",")
                .Replace("\\;", ";")
                .Replace("\\\\", "\\");
        }
    }
}

