﻿// Copyright (c) Richasy. All rights reserved.

using System;
using System.ComponentModel;
using System.Linq;
using Richasy.Bili.App.Resources.Extension;
using Richasy.Bili.ViewModels.Uwp;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Richasy.Bili.App.Pages.Overlay
{
    /// <summary>
    /// 分区详情页面.
    /// </summary>
    public sealed partial class PartitionDetailPage : Page
    {
        /// <summary>
        /// Dependency property of <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(PartitionViewModel), typeof(PartitionDetailPage), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionDetailPage"/> class.
        /// </summary>
        public PartitionDetailPage()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        /// <summary>
        /// 分区视图模型.
        /// </summary>
        public PartitionViewModel ViewModel
        {
            get { return (PartitionViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        /// <inheritdoc/>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is PartitionViewModel data)
            {
                this.ViewModel = data;
                this.DataContext = data;
                var animationService = ConnectedAnimationService.GetForCurrentView();
                animationService.TryStartAnimation("PartitionLogoAnimate", PartitionLogo);
                animationService.TryStartAnimation("PartitionNameAnimate", PartitionName);
            }
        }

        /// <inheritdoc/>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            var animationService = ConnectedAnimationService.GetForCurrentView();
            var animate = animationService.PrepareToAnimate("PartitionBackAnimate", this.PartitionHeader);
            animate.Configuration = new DirectConnectedAnimationConfiguration();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            CheckCurrentSubPartition();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.CurrentSelectedSubPartition))
            {
                CheckCurrentSubPartition();
            }
        }

        private void OnDetailNavigationViewItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            var invokeItem = args.InvokedItemContainer as Microsoft.UI.Xaml.Controls.NavigationViewItem;
            var subPartitionId = invokeItem.Tag == null ? -1 : Convert.ToInt32(invokeItem.Tag);
            SubPartitionViewModel vm;
            if (subPartitionId == -1)
            {
                vm = ViewModel.SubPartitionCollection.First();
            }
            else
            {
                vm = ViewModel.SubPartitionCollection.Where(p => p.SubPartitionId == subPartitionId).FirstOrDefault();
            }

            ViewModel.CurrentSelectedSubPartition = vm;
        }

        private void CheckCurrentSubPartition()
        {
            var vm = ViewModel.CurrentSelectedSubPartition;
            if (vm != null)
            {
                if (!(DetailNavigationView.SelectedItem is SubPartitionViewModel selectedItem) || selectedItem != vm)
                {
                    DetailNavigationView.SelectedItem = vm;
                }
            }
        }
    }
}