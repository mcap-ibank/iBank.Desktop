using ExcelDataReader;

using iBank.Core.Extensions;
using iBank.Operator.Desktop.Excel;

using OfficeOpenXml;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace iBank.Operator.Desktop.Utils
{
    public static class ExcelUtils
    {
        public static bool OpenWithoutLocking(string path, out byte[] data)
        {
            /*
            var fileName = Path.GetFileName(path);
            var checkFileName = fileName.Insert(0, "~$");
            var checkPath = path.Replace(fileName, checkFileName);
            if (!File.Exists(checkPath))
            {
                data = new byte[0];
                return false;
            }
            */

            var tempPath = Path.GetTempFileName();
            using (var inputFile = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var outputFile = new FileStream(tempPath, FileMode.Create))
            {
                var buffer = new byte[0x10000];
                int bytes;
                while ((bytes = inputFile.Read(buffer, 0, buffer.Length)) > 0)
                    outputFile.Write(buffer, 0, bytes);
            }

            using (var fs = new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                data = fs.ReadFully();
                return true;
            }
        }

        public static IEnumerable<ExcelPerson> LoadInputForm(string path, byte[]? data = null)
        {


            switch (Path.GetExtension(path))
            {
                case ".xls":
                case ".xlm":
                case ".csv":
                    if (data != null)
                        return LoadInputFormFallback(data);
                    else
                        return OpenWithoutLocking(path, out data) ? LoadInputFormFallback(data) : new List<ExcelPerson>();
                case ".xlsx":
                case ".xlsm":
                    if (data != null)
                        return LoadInputForm(data);
                    else
                        return OpenWithoutLocking(path, out data) ? LoadInputForm(data) : new List<ExcelPerson>();
                default:
                    return new List<ExcelPerson>();
            }
        }


        private static IEnumerable<ExcelPerson> LoadInputForm(byte[] data)
        {
            using (var excel = new ExcelPackage(new MemoryStream(data)))
            using (var workbook = excel.Workbook)
            using (var worksheet = workbook.Worksheets.First())
            {
                for (int i = 4; ; i++)
                {
                    // Пидоры те военкомы, что делают на похуй эти айдишники
                    if (worksheet.Cells[i, 1].Value == null || worksheet.Cells[i, 2].Value == null)
                        yield break;

                    yield return new ExcelPerson
                    {
                        LastName = worksheet.Cells[i, 02].Value.ToString().Trim(),
                        FirstName = worksheet.Cells[i, 03].Value.ToString().Trim(),
                        Patronymic = worksheet.Cells[i, 04].Value.ToString().Equals(" ") ? " " : worksheet.Cells[i, 04].Value.ToString().Trim(),
                        BirthDate = iBank.Core.Utils.GetDateTime(worksheet.Cells[i, 05].Value.ToString().Trim()),
                        BirthPlace = worksheet.Cells[i, 06].Value.ToString().Replace(Environment.NewLine, " ").Trim(),
                        PassportSerial = worksheet.Cells[i, 07].Value.ToString().Trim(),
                        PassportIssue = worksheet.Cells[i, 08].Value.ToString().Trim(),
                        PassportIssueDate = iBank.Core.Utils.GetDateTime(worksheet.Cells[i, 09].Value.ToString().Trim()),
                        PassportDivisionCode = worksheet.Cells[i, 10].Value.ToString().Trim(),
                        Address = worksheet.Cells[i, 11].Value.ToString().Replace(Environment.NewLine, " ").Trim(),
                        PhoneHome = worksheet.Cells[i, 13].Value.ToString().Trim(),
                        PhoneMobile = worksheet.Cells[i, 14].Value.ToString().Trim(),
                        Codeword = worksheet.Cells[i, 15].Value.ToString().Trim(),
                        RecruitmentOfficeID = int.TryParse(worksheet.Cells[i, 16].Value.ToString().Trim(), out var recruitmentOfficeID) ? recruitmentOfficeID : -1,
                        Comment = worksheet.Cells[i, 17].Value?.ToString()?.Trim() ?? string.Empty
                    };
                }
            }
        }
        private static IEnumerable<ExcelPerson> LoadInputFormFallback(byte[] data)
        {
            using (var reader = ExcelReaderFactory.CreateReader(new MemoryStream(data)))
            {
                reader.Read();
                reader.Read();
                reader.Read();

                // Пидоры те военкомы, что делают на похуй эти айдишники
                while (reader.Read() && !reader.IsDBNull(0) && !reader.IsDBNull(1))
                {
                    yield return new ExcelPerson
                    {
                        LastName = reader.GetValue(01).ToString().Trim(),
                        FirstName = reader.GetValue(02).ToString().Trim(),
                        Patronymic = reader.GetValue(03).ToString().Trim().Equals(" ") ? " " : reader.GetValue(03).ToString().Trim(),
                        BirthDate = iBank.Core.Utils.GetDateTime(reader.GetValue(04).ToString().Trim()),
                        BirthPlace = reader.GetValue(05).ToString().Replace(Environment.NewLine, " ").Trim(),
                        PassportSerial = reader.GetValue(06).ToString().Trim(),
                        PassportIssue = reader.GetValue(07).ToString().Trim(),
                        PassportIssueDate = iBank.Core.Utils.GetDateTime(reader.GetValue(08).ToString().Trim()),
                        PassportDivisionCode = reader.GetValue(09).ToString().Trim(),
                        Address = reader.GetValue(10).ToString().Replace(Environment.NewLine, " ").Trim(),
                        PhoneHome = reader.GetValue(12).ToString().Trim(),
                        PhoneMobile = reader.GetValue(13).ToString().Trim(),
                        Codeword = reader.GetValue(14).ToString().Trim(),
                        RecruitmentOfficeID = int.TryParse(reader.GetValue(15).ToString().Trim(), out var recruitmentOfficeID) ? recruitmentOfficeID : -1,
                        Comment = reader.FieldCount > 15 ? string.Empty : reader.GetValue(16).ToString().Trim()
                    };
                }
            }
        }
    }
}