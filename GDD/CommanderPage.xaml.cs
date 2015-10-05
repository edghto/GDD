using GDD.Common;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
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
            vm = new CommanderViewModel();
            DataContext = vm;
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            LeftDriveName_TextBlock.DataContext = vm.GetLeftPanel();
        }


        private async void LeftPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (e.AddedItems.Count == 0)
                return;

            if(listView.SelectionMode == ListViewSelectionMode.Single)
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
        
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }
        
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(e);
            //Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            await vm.ChangeLeftDir();
            await vm.ChangeRightDir();
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
            switch(idx)
            {
                case 0:
                    foreach(File item in LeftPanel.SelectedItems)
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

        private async void AppBarRefreshButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int idx = MainContent.SelectedIndex; // 0-left, 1-right
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
                param = vm.GetLeftPanel();
            }
            else if(name.CompareTo("RightInteractiveImage") == 0)
            {
                param = vm.GetRightPanel();
            }

            Frame.Navigate(typeof(ChooseStorage), param);
        }
    }
}
