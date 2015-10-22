using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GDD
{
    class ProgressManager : IDisposable
    {
        private FlipView mainContent;
        private StackPanel progressLayer;
        private ProgressRing progressRingHandler;

        public ProgressManager(FlipView mainContent, StackPanel progressLayer, ProgressRing progressRingHandler)
        {
            this.mainContent = mainContent;
            this.progressLayer = progressLayer;
            this.progressRingHandler = progressRingHandler;

            this.progressRingHandler.IsActive = true;
            this.progressLayer.Visibility = Visibility.Visible;
            this.mainContent.Visibility = Visibility.Collapsed;
        }
        
        public void Dispose()
        {
            this.progressLayer.Visibility = Visibility.Collapsed;
            this.mainContent.Visibility = Visibility.Visible;
            this.progressRingHandler.IsActive = false;
        }
    }
}
