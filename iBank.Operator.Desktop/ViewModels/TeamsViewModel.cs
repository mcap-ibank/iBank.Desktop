using iBank.Operator.Desktop.Database;
using iBank.Operator.Desktop.Files;
using iBank.Operator.Desktop.Data;
using iBank.Operator.Desktop.DragDrop;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Exceptions;

namespace iBank.Operator.Desktop.ViewModels
{
    public class TeamViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<PersonsWithCardList> PersonToAssignTeamList { get; } = new ObservableCollection<PersonsWithCardList>();
        public PersonsWithCardList SelectedPersonToAssignTeam { get; set; }

        public ObservableCollection<TeamEntry> TeamList { get; } = new ObservableCollection<TeamEntry>();
        public TeamEntry SelectedTeam { get; set; }

        public ObservableCollection<PersonFromTeam> PersonFromTeamList { get; } = new ObservableCollection<PersonFromTeam>();
        public PersonFromTeam SelectedPersonFromTeam { get; set; }

        public string AssignedCardCount { get; set; } = "Люди с присвоенными картами: 0";
        public string AssignedCardTodayCount { get; set; } = "Присвоено карт за сегодня: 0";
        public string AssignedCardTodayWithoutTeamCount { get; set; } = "Не отправенных в команду: 0";

        public string CreateTeamOutgoing { get; set; } = string.Empty;
        public string CreateTeamTeam { get; set; } = string.Empty;

        public Item1DropTarget Item1DropTarget { get; set; }

        public SqlTableDependency<PersonTeamMetadata>? Dependency { get; set; }

        private readonly object _lock = new object();

        public TeamViewModel()
        {
            Item1DropTarget = new Item1DropTarget(this);
            BindingOperations.EnableCollectionSynchronization(TeamList, _lock);

#if KEEPSQLOPEN
            ConnectionManager.Connection.StateChange += (s, e) =>
            {
                if (e.CurrentState == System.Data.ConnectionState.Open)
                {
                    var mapper = new ModelToTableMapper<PersonTeamMetadata>();
                    mapper.AddMapping(c => c.PassportSerial, "passport_serial");
                    mapper.AddMapping(c => c.AccountNumberRegisteredInMilitaryID, "account_number_registered_in_military_id");
                    Dependency = new SqlTableDependency<PersonTeamMetadata>(ConnectionManager.Connection.ConnectionString, "person_team_metadata", mapper: mapper);
                    Dependency.OnChanged += Dep_PersonFromTeamList_OnChanged;
                    Dependency.OnChanged += UpdateTeamList;
                }

                if (e.CurrentState == System.Data.ConnectionState.Closed)
                {
                    Dependency.OnChanged -= Dep_PersonFromTeamList_OnChanged;
                    Dependency.OnChanged -= UpdateTeamList;
                    Dependency.Stop();
                    Dependency.Dispose();
                }
            };
#else
            // Асинхронно подписываемся
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var mapper = new ModelToTableMapper<PersonTeamMetadata>();
                    mapper.AddMapping(c => c.PassportSerial, "passport_serial");
                    mapper.AddMapping(c => c.AccountNumberRegisteredInMilitaryID, "account_number_registered_in_military_id");
                    Dependency = new SqlTableDependency<PersonTeamMetadata>(new ConfigJsonFile().GetMainSQLConnectionString(), "person_team_metadata", mapper: mapper);
                    Dependency.OnChanged += Dep_PersonFromTeamList_OnChanged;
                    Dependency.OnChanged += UpdateTeamList;
                }
                catch (ImpossibleOpenSqlConnectionException) { }
            });
#endif
        }

        public void Dep_PersonFromTeamList_OnChanged(object sender, RecordChangedEventArgs<PersonTeamMetadata> e)
        {
            if (e.ChangeType == ChangeType.Insert || e.ChangeType == ChangeType.Update)
            {
                var person = PersonFromTeamList.FirstOrDefault(p => p.PassportSerial == e.Entity.PassportSerial);
                if (person != null)
                    person.IsSelected = e.Entity.AccountNumberRegisteredInMilitaryID == 1;
            }
        }

        public void UpdateTeamList(object sender, EventArgs e)
        {
            TeamList.Clear();
            using (var sqlcmd = new SqlCommandExecutor("EXEC pr_GetTeams"))
            using (var sqlReader = sqlcmd.ExecuteReader())
            {
                while (sqlReader.Read())
                {
                    TeamList.Add(new TeamEntry
                    {
                        Inventory = sqlReader.GetInt16(0).ToString(),
                        Team = sqlReader.GetInt16(1).ToString(),
                        PeopleCount = sqlReader.GetInt16(2).ToString(),
                        IsComplete = sqlReader.GetBoolean(3)
                    });
                }
            }
        }
    }
}
