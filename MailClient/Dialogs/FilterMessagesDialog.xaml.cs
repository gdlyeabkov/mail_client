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
    /// Логика взаимодействия для FilterMessagesDialog.xaml
    /// </summary>
    public partial class FilterMessagesDialog : Window
    {
        public FilterMessagesDialog()
        {
            InitializeComponent();
        }

        
        private void FilterMessagesHandler(object sender, RoutedEventArgs e)
        {
            string search = searchBox.Text;
            this.DataContext = search;
            this.Close();
        }

    }
}
