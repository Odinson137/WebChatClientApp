using System.Windows;
using System.Windows.Controls;

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
