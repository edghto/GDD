using GDD.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace GDD
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChooseStorage : Page
    {
        private NavigationHelper navigationHelper;

        private IDrive directory;

        private CommanderViewModel vm;

        public ObservableCollection<Drive> StorageCollection { get; private set; }

        public ChooseStorage()
        {
            this.InitializeComponent();
            StorageCollection = new ObservableCollection<Drive>();
            DataContext = this;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(e);
            vm = App.CommanderVM;

            directory = vm.CurrentActivePanel == 0? vm.LeftPanel : vm.RightPanel;
            foreach (var item in directory.Drives)
            {
                StorageCollection.Add(item);
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
            App.CommanderVM = vm;
        }
        #endregion


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {   
                if(vm.CurrentActivePanel == 0)
                {
                    vm.LeftPanel.CurrentDrive = e.AddedItems[0] as Drive;
                }
                else
                {
                    vm.RightPanel.CurrentDrive = e.AddedItems[0] as Drive;
                }
                Frame.GoBack();
            }
        }
    }
}
