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

using System.Net.Mail;
using System.Net;

namespace MailClient.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для SendMailDialog.xaml
    /// </summary>
    public partial class SendMailDialog : Window
    {
        public SendMailDialog()
        {
            InitializeComponent();
        }

        private void SendMailHandler(object sender, RoutedEventArgs e)
        {
            SendMail();
        }

        public void SendMail()
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("glebdyakov2000@gmail.com");
                string recepientBoxContent = recepientBox.Text;
                string[] recepients = recepientBoxContent.Split(new char[] { ',' });
                int countRecepients = recepients.Length;
                bool isManyRecepients = countRecepients >= 2;
                if (isManyRecepients)
                {
                    foreach (string recepient in recepients)
                    {
                        message.To.Add(new MailAddress(recepient));
                    }
                }
                else
                {
                    message.To.Add(new MailAddress(recepientBoxContent));
                }
                string subjectBoxContent = subjectBox.Text;
                message.Subject = subjectBoxContent;
                message.IsBodyHtml = true; //to make message body as html  
                string messageBodyBoxContent = messageBodyBox.Text;
                message.Body = messageBodyBoxContent;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("glebdyakov2000@gmail.com", "ttolpqpdzbigrkhz");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                this.Close();
            }
            catch (Exception) {
                MessageBox.Show("Произошла ошибка при отправке письма", "Ошибка");
            }
        }

    }
}
