﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Xaml.Interactivity;
using VLC.Model.Video;
using VLC.ViewModels;

namespace VLC_WinRT.UI.Legacy.Views.VariousPages
{
    public sealed partial class SearchPage : Page
    {
        public SearchPage()
        {
            this.InitializeComponent();
            this.Loaded += SearchPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Locator.SearchVM.OnNavigatedTo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Locator.SearchVM.Dispose();
        }
        
        private void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            Responsive(Window.Current.Bounds.Width);
            Window.Current.SizeChanged += Current_SizeChanged;
            this.Unloaded += MusicPaneButtons_Unloaded;
        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            Responsive(e.Size.Width);
        }

        void MusicPaneButtons_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        void Responsive(double width)
        {
            if (width <= 700)
                VisualStateUtilities.GoToState(this, "Minimal", false);
            else
                VisualStateUtilities.GoToState(this, "Normal", false);
        }

        private void MusicWrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TemplateSizer.ComputeAlbums(sender as ItemsWrapGrid, this.ActualWidth);
        }

        private void VideosWrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TemplateSizer.ComputeCompactVideo(sender as ItemsWrapGrid, this.ActualWidth);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Locator.SearchVM.SearchTag = MusicSearchBox.Text;
        }

        private void ToggleSearchMode_Click(object sender, RoutedEventArgs e)
        {
            Locator.SearchVM.MusicSearchEnabled = !Locator.SearchVM.MusicSearchEnabled;
        }

        private void AlbumItemClick(object sender, ItemClickEventArgs e)
        {
            Locator.MusicLibraryVM.AlbumClickedCommand.Execute(e.ClickedItem);
        }

        private void VideoItemClick(object sender, ItemClickEventArgs e)
        {
            Locator.VideoLibraryVM.OpenVideo.Execute(e.ClickedItem);
        }
    }
}