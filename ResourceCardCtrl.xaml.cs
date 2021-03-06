﻿using CatanSharedModels;
using CatanSvcTestClient;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CatanSvcTestClient
{
    public sealed partial class ResourceCardCtrl : UserControl
    {
        public ResourceCardCtrl()
        {
            this.InitializeComponent();

        }

        public static readonly DependencyProperty ResourceTypeProperty = DependencyProperty.Register("ResourceType", typeof(ResourceType), typeof(ResourceCardCtrl), new PropertyMetadata(ResourceType.Sheep, ResourceTypeChanged));
        public static readonly DependencyProperty CountProperty = DependencyProperty.Register("Count", typeof(int), typeof(ResourceCardCtrl), new PropertyMetadata(0, CountChanged));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(TileOrientation), typeof(ResourceCardCtrl), new PropertyMetadata(TileOrientation.FaceDown, OrientationChanged));
        public static readonly DependencyProperty ReadOnlyProperty = DependencyProperty.Register("ReadOnly", typeof(bool), typeof(ResourceCardCtrl), new PropertyMetadata(false, ReadOnlyChanged));

        public bool ReadOnly
        {
            get => (bool)GetValue(ReadOnlyProperty);
            set => SetValue(ReadOnlyProperty, value);
        }
        private static void ReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var depPropClass = d as ResourceCardCtrl;
            var depPropValue = (bool)e.NewValue;
            depPropClass?.SetReadOnly(depPropValue);
        }
        private void SetReadOnly(bool value)
        {

            _txtCount.IsReadOnly = value;
        }

        
        public TileOrientation Orientation
        {
            get => (TileOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
        private static void OrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResourceCardCtrl depPropClass = d as ResourceCardCtrl;
            TileOrientation depPropValue = (TileOrientation)e.NewValue;
            depPropClass.SetOrientation(depPropValue);
        }
        private void SetOrientation(TileOrientation value)
        {
            SetOrientationAsync(value);
        }

        public int Count
        {
            get
            {
                try
                {
                    return (int)GetValue(CountProperty);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                try
                {
                    SetValue(CountProperty, value);
                }
                catch
                {
                    SetValue(CountProperty, 0);
                }
            }
            
        }
        private static void CountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResourceCardCtrl depPropClass = d as ResourceCardCtrl;
            int depPropValue = (int)e.NewValue;
            depPropClass.SetCount(depPropValue);
        }
        private void SetCount(int value)
        {
            
        }

        public ResourceType ResourceType
        {
            get => (ResourceType)GetValue(ResourceTypeProperty);
            set => SetValue(ResourceTypeProperty, value);
        }
        private static void ResourceTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResourceCardCtrl depPropClass = d as ResourceCardCtrl;
            ResourceType depPropValue = (ResourceType)e.NewValue;
            depPropClass.SetResourceType(depPropValue);
        }
        private void SetResourceType(ResourceType value)
        {
            string bitmapPath = "ms-appx:Assets/back.jpg";
            // VictoryPoint, Knight, YearOfPlenty, RoadBuilding, Monopoly
            switch (value)
            {
                case ResourceType.Sheep:
                    bitmapPath = "ms-appx:Assets/sheep.png";
                    break;
                case ResourceType.Wood:
                    bitmapPath = "ms-appx:Assets/wood.png";
                    break;
                case ResourceType.Ore:
                    bitmapPath = "ms-appx:Assets/ore.png";
                    break;
                case ResourceType.Wheat:
                    bitmapPath = "ms-appx:Assets/wheat.png";
                    break;
                case ResourceType.Brick:
                    bitmapPath = "ms-appx:Assets/brick.png";
                    break;
                case ResourceType.Desert:
                case ResourceType.None:
                    bitmapPath = "ms-appx:Assets/back.png";
                    break;
                case ResourceType.GoldMine:
                    bitmapPath = "ms-appx:Assets/gold.png";
                    break;
                case ResourceType.VictoryPoint:
                    bitmapPath = "ms-appx:Assets/VictoryPoint.jpg";
                    break;
                case ResourceType.Knight:
                    bitmapPath = "ms-appx:Assets/knight.jpg";
                    break;
                case ResourceType.YearOfPlenty:
                    bitmapPath = "ms-appx:Assets/YearOfPlenty.jpg";
                    break;
                case ResourceType.RoadBuilding:
                    bitmapPath = "ms-appx:Assets/RoadBuilding.jpg";
                    break;
                case ResourceType.Monopoly:
                    bitmapPath = "ms-appx:Assets/Monopoly.jpg";
                    break;

                default:
                    break;

            }
            BitmapImage bitmapImage = new BitmapImage(new Uri(bitmapPath, UriKind.RelativeOrAbsolute));
            _imgFront.ImageSource = bitmapImage;
            _imgFront.Stretch = Stretch.UniformToFill;

        }

        public void SetOrientationAsync(TileOrientation orientation, double startAfter = 0)
        {
            bool flipToFaceUp = (orientation == TileOrientation.FaceUp) ? true : false;
            StaticHelpers.SetupFlipAnimation(flipToFaceUp, _daFlipBackCard, _daFlipFrontCard, 100, startAfter);
            _sbFlipTile.Begin();
        }

        
    }
}
