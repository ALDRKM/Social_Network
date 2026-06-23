using System.IO.Compression;
using System.Text;

namespace Social_Network.Helpers
{
    // Генерация настоящего .xlsx без сторонних библиотек (OpenXML-минимум).
    // Первая строка каждого листа — жирный заголовок; задаются ширины колонок.
    public static class ExcelExporter
    {
        public sealed class Sheet
        {
            public string Name { get; set; } = "Лист1";
            public List<List<object?>> Rows { get; } = new();
            public List<double> ColumnWidths { get; } = new();
            public void Add(params object?[] cells) => Rows.Add(cells.ToList());
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

        // Стили: индекс 1 — жирный (для заголовков)
        private static string Styles() =>
            "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
            "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">" +
            "<fonts count=\"2\"><font><sz val=\"11\"/><name val=\"Calibri\"/></font>" +
            "<font><b/><sz val=\"11\"/><name val=\"Calibri\"/></font></fonts>" +
            "<fills count=\"1\"><fill><patternFill patternType=\"none\"/></fill></fills>" +
            "<borders count=\"1\"><border/></borders>" +
            "<cellStyleXfs count=\"1\"><xf/></cellStyleXfs>" +
            "<cellXfs count=\"2\"><xf/><xf fontId=\"1\" applyFont=\"1\"/></cellXfs>" +
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

            if (sheet.ColumnWidths.Count > 0)
            {
                sb.Append("<cols>");
                for (int c = 0; c < sheet.ColumnWidths.Count; c++)
                    sb.Append($"<col min=\"{c + 1}\" max=\"{c + 1}\" width=\"{sheet.ColumnWidths[c].ToString(System.Globalization.CultureInfo.InvariantCulture)}\" customWidth=\"1\"/>");
                sb.Append("</cols>");
            }

            sb.Append("<sheetData>");
            for (int r = 0; r < sheet.Rows.Count; r++)
            {
                sb.Append($"<row r=\"{r + 1}\">");
                var row = sheet.Rows[r];
                string style = r == 0 ? " s=\"1\"" : string.Empty; // первая строка — жирная
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
