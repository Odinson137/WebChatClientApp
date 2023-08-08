using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WebChatClientApp.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public delegate void Funcs();

        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register("Parameter", typeof(Funcs), typeof(ChatMenuView));

        public Funcs Parameter
        {
            get { return (Funcs)GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        public LoginView()
        {
            InitializeComponent();
        }

        private void Closed_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
