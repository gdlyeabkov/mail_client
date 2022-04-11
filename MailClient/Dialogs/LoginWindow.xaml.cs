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

namespace MailClient.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginHandler(object sender, RoutedEventArgs e)
        {
            Login();
        }

        public void Login ()
        {
            string login = loginField.Text;
            string password = passwordField.Password;
            bool isAuthenticated = ImapService.Login(login, password);
            if (isAuthenticated)
            {
                this.Hide();
                MainWindow window = new MainWindow();
                window.Cursor = Cursors.Wait;
                window.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Вы ввели неправильные данные", "Ошибка авторизации", MessageBoxButton.OK);
            }
        }

        private void InitializeHandler(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        public void Initialize()
        {
            ImapService.Initialize();
        }

    }
}
