using System.IO.Compression;
using System.Text;

namespace Social_Network.Helpers
{
    // Генерация настоящего .xlsx без сторонних библиотек (OpenXML-минимум).
    // Первая строка каждого листа — жирный заголовок; задаются ширины колонок.
    public static class ExcelExporter
    {
        // Стили строк (индексы соответствуют cellXfs в Styles())
        public const int StyleNormal = 0;
        public const int StyleBold = 1;
        public const int StyleHeader = 2;   // шапка таблицы — оливковая заливка, белый жирный
        public const int StyleTitle = 3;    // заголовок листа — коричневая заливка, белый жирный
        public const int StyleLabel = 4;    // важная ячейка — светлая заливка, жирный тёмный

        public sealed class Sheet
        {
            public string Name { get; set; } = "Лист1";
            public List<List<object?>> Rows { get; } = new();
            public List<double> ColumnWidths { get; } = new();
            // Индивидуальный стиль для строки (по индексу)
            public Dictionary<int, int> RowStyle { get; } = new();

            public void Add(params object?[] cells) => Rows.Add(cells.ToList());

            public void AddStyled(int style, params object?[] cells)
            {
                RowStyle[Rows.Count] = style;
                Rows.Add(cells.ToList());
            }
        }

        public static byte[] Build(IEnumerable<Sheet> sheets)
        {
            var list = sheets.ToList();
            using var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                Write(zip, "[Content_Types].xml", ContentTypes(list.Count));
                Write(zip, "_rels/.rels", Rels());
                Write(zip, "xl/workbook.xml", Workbook(list));
                Write(zip, "xl/_rels/workbook.xml.rels", WorkbookRels(list.Count));
                Write(zip, "xl/styles.xml", Styles());
                for (int i = 0; i < list.Count; i++)
                    Write(zip, $"xl/worksheets/sheet{i + 1}.xml", Worksheet(list[i]));
            }
            return ms.ToArray();
        }

        private static void Write(ZipArchive zip, string path, string content)
        {
            var entry = zip.CreateEntry(path, CompressionLevel.Optimal);
            using var w = new StreamWriter(entry.Open(), new UTF8Encoding(false));
            w.Write(content);
        }

