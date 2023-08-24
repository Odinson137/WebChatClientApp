using Microsoft.AspNetCore.Connections;
using System;
using System.Windows;
using System.Windows.Input;
using WebChatClientApp.Models;
using WebChatClientApp.Views;

namespace WebChatClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow.ResizeMode = ResizeMode.NoResize;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MiniMized_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void Closed_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //private void FullWindow_Click(object sender, RoutedEventArgs e)
        //{
        //    Application.Current.MainWindow.WindowState = WindowState.Maximized;

        //}
    }

    
}
