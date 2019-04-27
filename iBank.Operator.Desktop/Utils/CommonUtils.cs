using iBank.Core.Database;
using iBank.Operator.Desktop.Data;

using System;
using System.Globalization;
using System.Windows;

namespace iBank.Operator.Desktop.Utils
{
    public static class CommonUtils
    {
        public static void ShowException(Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ошибка!");
            //MessageBox.Show(ex.Message, "Ошибка!");
        }

        public static bool GetWarnings(IPersonData data, IPersonPassportData passport, out ErrorEnum @enum)
        {
            @enum = ErrorEnum.NONE;

            if (data.LastName.Length > 32)
                @enum |= ErrorEnum.LastName;
            if (data.FirstName.Length > 32)
                @enum |= ErrorEnum.FirstName;
            if (data.Patronymic.Length > 32)
                @enum |= ErrorEnum.Patronymic;

            if (data.BirthPlace.Length > 32)
                @enum |= ErrorEnum.BirthPlace;

            if (passport.PassportIssue.Length > 29)
                @enum |= ErrorEnum.PassportPlace;

            if (passport.Address.Length > 96)
                @enum |= ErrorEnum.Registration;

            return @enum != ErrorEnum.NONE;
        }

        public static bool GetErrors(IPersonData data, IPersonPassportData passport, out ErrorEnum @enum)
        {
            @enum = ErrorEnum.NONE;

            if (DateTime.TryParseExact(data.BirthDate.Replace(".", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var bDay))
            {
                // Уже стукнуло 27
                if (DateTime.Now > bDay.AddYears(27))
                    @enum |= ErrorEnum.BirthDate;

                // Еще не стукнуло 18
                if (DateTime.Now < bDay.AddYears(18))
                    @enum |= ErrorEnum.BirthDate;
            }
            else
                @enum |= ErrorEnum.BirthDate;

            if (data.PassportSerial.Length != 12) // 10?
                @enum |= ErrorEnum.PassportSerial;
            if (DateTime.TryParseExact(passport.PassportIssueDate.Replace(".", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var pDay))
            {
                // Больше 20 лет, но паспорт не обновлен
                if (bDay.AddYears(20) < DateTime.Now && bDay.AddYears(20) > pDay)
                    @enum |= ErrorEnum.BirthDate | ErrorEnum.PassportDate;
            }
            else
                @enum |= ErrorEnum.BirthDate;

            if (passport.PassportDivisionCode.Length != 7)
                @enum |= ErrorEnum.PassportPlaceCode;
            if (passport.PassportDivisionCode.Length == 7 && passport.PassportDivisionCode[3] != '-')
                @enum |= ErrorEnum.PassportPlaceCode;
            if (passport.PassportDivisionCode.Length == 7 &&
                (!char.IsDigit(passport.PassportDivisionCode[0]) || !char.IsDigit(passport.PassportDivisionCode[1]) || !char.IsDigit(passport.PassportDivisionCode[2]) ||
                 !char.IsDigit(passport.PassportDivisionCode[4]) || !char.IsDigit(passport.PassportDivisionCode[5]) || !char.IsDigit(passport.PassportDivisionCode[6])))
                @enum |= ErrorEnum.PassportPlaceCode;

            if (passport.PhoneHome.Length != 10)
                @enum |= ErrorEnum.PhoneHome;
            if (passport.PhoneMobile.Length != 10)
                @enum |= ErrorEnum.PhoneMobile;

            if (Equals(bDay.Date, pDay.Date))
                @enum |= ErrorEnum.BirthDate | ErrorEnum.PassportDate;


            return @enum != ErrorEnum.NONE;
        }
    }
}