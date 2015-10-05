using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GDD
{
    class CommanderViewModel : INotifyPropertyChanged
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


        private IDrive leftPanel = new GDriveDirectory();
        private IDrive rightPanel = new WPDirectory();

        public IDrive GetLeftPanel()
        {
            return leftPanel;
        }

        public IDrive GetRightPanel()
        {
            return rightPanel;
        }

        public CommanderViewModel()
        {
            LeftPanelCollection = new ObservableCollection<File>();
            RightPanelCollection = new ObservableCollection<File>();

            LeftPanelName = leftPanel.Name;
            IsLeftInteractive = leftPanel.IsInteractive;
            RightPanelName = rightPanel.Name;
            IsRightInteractive = rightPanel.IsInteractive;
        }

        public async Task CopyToLeft(File file)
        {
            string target = System.IO.Path.Combine(new string[] { leftPanel.GetCurrentDir(), file.Title });
            using (var manager = new CopierManager(
                leftPanel, file, leftPanel.GetNewFile(file.Title, target)))
            {
                await manager.CopyAsync();
                await refreshListing(LeftPanelCollection, leftPanel);
            }
        }

        public async Task CopyToRigth(File file)
        {
            string target = System.IO.Path.Combine(new string[] { rightPanel.GetCurrentDir(), file.Title });
            using (var manager = new CopierManager(
                rightPanel, file, rightPanel.GetNewFile(file.Title, target)))
            {
                await manager.CopyAsync();
                await refreshListing(RightPanelCollection, rightPanel);
            }
        }

        public async Task ChangeLeftDir(File file = null)
        {
            if(null != file)
                leftPanel.ChangeDirectory(file);
            await refreshListing(LeftPanelCollection, leftPanel);
        }

        public async Task ChangeRightDir(File file = null)
        {
            if (null != file)
                rightPanel.ChangeDirectory(file);
            await refreshListing(RightPanelCollection, rightPanel);
        }

        private async Task refreshListing(ObservableCollection<File> panelListing, IDrive panelModel)
        {
            panelListing.Clear();

            foreach (var item in panelModel.GetListing())
                panelListing.Add(item);
        }

    }
}
