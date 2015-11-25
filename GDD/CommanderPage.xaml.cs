using GDD.Common;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;

namespace GDD
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommanderPage : Page
    {
        private NavigationHelper navigationHelper;

        private CommanderViewModel vm;

        public CommanderPage()
        {
            vm = App.CommanderVM;
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }


        private async void LeftPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (e.AddedItems.Count == 0)
                return;

            if(listView.SelectionMode == ListViewSelectionMode.Single)
            {
                using (ProgressManager pm = new ProgressManager(MainContent, ProgressLayer, ProgressRingHandler))
                {
                    foreach (File file in e.AddedItems)
                    {
                        if (file.IsDirectory)
                        {
                            await vm.ChangeLeftDir(file);
                        }
                        else
                        {
                            await vm.CopyToRigth(file);
                        }
                    }
                }
            }
            else if (listView.SelectionMode == ListViewSelectionMode.Multiple)
            {
                foreach (File file in e.AddedItems)
                {
                    if (file.IsDirectory)
                    {
                        listView.SelectedItems.Remove(file);
                    }
                }
            }
        }

        private async void RightPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (e.AddedItems.Count == 0)
                return;

            if (listView.SelectionMode == ListViewSelectionMode.Single)
            {
                using (ProgressManager pm = new ProgressManager(MainContent, ProgressLayer, ProgressRingHandler))
                {
                    foreach (File file in e.AddedItems)
                    {
                        if (file.IsDirectory)
                        {
                            await vm.ChangeRightDir(file);
                        }
                        else
                        {
                            await vm.CopyToLeft(file);
                        }
                    }
                }
            }
            else if (listView.SelectionMode == ListViewSelectionMode.Multiple)
            {
                foreach (File file in e.AddedItems)
                {
                    if (file.IsDirectory)
                    {
                        listView.SelectedItems.Remove(file);
                    }
                }
            }
        }

        private void MainContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = GetListView();
            if (listView == null)
                return;

            if(listView.SelectionMode == ListViewSelectionMode.Multiple)
            {
                AppBarButton_Copy.IsEnabled = true;
            }
            else
            {
                AppBarButton_Copy.IsEnabled = false;
            }
        }


        #region NavigationHelper registration
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            vm = App.CommanderVM;

            DataContext = null;
            LeftFlipView.DataContext = null;
            RightFlipView.DataContext = null;

            DataContext = vm;
            LeftFlipView.DataContext = vm.LeftPanel;
            RightFlipView.DataContext = vm.RightPanel;

            MainContent.SelectedIndex = vm.CurrentActivePanel;

            await vm.ChangeLeftDir();
            await vm.ChangeRightDir();
        }
        
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            vm.CurrentActivePanel = MainContent.SelectedIndex;
            App.CommanderVM = vm;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(e);

            foreach(var page in Frame.BackStack)
            {
                if(page.SourcePageType == typeof(MainPage))
                {
                    Frame.BackStack.Remove(page);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region Application Bar 
        private void AppBarListButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ListView listView = GetListView();
            if(listView.SelectionMode == ListViewSelectionMode.Single)
            {
                listView.SelectionMode = ListViewSelectionMode.Multiple;
                AppBarButton_Copy.IsEnabled = true;
            }
            else
            {
                listView.SelectedItems.Clear();
                listView.SelectionMode = ListViewSelectionMode.Single;
                AppBarButton_Copy.IsEnabled = false;
            }
        }

        private async void AppBarCopyButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int idx = MainContent.SelectedIndex; // 0-left, 1-right

            using (ProgressManager pm = new ProgressManager(MainContent, ProgressLayer, ProgressRingHandler))
            {
                switch (idx)
                {
                    case 0:
                        foreach (File item in LeftPanel.SelectedItems)
                        {
                            await vm.CopyToRigth(item);
                        }
                        break;
                    case 1:
                        foreach (File item in RightPanel.SelectedItems)
                        {
                            await vm.CopyToLeft(item);
                        }
                        break;
                }

                LeftPanel.SelectedItems.Clear();
                RightPanel.SelectedItems.Clear();
                LeftPanel.SelectionMode = ListViewSelectionMode.Single;
                RightPanel.SelectionMode = ListViewSelectionMode.Single;
                AppBarButton_Copy.IsEnabled = false;
            }
        }

        private async void AppBarRefreshButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int idx = MainContent.SelectedIndex; // 0-left, 1-right
            using (ProgressManager pm = new ProgressManager(MainContent, ProgressLayer, ProgressRingHandler))
            {
                switch (idx)
                {
                    case 0:
                        await vm.ChangeLeftDir();
                        break;
                    case 1:
                        await vm.ChangeRightDir();
                        break;
                }
            }
        }

        private async void AppBarFolerInfoButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        { 
            
            await new Windows.UI.Popups.MessageDialog(GetDrive().GetCurrentDir()).ShowAsync();
        }

        private IDrive GetDrive()
        {
            if (MainContent == null)
                return null;

            switch (MainContent.SelectedIndex)
            {
                case 0: return vm.LeftPanel;
                case 1: return vm.RightPanel;
                default: return null;
            }
        }

        private ListView GetListView()
        {
            if (MainContent == null)
                return null;

            switch (MainContent.SelectedIndex)
            {
                case 0: return LeftPanel;
                case 1: return RightPanel;
                default: return null;
            }
        }
        #endregion

        private void InteractiveImage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            string name = (sender as Image).Name;
            IDrive param = null;
            if(name.CompareTo("LeftInteractiveImage") == 0)
            {
                param = vm.LeftPanel;
            }
            else if(name.CompareTo("RightInteractiveImage") == 0)
            {
                param = vm.RightPanel;
            }

            Frame.Navigate(typeof(ChooseStorage), param);
        }
    }
}
