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
            string target = System.IO.Path.Combine(new string[] { LeftPanel.GetCurrentDir(), file.Title });
            using (var manager = new CopierManager(
                LeftPanel, file, LeftPanel.GetNewFile(file.Title, target)))
            {
                await manager.CopyAsync();
                await refreshListing(LeftPanel);
            }
        }

        public async Task CopyToRigth(File file)
        {
            //This is workaround for existing files, it only affects windows phone sotrage
            string title = file.Title;
            while(RightPanel.FileCollection.FirstOrDefault(f => f.Title == title) != null)
            {
                title += "_";
            }

            string target = System.IO.Path.Combine(new string[] { RightPanel.GetCurrentDir(), title });
            using (var manager = new CopierManager(
                RightPanel, file, RightPanel.GetNewFile(title, target)))
            {
                await manager.CopyAsync();
                await refreshListing(RightPanel);
            }
        }

        public async Task ChangeLeftDir(File file = null)
        {
            if(null != file)
                LeftPanel.ChangeDirectory(file);
            await refreshListing(LeftPanel);
        }

        public async Task ChangeRightDir(File file = null)
        {
            if (null != file)
                RightPanel.ChangeDirectory(file);
            await refreshListing(RightPanel);
        }

        private async Task refreshListing(IDrive panelModel)
        {
            panelModel.FileCollection.Clear();

            var listing = await Task.FromResult(panelModel.GetListing());  //it will still block current thead which is UI thread
            var sortedListing = from i in listing
                                orderby i.IsDirectory descending, i.Title ascending
                                select i;

            foreach (var item in sortedListing)
                panelModel.FileCollection.Add(item);
        }

    }
}
