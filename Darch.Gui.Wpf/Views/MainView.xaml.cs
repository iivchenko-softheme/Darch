// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Windows;
using System.Windows.Input;
using Darch.ViewModels.Archiving;
using Darch.ViewModels.ViewModels;
using Microsoft.Win32;

namespace Darch.GUI.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml.
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void OpenArchiveMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                ((IMainViewModel)DataContext).Open(openFileDialog.FileName);
            }
        }

        private void CreateArchiveMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == true)
            {
                ((IMainViewModel)DataContext).Create(saveFileDialog.FileName);
            }
        }

        private void DragWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Darch v 0.1.1\nDeduplication Archive\nDeveloped by Shogun\nE-mail: iivchenko@live.com");
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                ((IMainViewModel)DataContext).Add(openFileDialog.FileName);
            }
        }

        private void Remove_OnClick(object sender, RoutedEventArgs e)
        {
            ((IMainViewModel)DataContext).Remove(((ArchiveFile)this.Files.SelectedItem).Name);
        }

        private void Extract_OnClick(object sender, RoutedEventArgs e)
        {
            ((IMainViewModel)DataContext).Extract(((ArchiveFile)this.Files.SelectedItem).Name);
        }
    }
}
