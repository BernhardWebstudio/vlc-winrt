﻿using VLC_WinRT.Model.Video;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using VLC_WinRT.ViewModels;
using VLC_WinRT.Model.Music;
using VLC_WinRT.Utils;
using Windows.UI.Xaml.Media.Imaging;
using System;
#if WINDOWS_PHONE_APP
using Windows.Phone.UI.Input;
#endif

namespace VLC_WinRT.UI.Legacy.Views.UserControls
{
    public sealed partial class AlbumWithTracksResponsiveTemplate : UserControl
    {
        bool areTracksVisible = false;
        bool forceVisibleTracks = false;
        public AlbumWithTracksResponsiveTemplate()
        {
            this.InitializeComponent();
            this.Loaded += AlbumWithTracksResponsiveTemplate_Loaded;
        }

        private void AlbumWithTracksResponsiveTemplate_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.SizeChanged += AlbumWithTracksResponsiveTemplate_SizeChanged;
            ResponsiveTracksListView();
            Responsive();
#if WINDOWS_PHONE_APP
            HardwareButtons.BackPressed += (obj, args) =>
            {
                if (areTracksVisible && forceVisibleTracks)
                {
                    Locator.MainVM.PreventAppExit = true;
                    args.Handled = true;
                    HideTracks();
                }
            };
#endif
        }

        private void AlbumWithTracksResponsiveTemplate_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            ResponsiveTracksListView();
            Responsive();
        }

        void Responsive()
        {
            if (forceVisibleTracks)
            {
                forceVisibleTracks = false;
                return;
            }

            if (this.ActualWidth > 600)
            {
                ShowTracks();
            }
            else
            {
                HideTracks();
            }

            if (this.ActualWidth > 900)
            {
                CoverImage.Width = CoverImage.Height = HeaderGrid.Height = 150;
            }
            else
            {
                CoverImage.Width = CoverImage.Height = HeaderGrid.Height = 90;
            }
        }

        void ShowTracks()
        {
            TracksListView.Visibility = PlayAppBarButton.Visibility = FavoriteAppBarButton.Visibility = PinAppBarButton.Visibility = Visibility.Visible;
            areTracksVisible = true;
        }

        void HideTracks()
        {
            TracksListView.Visibility = PlayAppBarButton.Visibility = FavoriteAppBarButton.Visibility = PinAppBarButton.Visibility = Visibility.Collapsed;
            areTracksVisible = false;
        }

        void ResponsiveTracksListView()
        {
            var wrapGrid = TracksListView.ItemsPanelRoot as ItemsWrapGrid;
            if (wrapGrid == null) return;
            TemplateSizer.ComputeAlbumTracks(ref wrapGrid, TracksListView.ActualWidth - wrapGrid.Margin.Left - wrapGrid.Margin.Right);
        }

        private void HeaderGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (!areTracksVisible)
            {
                forceVisibleTracks = true;
                ShowTracks();
            }
        }
        
        public AlbumItem Album
        {
            get { return (AlbumItem)GetValue(AlbumProperty); }
            set { SetValue(AlbumProperty, value); }
        }

        public static readonly DependencyProperty AlbumProperty =
            DependencyProperty.Register("Album", typeof(AlbumItem), typeof(AlbumWithTracksResponsiveTemplate), new PropertyMetadata(null, PropertyChangedCallback));


        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var that = (AlbumWithTracksResponsiveTemplate)dependencyObject;
            that.Init();
        }

        public void Init()
        {
            if (Album == null) return;
            NameTextBlock.Text = Strings.HumanizedAlbumName(Album.Name);

            PlayAppBarButton.Label = Strings.PlayAlbum;
            PlayAppBarButton.Command = Locator.MusicLibraryVM.PlayAlbumCommand;
            PlayAppBarButton.CommandParameter = Album;

            PinAppBarButton.Command = Album.PinAlbumCommand;
            PinAppBarButton.CommandParameter = Album;

            FavoriteAppBarButton.Command = Album.FavoriteAlbum;
            FavoriteAppBarButton.CommandParameter = Album;

            TracksListView.ItemsSource = await Locator.MusicLibraryVM.MusicLibrary.LoadTracksByAlbumId(Album.Id);
        }
    }
}