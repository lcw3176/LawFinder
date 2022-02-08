using LawFinders.ViewModel;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace LawFinders
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Height = SystemParameters.PrimaryScreenHeight * 0.8;
            this.Width = SystemParameters.PrimaryScreenWidth * 0.8;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.DataContext = new MainViewModel(browser);
        }
    }
}
