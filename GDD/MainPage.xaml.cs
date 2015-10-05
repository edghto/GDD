using GDD.Common;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;
using System.Threading.Tasks;

namespace GDD
{
    public sealed partial class MainPage : Page, Generic.IWebAuthenticationContinuable
    {
        private NavigationHelper navigationHelper;

        private GDrive.Proxy proxy;
        
        public MainPage()
        {
            DataContext = this;
            InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            proxy = GDrive.Proxy.GetInstance();
            Loaded += OnLoadedEventHandler;
        }

        private async void OnLoadedEventHandler(object sender, object e)
        {
            await proxy.InitAsync();
            await PerformAuthorizationAsync();
        }
        
        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await PerformAuthorizationAsync();
        }
        
        private async Task PerformAuthorizationAsync()
        {
            Generic.ContinuationManager.Current = this as Generic.IWebAuthenticationContinuable;
            AuthorizationMessageTextBlock.IsTapEnabled = false;
            AuthorizationMessageTextBlock.Text = "Authorization in progress...";
            if (await proxy.ConnectAsync())
            {
                AuthorizationMessageTextBlock.Text = "Authorization Succeeded";
                Generic.ContinuationManager.Current = null;
                Frame.Navigate(typeof(CommanderPage));
            }
            else
            {
                AuthorizationMessageTextBlock.Text = "Authorization Failed";
                AuthorizationMessageTextBlock.IsTapEnabled = true;
            }
            Generic.ContinuationManager.Current = null;
        }

        #region Navigation managment
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
            AuthorizationMessageTextBlock.Text = "Authorization in progress...";
            AuthorizationMessageTextBlock.IsTapEnabled = false;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedFrom(e);
        }
        #endregion


        #region Implementation of interface Generic.IWebAuthenticationContinuable
        public async void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            proxy.ContinueWebAuthentication(args);
            await PerformAuthorizationAsync();
        }
        #endregion
    }
}
