using iBank.Core.Database;
using iBank.Core.Files;
using iBank.Core.Utils;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace iBank.Desktop.Database
{
    public class BankProviderPerson : INotifyPropertyChanged, IPersonData
    {
        public static IEnumerable<BankProviderPerson> GetExecutedByDate(DateTime date)
        {
            using (var client = new ConfigJsonFile().GetBankProviderClient())
                return JsonConvert.DeserializeObject<List<BankProviderPerson>>(client.GetExecutedByDate(date));
        }

        public static IEnumerable<BankProviderPerson> GetAll()
        {
            using (var client = new ConfigJsonFile().GetBankProviderClient())
                return JsonConvert.DeserializeObject<List<BankProviderPerson>>(client.GetAll());
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public DateTime BirthDateDate => CommonUtils.GetDateTime(BirthDate);
        public DateTime PassportIssueDateDate => CommonUtils.GetDateTime(PassportIssueDate);

        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string Patronymic { get; set; } = "";
        public string BirthDate { get; set; } = "";
        public string PassportSerial { get; set; } = "";
        public string PassportIssue { get; set; } = "";
        public string PassportIssueDate { get; set; } = "";
        public string PassportDivisionCode { get; set; } = "";
        public string PhoneHome { get; set; } = "";
        public string Codeword { get; set; } = "";
       //public DateTime DateImport { get; set; } = DateTime.MinValue;
        public string PhoneMobile { get; set; } = "";
        public string Address { get; set; } = "";
        public string RecruitmentOfficeID { get; set; } = "";
        public string AccountNumber { get; set; } = "";
        public string BirthPlace { get; set; } = "";
        public bool? CardGiven { get; set; } = false;
        //public DateTime DateCardGiven { get; set; } = DateTime.MinValue;
        //public DateTime DatePrint { get; set; } = DateTime.MinValue;
    }
}