using System;

namespace iBank.Operator.Desktop.Data
{
    [Flags]
    public enum ErrorEnum
    {
        NONE = 0,

        LastName = 1,
        FirstName = 2,
        Patronymic = 4,
        BirthDate = 8,
        BirthPlace = 16,

        PassportSerial = 32,
        PassportDate = 64,
        PassportPlace = 128,
        PassportPlaceCode = 256,
        Registration = 512,

        PhoneHome = 1024,
        PhoneMobile = 2048,

        CodePhrase = 4096, 
        CardNumber = 8192
    }
}