using System.ComponentModel;

namespace iBank.Operator.Desktop.Database
{
    public class PersonExtended : Person, INotifyPropertyChanged
    {
        public string RecruitmentOffice
        {
            get
            {
                var sql = $"SELECT name FROM recruitment_office_name WHERE id = {RecruitmentOfficeID}";
                using (var cmd = new SqlCommandExecutor(sql))
                    return cmd.ExecuteScalar().ToString();
            }
        }

        public PersonExtended(Person model)
        {
            PassportSerial = model.PassportSerial;
            LastName = model.LastName;
            FirstName = model.FirstName;
            Patronymic = model.Patronymic;
            BirthDate = model.BirthDate;
            BirthPlace = model.BirthPlace;
            PassportIssue = model.PassportIssue;
            PassportIssueDate = model.PassportIssueDate;
            PassportDivisionCode = model.PassportDivisionCode;
            Address = model.Address;
            PhoneHome = model.PhoneHome;
            PhoneMobile = model.PhoneMobile;
            RecruitmentOfficeID = model.RecruitmentOfficeID;
            Codeword = model.Codeword;
            Comment = model.Comment;
        }
    }
}