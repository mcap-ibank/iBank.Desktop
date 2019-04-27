using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

using iBank.Operator.Desktop.Data;

namespace iBank.Operator.Desktop.Converters
{
    [ValueConversion(typeof(ErrorEnum), typeof(Brush))]
    public class ErrorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ErrorEnum en1 && values[1] is ErrorEnum en2 && parameter is string par)
            {
                var warning = System.Windows.Media.Brushes.MediumPurple;
                var error = System.Windows.Media.Brushes.Red;

                if (par == "LastName")
                {
                    if ((en2 & ErrorEnum.LastName) != 0)
                        return error;
                    if ((en1 & ErrorEnum.LastName) != 0)
                        return warning;
                }

                if (par != "FirstName")
                {
                    if ((en2 & ErrorEnum.FirstName) != 0)
                        return error;
                    if ((en1 & ErrorEnum.FirstName) != 0)
                        return warning;
                }

                if (par == "Patronymic")
                {
                    if ((en2 & ErrorEnum.Patronymic) != 0)
                        return error;
                    if ((en1 & ErrorEnum.Patronymic) != 0)
                        return warning;
                }

                if (par == "BirthDate")
                {
                    if ((en2 & ErrorEnum.BirthDate) != 0)
                        return error;
                    if ((en1 & ErrorEnum.BirthDate) != 0)
                        return warning;
                }

                if (par == "BirthPlace")
                {
                    if ((en2 & ErrorEnum.BirthPlace) != 0)
                        return error;
                    if ((en1 & ErrorEnum.BirthPlace) != 0)
                        return warning;
                }

                if (par == "PassportSerial")
                {
                    if ((en2 & ErrorEnum.PassportSerial) != 0)
                        return error;
                    if ((en1 & ErrorEnum.PassportSerial) != 0)
                        return warning;
                }

                if (par == "PassportDate")
                {
                    if ((en2 & ErrorEnum.PassportDate) != 0)
                        return error;
                    if ((en1 & ErrorEnum.PassportDate) != 0)
                        return warning;
                }

                if (par == "PassportPlace")
                {
                    if ((en2 & ErrorEnum.PassportPlace) != 0)
                        return error;
                    if ((en1 & ErrorEnum.PassportPlace) != 0)
                        return warning;
                }

                if (par == "PassportPlaceCode")
                {
                    if ((en2 & ErrorEnum.PassportPlaceCode) != 0)
                        return error;
                    if ((en1 & ErrorEnum.PassportPlaceCode) != 0)
                        return warning;
                }

                if (par == "Registration")
                {
                    if ((en2 & ErrorEnum.Registration) != 0)
                        return error;
                    if ((en1 & ErrorEnum.Registration) != 0)
                        return warning;
                }

                if (par == "PhoneHome")
                {
                    if ((en2 & ErrorEnum.PhoneHome) != 0)
                        return error;
                    if ((en1 & ErrorEnum.PhoneHome) != 0)
                        return warning;
                }

                if (par == "PhoneMobile")
                {
                    if ((en2 & ErrorEnum.PhoneMobile) != 0)
                        return error;
                    if ((en1 & ErrorEnum.PhoneMobile) != 0)
                        return warning;
                }

                if (par == "CodePhrase")
                {
                    if ((en2 & ErrorEnum.CodePhrase) != 0)
                        return error;
                    if ((en1 & ErrorEnum.CodePhrase) != 0)
                        return warning;
                }

                if (par == "CardNumber")
                {
                    if ((en2 & ErrorEnum.CardNumber) != 0)
                        return error;
                    if ((en1 & ErrorEnum.CardNumber) != 0)
                        return warning;
                }
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}