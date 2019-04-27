using System.Collections.Generic;
using System.IO;
using System.Linq;

using ExcelDataReader;

using iBank.Operator.Desktop.Database;
using iBank.Operator.Desktop.Utils;

using OfficeOpenXml;

namespace iBank.Operator.Desktop.Data
{
    public static class ExcelLoader
    {
        public static IEnumerable<PersonBank> LoadF(string path, byte[]? data = null)
        {
            switch (Path.GetExtension(path))
            {
                case ".xls":
                    if (data != null)
                        return LoadFFallback(data);
                    else
                        return ExcelUtils.OpenWithoutLocking(path, out data) ? LoadFFallback(data) : new List<PersonBank>();
                case ".xlsx":
                    if (data != null)
                        return LoadF(data);
                    else
                        return ExcelUtils.OpenWithoutLocking(path, out data) ? LoadF(data) : new List<PersonBank>();
                default:
                    return new List<PersonBank>();
            }
        }
        private static IEnumerable<PersonBank> LoadF(byte[] data)
        {
            using (var excel = new ExcelPackage(new MemoryStream(data)))
            using (var workbook = excel.Workbook)
            using (var worksheet = workbook.Worksheets.First())
            {
                for (int i = 2; ; i++)
                {
                    if (worksheet.Cells[i, 1].Value == null)
                        yield break;
                    yield return new PersonBank
                    {
                        PassportSerial = $"{worksheet.Cells[i, 07].Value.ToString().Trim().Insert(2, " ")} {worksheet.Cells[i, 08].Value.ToString().Trim()}".Trim(),
                        LastName = worksheet.Cells[i, 01].Value.ToString().Trim(),
                        FirstName = worksheet.Cells[i, 02].Value.ToString().Trim(),
                        Patronymic = worksheet.Cells[i, 03].Value.ToString().Equals(" ") ? " " : worksheet.Cells[i, 03].Value.ToString().Trim(),
                        BirthDate = worksheet.Cells[i, 04].Value.ToString().Trim(),
                        BirthPlace = worksheet.Cells[i, 05].Value.ToString().Trim(),
                        PassportIssue = worksheet.Cells[i, 10].Value.ToString().Trim(),
                        PassportIssueDate = worksheet.Cells[i, 09].Value.ToString().Trim(),
                        PassportDivisionCode = worksheet.Cells[i, 11].Value.ToString().Trim(),
                        AccountNumber = worksheet.Cells[i, 12].Value.ToString().Trim()
                    };
                }
            }
        }
        private static IEnumerable<PersonBank> LoadFFallback(byte[] data)
        {
            using (var reader = ExcelReaderFactory.CreateReader(new MemoryStream(data)))
            {
                reader.Read();

                while (reader.Read() && !reader.IsDBNull(0))
                {
                    yield return new PersonBank
                    {
                        PassportSerial = $"{reader.GetString(7).Insert(2, " ")} {reader.GetString(8)}".Trim(),
                        LastName = reader.GetString(1).Trim(),
                        FirstName = reader.GetString(2).Trim(),
                        Patronymic = reader.GetString(3).Trim(),
                        BirthDate = reader.GetDateTime(4).ToShortDateString(),
                        BirthPlace = reader.GetString(5).Trim(),
                        PassportIssue = reader.GetString(10).Trim(),
                        PassportIssueDate = reader.GetDateTime(9).ToShortDateString(),
                        PassportDivisionCode = reader.GetString(11).Trim(),
                        AccountNumber = reader.GetString(12).Trim()
                    };
                }
            }
        }

