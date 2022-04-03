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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Speech.Synthesis;
using ImapX.Collections;
using ImapX;
using MaterialDesignThemes.Wpf;
using System.Windows.Threading;

namespace MailClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public SpeechSynthesizer debugger;
        public string search = "";
        public string currentFolder = "[Gmail]";
        public int currentSubFolder = 1;

        public MainWindow()
        {
            InitializeComponent();

            Initialize();
        
        }

        public void Initialize()
        {
            debugger = new SpeechSynthesizer();
            ImapService.Initialize();
            ImapService.Login("glebdyakov2000@gmail.com", "ttolpqpdzbigrkhz"); 
            List<Folder> folders = ImapService.GetFolders(debugger);
            foreach (Folder folder in folders)
            {
                string folderTitle = folder.Name;
                StackPanel foldersItem = new StackPanel();
                foldersItem.Orientation = Orientation.Horizontal;
                foldersItem.Height = 35;
                PackIcon foldersItemIcon = new PackIcon();
                foldersItemIcon.Kind = PackIconKind.Email;
                foldersItemIcon.Width = 24;
                foldersItemIcon.Height = 24;
                foldersItemIcon.Margin = new Thickness(5, 0, 5, 0);
                foldersItemIcon.VerticalAlignment = VerticalAlignment.Center;
                foldersItem.Children.Add(foldersItemIcon);
                TextBlock foldersItemLabel = new TextBlock();
                foldersItemLabel.Text = folderTitle;
                foldersItemLabel.VerticalAlignment = VerticalAlignment.Center;
                foldersItem.Children.Add(foldersItemLabel);
                emailFolders.Children.Add(foldersItem);
                foldersItem.DataContext = folder;
                foldersItem.MouseUp += SelectFolderHandler;
                List<Folder> subFolders = folder.SubFolders.ToList();
                int countSubFolders = subFolders.Count;
                bool isHaveSubFolders = countSubFolders >= 1;
                if (isHaveSubFolders)
                {
                    foreach (Folder subfolder in subFolders)
                    {
                        string subFolderTitle = subfolder.Name;
                        StackPanel subFoldersItem = new StackPanel();
                        subFoldersItem.Orientation = Orientation.Horizontal;
                        subFoldersItem.Height = 35;
                        subFoldersItem.Margin = new Thickness(25, 0, 0, 0);
                        PackIcon subFoldersItemIcon = new PackIcon();
                        subFoldersItemIcon.Kind = PackIconKind.Email;
                        subFoldersItemIcon.Width = 24;
                        subFoldersItemIcon.Height = 24;
                        subFoldersItemIcon.Margin = new Thickness(5, 0, 5, 0);
                        subFoldersItemIcon.VerticalAlignment = VerticalAlignment.Center;
                        subFoldersItem.Children.Add(subFoldersItemIcon);
                        TextBlock subFoldersItemLabel = new TextBlock();
                        subFoldersItemLabel.Text = subFolderTitle;
                        subFoldersItemLabel.VerticalAlignment = VerticalAlignment.Center;
                        subFoldersItem.Children.Add(subFoldersItemLabel);
                        emailFolders.Children.Add(subFoldersItem);
                        int subFolderIndex = subFolders.IndexOf(subfolder);
                        subFoldersItem.DataContext = subFolderIndex;
                        subFoldersItem.MouseUp += SelectSubFolderHandler;
                    }
                }
            }
            OutputMessages("[Gmail]");

        }

        public void OutputMessages(string folderTitle, int subFolderIndex = 1)
        {
            loader.Visibility = Visibility.Visible;
            messages.Visibility = Visibility.Collapsed;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Start();
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                int count = messages.RowDefinitions.Count;
                if (count >= 2)
                {
                    messages.RowDefinitions.RemoveRange(1, count - 1);
                    int counChilds = messages.Children.Count;
                    messages.Children.RemoveRange(5, counChilds - 1);
                }
                Message[] messageCollection = ImapService.MessageCollectionGetMessagesForFolder(folderTitle);
                if (folderTitle == "[Gmail]")
                {
                    messageCollection = ImapService.MessageCollectionGetMessagesForFolder(folderTitle, subFolderIndex);
                }
                foreach (Message message in messageCollection.ToList())
                {
                    string subject = message.Subject;
                    Attachment[] attachments = message.Attachments;
                    DateTime date = ((DateTime)(message.Date));
                    List<MailAddress> to = message.To.ToList();
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(35);
                    messages.RowDefinitions.Add(rowDefinition);
                    int countMessages = messages.RowDefinitions.Count;
                    PackIcon favoriteBtn = new PackIcon();
                    favoriteBtn.Kind = PackIconKind.Star;
                    bool isSearchMatch = search == "" || (search != "" && subject.Contains(search));
                    if (isSearchMatch)
                    {
                        messages.Children.Add(favoriteBtn);
                        Grid.SetRow(favoriteBtn, countMessages - 1);
                        Grid.SetColumn(favoriteBtn, 0);
                    }
                    PackIcon attachmentBtn = new PackIcon();
                    int countAttachments = attachments.Length;
                    bool isHaveAttachments = countAttachments >= 1;
                    attachmentBtn.Kind = PackIconKind.Attachment;
                    if (isHaveAttachments)
                    {
                        attachmentBtn.Foreground = Brushes.Black;
                    }
                    else
                    {
                        attachmentBtn.Foreground = Brushes.LightGray;
                    }
                    isSearchMatch = search == "" || (search != "" && subject.Contains(search));
                    if (isSearchMatch)
                    {
                        messages.Children.Add(attachmentBtn);
                        Grid.SetRow(attachmentBtn, countMessages - 1);
                        Grid.SetColumn(attachmentBtn, 1);
                    }
                    TextBlock subjectLabel = new TextBlock();
                    subjectLabel.Text = subject;
                    isSearchMatch = search == "" || (search != "" && subject.Contains(search));
                    if (isSearchMatch)
                    {
                        messages.Children.Add(subjectLabel);
                        Grid.SetRow(subjectLabel, countMessages - 1);
                        Grid.SetColumn(subjectLabel, 2);
                    }
                    TextBlock toLabel = new TextBlock();
                    string toLabelContent = "";
                    foreach (MailAddress toItem in to)
                    {
                        string address = toItem.Address;
                        toLabelContent += address;
                        int index = to.IndexOf(toItem);
                        int countAdresess = to.Count;
                        int lastIndex = countAdresess - 1;
                        bool isNotLastIndex = index < lastIndex;
                        if (isNotLastIndex)
                        {
                            toLabelContent += ", ";
                        }
                    }
                    toLabel.Text = toLabelContent;
                    isSearchMatch = search == "" || (search != "" && subject.Contains(search));
                    if (isSearchMatch)
                    {
                        messages.Children.Add(toLabel);
                        Grid.SetRow(toLabel, countMessages - 1);
                        Grid.SetColumn(toLabel, 3);
                        TextBlock dateLabel = new TextBlock();
                        dateLabel.Text = date.ToLongDateString();
                        messages.Children.Add(dateLabel);
                        Grid.SetRow(dateLabel, countMessages - 1);
                        Grid.SetColumn(dateLabel, 4);
                    }
                }
                loader.Visibility = Visibility.Collapsed;
                messages.Visibility = Visibility.Visible;
            };
        }

        public void SelectFolderHandler(object sender, MouseEventArgs e)
        {
            StackPanel folderItem = ((StackPanel)(sender));
            object rawFolder = folderItem.DataContext;
            Folder folder = ((Folder)(rawFolder));
            SelectFolder(folder.Name);
        }

        public void SelectSubFolderHandler(object sender, MouseEventArgs e)
        {

            loader.Visibility = Visibility.Visible;
            messages.Visibility = Visibility.Collapsed;

            StackPanel folderItem = ((StackPanel)(sender));
            object rawFolder = folderItem.DataContext;
            int subFolderIndex = ((int)(rawFolder));
            SelectFolder("[Gmail]", subFolderIndex);
        }

        public void SelectFolder(string folderTitle, int subFolderIndex = 1)
        {
            currentFolder = folderTitle;
            currentSubFolder = subFolderIndex;
            OutputMessages(folderTitle, subFolderIndex);
        }

        private void OpenFilterMessagesHandler(object sender, RoutedEventArgs e)
        {
            OpenFilterMessages();
        }

        public void OpenFilterMessages()
        {
            Dialogs.FilterMessagesDialog dialog = new Dialogs.FilterMessagesDialog();
            dialog.Closed += FilterMessagesHandler;
            dialog.Show();
        }

        public void FilterMessagesHandler(object sender, EventArgs e)
        {
            Window dialog = ((Window)(sender));
            object data = dialog.DataContext;
            search = ((string)(data));
            OutputMessages(currentFolder, currentSubFolder);
        }

        private void ResetFilterMessagesHandler(object sender, RoutedEventArgs e)
        {
            ResetFilterMessages();
        }

        public void ResetFilterMessages()
        {
            search = "";
            OutputMessages(currentFolder, currentSubFolder);
        }

    }
}