        private static string ContentTypes(int sheets)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">");
            sb.Append("<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>");
            sb.Append("<Default Extension=\"xml\" ContentType=\"application/xml\"/>");
            sb.Append("<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>");
            sb.Append("<Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/>");
            for (int i = 1; i <= sheets; i++)
                sb.Append($"<Override PartName=\"/xl/worksheets/sheet{i}.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>");
            sb.Append("</Types>");
            return sb.ToString();
        }

        private static string Rels() =>
            "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
            "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
            "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>" +
            "</Relationships>";

        // Стили в тему приложения: жирный, шапка (олива), заголовок (коричневый), важная ячейка (тан)
        private static string Styles() =>
            "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
            "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">" +
            "<fonts count=\"4\">" +
            "<font><sz val=\"11\"/><name val=\"Calibri\"/></font>" +
            "<font><b/><sz val=\"11\"/><name val=\"Calibri\"/></font>" +
            "<font><b/><sz val=\"11\"/><color rgb=\"FFFFFFFF\"/><name val=\"Calibri\"/></font>" +
            "<font><b/><sz val=\"13\"/><color rgb=\"FFFFFFFF\"/><name val=\"Calibri\"/></font>" +
            "</fonts>" +
            "<fills count=\"5\">" +
            "<fill><patternFill patternType=\"none\"/></fill>" +
            "<fill><patternFill patternType=\"gray125\"/></fill>" +
            "<fill><patternFill patternType=\"solid\"><fgColor rgb=\"FF5A6E2E\"/></patternFill></fill>" +
            "<fill><patternFill patternType=\"solid\"><fgColor rgb=\"FF4A2810\"/></patternFill></fill>" +
            "<fill><patternFill patternType=\"solid\"><fgColor rgb=\"FFEDE0C8\"/></patternFill></fill>" +
            "</fills>" +
            "<borders count=\"1\"><border/></borders>" +
            "<cellStyleXfs count=\"1\"><xf/></cellStyleXfs>" +
            "<cellXfs count=\"5\">" +
            "<xf/>" +
            "<xf fontId=\"1\" applyFont=\"1\"/>" +
            "<xf fontId=\"2\" fillId=\"2\" applyFont=\"1\" applyFill=\"1\"><alignment vertical=\"center\"/></xf>" +
            "<xf fontId=\"3\" fillId=\"3\" applyFont=\"1\" applyFill=\"1\"><alignment vertical=\"center\"/></xf>" +
            "<xf fontId=\"1\" fillId=\"4\" applyFont=\"1\" applyFill=\"1\"/>" +
            "</cellXfs>" +
            "</styleSheet>";

        private static string Workbook(List<Sheet> sheets)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets>");
            for (int i = 0; i < sheets.Count; i++)
                sb.Append($"<sheet name=\"{Esc(sheets[i].Name)}\" sheetId=\"{i + 1}\" r:id=\"rId{i + 1}\"/>");
            sb.Append("</sheets></workbook>");
            return sb.ToString();
        }

        private static string WorkbookRels(int sheets)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
            for (int i = 1; i <= sheets; i++)
                sb.Append($"<Relationship Id=\"rId{i}\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet{i}.xml\"/>");
            sb.Append($"<Relationship Id=\"rId{sheets + 1}\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>");
            sb.Append("</Relationships>");
            return sb.ToString();
        }

        private static string Worksheet(Sheet sheet)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");

            // Автоширина: если ширины не заданы — считаем по содержимому
            var widths = sheet.ColumnWidths.Count > 0 ? sheet.ColumnWidths : AutoWidths(sheet);
            if (widths.Count > 0)
            {
                sb.Append("<cols>");
                for (int c = 0; c < widths.Count; c++)
                    sb.Append($"<col min=\"{c + 1}\" max=\"{c + 1}\" width=\"{widths[c].ToString(System.Globalization.CultureInfo.InvariantCulture)}\" customWidth=\"1\"/>");
                sb.Append("</cols>");
            }

            sb.Append("<sheetData>");
            for (int r = 0; r < sheet.Rows.Count; r++)
            {
                sb.Append($"<row r=\"{r + 1}\">");
                var row = sheet.Rows[r];
                int styleId = sheet.RowStyle.TryGetValue(r, out var sid)
                    ? sid
                    : (sheet.RowStyle.Count == 0 && r == 0 ? 1 : 0); // совместимость: первая строка жирная
                string style = styleId > 0 ? $" s=\"{styleId}\"" : string.Empty;
                for (int c = 0; c < row.Count; c++)
                {
                    var cellRef = $"{Col(c)}{r + 1}";
                    var val = row[c];
                    if (val is int or long or double or float or decimal)
                        sb.Append($"<c r=\"{cellRef}\"{style}><v>{Convert.ToString(val, System.Globalization.CultureInfo.InvariantCulture)}</v></c>");
                    else
                        sb.Append($"<c r=\"{cellRef}\"{style} t=\"inlineStr\"><is><t xml:space=\"preserve\">{Esc(val?.ToString() ?? string.Empty)}</t></is></c>");
                }
                sb.Append("</row>");
            }
            sb.Append("</sheetData></worksheet>");
            return sb.ToString();
        }

        // Ширина столбцов по самой длинной ячейке (с разумными границами)
        private static List<double> AutoWidths(Sheet sheet)
        {
            int cols = sheet.Rows.Count == 0 ? 0 : sheet.Rows.Max(r => r.Count);
            var widths = new List<double>();
            for (int c = 0; c < cols; c++)
            {
                double max = 8;
                foreach (var row in sheet.Rows)
                {
                    if (c >= row.Count) continue;
                    int len = (row[c]?.ToString() ?? string.Empty).Length;
                    if (len + 2 > max) max = len + 2;
                }
                widths.Add(Math.Min(max, 60));
            }
            return widths;
        }

        private static string Col(int index)
        {
            index++;
            var s = string.Empty;
            while (index > 0)
            {
                int m = (index - 1) % 26;
                s = (char)('A' + m) + s;
                index = (index - 1) / 26;
            }
            return s;
        }

        private static string Esc(string s) =>
            new StringBuilder(s).Replace("&", "&amp;").Replace("<", "&lt;")
                .Replace(">", "&gt;").Replace("\"", "&quot;").ToString();
    }
}
