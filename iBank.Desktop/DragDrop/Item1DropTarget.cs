using System.Collections;
using System.Windows;

using GongSolutions.Wpf.DragDrop;

using iBank.Core;
using iBank.Core.Database;

using iBank.Desktop.Extensions;
using iBank.Desktop.ViewModel;

namespace iBank.Desktop.DragDrop
{
    public class Item1DropTarget : IDropTarget
    {
        private TeamViewModel TeamsViewModel { get; }

        public Item1DropTarget(TeamViewModel teamsViewModel)
        {
            TeamsViewModel = teamsViewModel;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is PersonsWithCardList && dropInfo.TargetCollection is IList)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (TeamsViewModel.SelectedTeam == null)
                return;
            if (dropInfo.Data is PersonsWithCardList sourceItem && dropInfo.TargetCollection is IList targetCollection)
            {
                var sql = $"EXEC pr_AssignTeam \'{sourceItem.PassportSerial}\', \'{TeamsViewModel.SelectedTeam.Inventory}\' ";
                using (var sqlcmd = new SqlCommandExecutor(sql))
                {
                    if (sqlcmd.TryExecuteNonQuery(out _) && dropInfo.DragInfo.SourceCollection is IList sourceCollection)
                    {
                        targetCollection.Add(new PersonFromTeam(sourceItem));
                        sourceCollection.Remove(sourceItem);
                    }
                }
            }
        }
    }
}