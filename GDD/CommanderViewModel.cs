using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GDD
{
    public class CommanderViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<File> LeftPanelCollection { get; set; }
        public ObservableCollection<File> RightPanelCollection { get; set; }

        private string _LeftPanelName;
        public string LeftPanelName
        {
            get { return _LeftPanelName; }
            set
            {
                _LeftPanelName = value;
                RaisePropertyChanged("LeftPanelName");
            }
        }

        private bool _IsLeftInteractive;
        public bool IsLeftInteractive
        {
            get { return _IsLeftInteractive; }
            set
            {
                _IsLeftInteractive = value;
                RaisePropertyChanged("IsLeftInteractive");
            }
        }

        private string _RightPanelName { get; set; }
        public string RightPanelName
        {
            get { return _RightPanelName; }
            set
            {
                _RightPanelName = value;
                RaisePropertyChanged("RightPanelName");
            }
        }

        private bool _IsRightInteractive;
        public bool IsRightInteractive
        {
            get { return _IsRightInteractive; }
            set
            {
                _IsRightInteractive = value;
                RaisePropertyChanged("IsRightInteractive");
            }
        }

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
            LeftPanelCollection = new ObservableCollection<File>();
            RightPanelCollection = new ObservableCollection<File>();

            LeftPanel = new GDriveDirectory();
            RightPanel = new WPDirectory();

            LeftPanelName = LeftPanel.Name;
            IsLeftInteractive = LeftPanel.IsInteractive;
            RightPanelName = RightPanel.Name;
            IsRightInteractive = RightPanel.IsInteractive;
        }

        public async Task CopyToLeft(File file)
        {
            string target = System.IO.Path.Combine(new string[] { LeftPanel.GetCurrentDir(), file.Title });
            using (var manager = new CopierManager(
                LeftPanel, file, LeftPanel.GetNewFile(file.Title, target)))
            {
                await manager.CopyAsync();
                await refreshListing(LeftPanelCollection, LeftPanel);
            }
        }

        public async Task CopyToRigth(File file)
        {
            string target = System.IO.Path.Combine(new string[] { RightPanel.GetCurrentDir(), file.Title });
            using (var manager = new CopierManager(
                RightPanel, file, RightPanel.GetNewFile(file.Title, target)))
            {
                await manager.CopyAsync();
                await refreshListing(RightPanelCollection, RightPanel);
            }
        }

        public async Task ChangeLeftDir(File file = null)
        {
            if(null != file)
                LeftPanel.ChangeDirectory(file);
            await refreshListing(LeftPanelCollection, LeftPanel);
        }

        public async Task ChangeRightDir(File file = null)
        {
            if (null != file)
                RightPanel.ChangeDirectory(file);
            await refreshListing(RightPanelCollection, RightPanel);
        }

        private async Task refreshListing(ObservableCollection<File> panelListing, IDrive panelModel)
        {
            panelListing.Clear();

            foreach (var item in panelModel.GetListing())
                panelListing.Add(item);
        }

    }
}
