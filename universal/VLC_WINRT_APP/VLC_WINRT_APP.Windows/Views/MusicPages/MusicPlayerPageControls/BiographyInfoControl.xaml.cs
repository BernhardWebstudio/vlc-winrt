﻿using System;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VLC_WINRT_APP.Views.MusicPages.MusicPlayerPageControls
{
    public sealed partial class BiographyInfoControl : UserControl
    {
        public BiographyInfoControl()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            Window.Current.SizeChanged += OnSizeChanged;
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Debug.WriteLine("Unloading panel BiographyInfoControl");
            Window.Current.SizeChanged -= OnSizeChanged;
        }

        private void OnSizeChanged(object sender, WindowSizeChangedEventArgs sizeChangedEventArgs)
        {
            Responsive();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Responsive();
        }

        void Responsive()
        {
            if (Window.Current.Bounds.Width < 400)
            {
                this.Width = Window.Current.Bounds.Width - 80;
            }
            else if (Window.Current.Bounds.Width < 600)
            {
                this.Width = Window.Current.Bounds.Width - 200;
            }
            else
            {
                this.Width = 480;
            }
        }
    }
}
