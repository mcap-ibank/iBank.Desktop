namespace iBank.Desktop.Data
{
    public class Formirovanie
    {
        public string ID { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DogTag { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Station { get; set; } = string.Empty;
        public string DateDispatch { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string MilitaryIDSerial { get; set; } = string.Empty;
        public string MilitaryIDNumber { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;
    }

    public class FullFormirovanie : Formirovanie
    {
        public string BirthPlace { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string PassportSerial { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public string PassportIssueDate { get; set; } = string.Empty;
        public string PassportIssue { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string BankBIK { get; set; } = string.Empty;
        public string DispatchPlace { get; set; } = string.Empty;
        public string Rank { get; set; } = string.Empty;
        public string DispatchPlaceTo { get; set; } = string.Empty;
    }
}