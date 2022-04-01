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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MailClient
{
    /// <summary>
    /// Логика взаимодействия для SpinnerControl.xaml
    /// </summary>
    public partial class SpinnerControl : UserControl
    {

        private Storyboard _sb;

        public SpinnerControl()
        {
            InitializeComponent();
            StartStoryBoard();
            IsVisibleChanged += CircularProgressBarBlueIsVisibleChanged;
        }

        void CircularProgressBarBlueIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_sb == null) return;
            if (e != null && e.NewValue != null && (((bool)e.NewValue)))
            {
                _sb.Begin();
                _sb.Resume();
            }
            else
            {
                _sb.Stop();
            }
        }

        void StartStoryBoard()
        {
            try
            {
                _sb = (Storyboard)TryFindResource("spinning");
                if (_sb != null)
                    _sb.Begin();
            }
            catch
            { }
        }

    }
}