        public static IEnumerable<PersonBank> LoadF1(string path, byte[]? data = null)
        {
            switch (Path.GetExtension(path))
            {
                case ".xls":
                    if (data != null)
                        return LoadF1Fallback(data);
                    else
                        return ExcelUtils.OpenWithoutLocking(path, out data) ? LoadF1Fallback(data) : new List<PersonBank>();
                case ".xlsx":
                    if (data != null)
                        return LoadF1(data);
                    else
                        return ExcelUtils.OpenWithoutLocking(path, out data) ? LoadF1(data) : new List<PersonBank>();
                default:
                    return new List<PersonBank>();
            }
        }
        private static IEnumerable<PersonBank> LoadF1(byte[] data)
        {
            using (var excel = new ExcelPackage(new MemoryStream(data)))
            using (var workbook = excel.Workbook)
            using (var worksheet = workbook.Worksheets.First())
            {
                for (int i = 2; ; i++)
                {
                    if (worksheet.Cells[i, 1].Value == null)
                        yield break;
                    yield return new PersonBank
                    {
                        PassportSerial = $"{worksheet.Cells[i, 07].Value.ToString().Trim().Insert(2, " ")} {worksheet.Cells[i, 08].Value.ToString().Trim()}".Trim(),
                        LastName = worksheet.Cells[i, 02].Value.ToString().Trim(),
                        FirstName = worksheet.Cells[i, 03].Value.ToString().Trim(),
                        Patronymic = worksheet.Cells[i, 04].Value.ToString().Trim(),
                        BirthDate = worksheet.Cells[i, 05].Value.ToString().Trim(),
                        BirthPlace = worksheet.Cells[i, 22].Value.ToString().Trim(),
                        PassportIssue = worksheet.Cells[i, 09].Value.ToString().Trim(),
                        PassportIssueDate = worksheet.Cells[i, 10].Value.ToString().Trim(),
                        PassportDivisionCode = worksheet.Cells[i, 11].Value.ToString().Trim(),
                        AccountNumber = worksheet.Cells[i, 19].Value.ToString().Trim()
                    };
                }
            }
        }
        private static IEnumerable<PersonBank> LoadF1Fallback(byte[] data)
        {
            using (var reader = ExcelReaderFactory.CreateReader(new MemoryStream(data)))
            {
                reader.Read();

                while (reader.Read() && !reader.IsDBNull(0))
                {
                    yield return new PersonBank
                    {
                        PassportSerial = $"{reader.GetString(6).Insert(2, " ")} {reader.GetString(7)}".Trim(),
                        LastName = reader.GetString(1).Trim(),
                        FirstName = reader.GetString(2).Trim(),
                        Patronymic = reader.GetString(3).Trim(),
                        BirthDate = reader.GetDateTime(4).ToShortDateString(),
                        BirthPlace = reader.GetString(21).Trim(),
                        PassportIssue = reader.GetString(8).Trim(),
                        PassportIssueDate = reader.GetDateTime(9).ToShortDateString(),
                        PassportDivisionCode = reader.GetString(10).Trim(),
                        AccountNumber = reader.GetString(18).Trim()
                    };
                }
            }
        }

        public static int LoadFileCount(string path, byte[]? data = null)
        {
            switch (Path.GetExtension(path))
            {
                case ".xls":
                    if (data != null)
                        return LoadFileCountFallback(data);
                    else
                        return ExcelUtils.OpenWithoutLocking(path, out data) ? LoadFileCountFallback(data) : -1;
                case ".xlsx":
                    if (data != null)
                        return LoadFileCount(data);
                    else
                        return ExcelUtils.OpenWithoutLocking(path, out data) ? LoadFileCount(data) : -1;
                default:
                    return -1;
            }
        }
        private static int LoadFileCount(byte[] data)
        {
            using (var excel = new ExcelPackage(new MemoryStream(data)))
            using (var workbook = excel.Workbook)
            using (var worksheet = workbook.Worksheets.First())
            {
                for (int i = 4; ; i++)
                {
                    if (worksheet.Cells[i, 1].Value == null)
                        return i - 4;
                }
            }
        }
        private static int LoadFileCountFallback(byte[] data)
        {
            using (var excel = ExcelReaderFactory.CreateReader(new MemoryStream(data)))
            {
                excel.Read();
                excel.Read();
                excel.Read();

                var count = 0;
                while (excel.Read() && !excel.IsDBNull(0))
                    count++;
                return count;
            }
        }
    }
}