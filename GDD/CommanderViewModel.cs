using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;

namespace GDD
{
    public class CommanderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int CurrentActivePanel { get; set; }

        public IDrive LeftPanel { get; set; }
        public IDrive RightPanel { get; set; }

        public CommanderViewModel()
        {
            LeftPanel = new GDriveDirectory();
            RightPanel = new WPDirectory();
        }

        public async Task CopyToLeft(File file)
        {
            //try {
                string target = System.IO.Path.Combine(new string[] { LeftPanel.GetCurrentDir().Id, file.Title });
                using (var manager = new CopierManager(
                    LeftPanel, file, LeftPanel.GetNewFile(file.Title, target)))
                {
                    await manager.CopyAsync();
                    await refreshListing(LeftPanel);
                }
            //}
            //catch(Exception e)
            //{
            //    await ShowAlert(e);
            //}
        }

        private async Task ShowAlert(Exception e)
        {
            var alert = new Windows.UI.Popups.MessageDialog(e.Message + "\n" + e.StackTrace.ToString());
            await alert.ShowAsync();
        }

        public async Task CopyToRigth(File file)
        {
            //try { 
                string title = file.Title;

                string target = System.IO.Path.Combine(new string[] { RightPanel.GetCurrentDir().Id, title });
                using (var manager = new CopierManager(
                    RightPanel, file, RightPanel.GetNewFile(title, target)))
                {
                    await manager.CopyAsync();
                    await refreshListing(RightPanel);
                }
            //}
            //catch (Exception e)
            //{
            //    await ShowAlert(e);
            //}
        }

        public async Task ChangeLeftDir(File file = null)
        {
            //try
            //{ 
                if(null != file)
                    LeftPanel.ChangeDirectory(file);
                await refreshListing(LeftPanel);
            //}
            //catch (Exception e)
            //{
            //    await ShowAlert(e);
            //}
        }

        public async Task ChangeRightDir(File file = null)
        {
            //try
            //{ 
                if (null != file)
                    RightPanel.ChangeDirectory(file);
                await refreshListing(RightPanel);
            //}
            //catch(Exception e)
            //{
            //    await ShowAlert(e);
            //}
        }

        private async Task refreshListing(IDrive panelModel)
        {
            //try
            //{ 
                panelModel.FileCollection.Clear();

                var listing = await panelModel.GetListingAsync();  //it will still block current thead which is UI thread
                var sortedListing = from i in listing
                                    orderby i.IsDirectory descending, i.Title ascending
                                    select i;

                foreach (var item in sortedListing)
                    panelModel.FileCollection.Add(item);
            //}
            //catch (Exception e)
            //{
            //    await ShowAlert(e);
            //}
        }

    }
}
