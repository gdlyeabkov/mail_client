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
using ImapX.Enums;
// using System.Windows.Forms;

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
        }

        public void Initialize()
        {
            debugger = new SpeechSynthesizer();
            ImapService.Initialize();
            ImapService.Login("glebdyakov2000@gmail.com", "ttolpqpdzbigrkhz"); 
            List<Folder> folders = ImapService.GetFolders(debugger);
            foreach (Folder folder in folders)
            {

                // Message[] messages = folder.Search("ALL", MessageFetchMode.ClientDefault, 5);

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
                TextBlock foldersItemCountMessagesLabel = new TextBlock();
                // MessageCollection messages = folder.Messages;
                /*int countMessages = messages.Count();
                String rawCountMessages = countMessages.ToString();
                foldersItemCountMessagesLabel.VerticalAlignment = VerticalAlignment.Center;
                foldersItemCountMessagesLabel.Margin = new Thickness(60, 0, 0, 0);
                foldersItemCountMessagesLabel.Text = rawCountMessages;
                foldersItem.Children.Add(foldersItemCountMessagesLabel);*/
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
                        
                        Message[] messages = subfolder.Search("ALL", MessageFetchMode.ClientDefault, 5);

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
                        TextBlock subFoldersItemCountMessagesLabel = new TextBlock();
                        // messages = subfolder.Messages;
                        int countMessages = messages.Count();
                        string rawCountMessages = countMessages.ToString();
                        subFoldersItemCountMessagesLabel.VerticalAlignment = VerticalAlignment.Center;
                        subFoldersItemCountMessagesLabel.Margin = new Thickness(60, 0, 0, 0);
                        subFoldersItemCountMessagesLabel.Text = rawCountMessages;
                        subFoldersItem.Children.Add(subFoldersItemCountMessagesLabel);
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
                bool isHaveMessages = count >= 2;
                if (isHaveMessages)
                {
                    messages.RowDefinitions.RemoveRange(1, count - 1);
                    int counChilds = messages.Children.Count;
                    messages.Children.RemoveRange(5, counChilds - 1);
                }
                Message[] messageCollection = ImapService.MessageCollectionGetMessagesForFolder(folderTitle);
                bool isGmailFolder = folderTitle == "[Gmail]";
                if (isGmailFolder)
                {
                    messageCollection = ImapService.MessageCollectionGetMessagesForFolder(folderTitle, subFolderIndex);
                }
                foreach (Message message in messageCollection.ToList())
                {
                    string subject = message.Subject;
                    Attachment[] attachments = message.Attachments;
                    DateTime? possibleDate = message.Date;
                    DateTime date = ((DateTime)(possibleDate));
                    List<MailAddress> to = message.To.ToList();
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(35);
                    messages.RowDefinitions.Add(rowDefinition);
                    int countMessages = messages.RowDefinitions.Count;
                    PackIcon favoriteBtn = new PackIcon();
                    favoriteBtn.Kind = PackIconKind.Star;
                    bool isSearchNotExists = search == "";
                    bool isSetSearch = search != "" && subject.Contains(search);
                    bool isSearchMatch = isSearchNotExists || isSetSearch;
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
                        ContextMenu attachmentBtnContextMenu = new ContextMenu();
                        MenuItem attachmentBtnContextMenuItem = new MenuItem();
                        attachmentBtnContextMenuItem.Header = "Скачать";
                        attachmentBtnContextMenuItem.DataContext = attachments;
                        attachmentBtnContextMenuItem.Click += DownloadAttachmentsHandler;
                        attachmentBtnContextMenu.Items.Add(attachmentBtnContextMenuItem);
                        attachmentBtn.ContextMenu = attachmentBtnContextMenu;
                    }
                    else
                    {
                        attachmentBtn.Foreground = Brushes.LightGray;
                    }
                    isSearchNotExists = search == "";
                    isSetSearch = search != "" && subject.Contains(search);
                    isSearchMatch = isSearchNotExists || isSetSearch;
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
                        subjectLabel.DataContext = message;
                        subjectLabel.MouseLeftButtonUp += OpenMessageDialogHandler;
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
                    isSearchNotExists = search == "";
                    isSetSearch = search != "" && subject.Contains(search);
                    isSearchMatch = isSearchNotExists || isSetSearch;
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
                this.Cursor = Cursors.Arrow;
            };
        }

        public void DownloadAttachmentHandler (object sender, RoutedEventArgs e)
        {
            StackPanel attachmentWrap = ((StackPanel)(sender));
            object attachmentWrapData = attachmentWrap.DataContext;
            Attachment attachment = ((Attachment)(attachmentWrapData));
            DownloadAttachment(attachment);
        }

        public void DownloadAttachment (Attachment attachment)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult res = fbd.ShowDialog();
            System.Windows.Forms.DialogResult dialogOkResult = System.Windows.Forms.DialogResult.OK;
            bool isPathSelected = res == dialogOkResult;
            if (isPathSelected)
            {
                string selectedPath = fbd.SelectedPath;
                attachment.Download();
                attachment.Save(selectedPath);
            }
        }

        public void DownloadAttachmentsHandler (object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = ((MenuItem)(sender));
            object menuItemData = menuItem.DataContext;
            Attachment[] attachments = ((Attachment[])(menuItemData));
            DownloadAttachments(attachments);
        }

        public void DownloadAttachments (Attachment[] attachments)
        {

            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult res = fbd.ShowDialog();
            System.Windows.Forms.DialogResult dialogOkResult = System.Windows.Forms.DialogResult.OK;
            bool isPathSelected = res == dialogOkResult;
            if (isPathSelected)
            {
                string selectedPath = fbd.SelectedPath;
                foreach (Attachment attachment in attachments)
                {
                    attachment.Download();
                    attachment.Save(selectedPath);
                }
            }
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
            this.Cursor = Cursors.Wait;

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

        private void SendMailHandler(object sender, RoutedEventArgs e)
        {
            Dialogs.SendMailDialog dialog = new Dialogs.SendMailDialog("");
            dialog.Closed += RefreshMessagesHandler;
            dialog.Show();
        }

        public void RefreshMessagesHandler(object sender, EventArgs e)
        {
            OutputMessages(currentFolder, currentSubFolder);
        }

        public void OpenMessageDialogHandler(object sender, RoutedEventArgs e)
        {
            TextBlock subject = ((TextBlock)(sender));
            object subjectData = subject.DataContext;
            Message msg = ((Message)(subjectData));
            OpenMessageDialog(msg);
        }

        public void OpenMessageDialog (Message msg)
        {
            /*
            Dialogs.MessageDialog dialog = new Dialogs.MessageDialog(msg);
            dialog.Show();
            */
            article.SelectedIndex = 1;
            msgDetailSubjectLabel.Text = msg.Subject;
            MailAddress from = msg.From;
            string fromAddress = from.Address;
            msgDetailFromLabel.Text = fromAddress;
            DateTime? possibleMsgDate = msg.Date;
            bool isMsgDateExists = possibleMsgDate != null;
            if (isMsgDateExists)
            {
                DateTime msgDate = ((DateTime)(possibleMsgDate));
                string rawMsgDate = msgDate.ToLongDateString();
                msgDetailDateLabel.Text = rawMsgDate;
            }
            MessageBody msgBody = msg.Body;
            msgDetailContentLabel.Text = msgBody.Text;
            foreach (Attachment attachment in msg.Attachments) {
                Border msgDetailAttachmentBorder = new Border();
                msgDetailAttachmentBorder.BorderBrush = System.Windows.Media.Brushes.Black;
                msgDetailAttachmentBorder.BorderThickness = new Thickness(1);
                msgDetailAttachmentBorder.Margin = new Thickness(15);
                StackPanel msgDetailAttachment = new StackPanel();
                msgDetailAttachment.Width = 100;
                msgDetailAttachment.Height = 100;
                msgDetailAttachment.Margin = new Thickness(15);
                PackIcon msgDetailAttachmentDownloadIcon = new PackIcon();
                msgDetailAttachmentDownloadIcon.Margin = new Thickness(15);
                msgDetailAttachmentDownloadIcon.Width = 24;
                msgDetailAttachmentDownloadIcon.Height = 24;
                msgDetailAttachmentDownloadIcon.Kind = PackIconKind.Download;
                msgDetailAttachmentDownloadIcon.HorizontalAlignment = HorizontalAlignment.Center;
                msgDetailAttachment.Children.Add(msgDetailAttachmentDownloadIcon);
                string msgDetailAttachmentName = attachment.FileName;
                TextBlock msgDetailAttachmentNameLabel = new TextBlock();
                msgDetailAttachmentNameLabel.TextAlignment = TextAlignment.Center;
                msgDetailAttachmentNameLabel.TextWrapping = TextWrapping.Wrap;
                msgDetailAttachmentNameLabel.Text = msgDetailAttachmentName;
                msgDetailAttachment.Children.Add(msgDetailAttachmentNameLabel);
                msgDetailAttachmentBorder.Child = msgDetailAttachment;
                msgDetailAttachments.Children.Add(msgDetailAttachmentBorder);
                msgDetailAttachment.DataContext = attachment;
                msgDetailAttachment.MouseLeftButtonDown += DownloadAttachmentHandler;
            }
        }

        private void BackToMessagesListHandler(object sender, MouseButtonEventArgs e)
        {
            BackToMessagesList();
        }

        public void BackToMessagesList()
        {
            article.SelectedIndex = 0;
        }

        private void ReplyHandler(object sender, RoutedEventArgs e)
        {
            Reply();
        }

        public void Reply ()
        {
            string msgDetailFromLabelContent = msgDetailFromLabel.Text;
            Dialogs.SendMailDialog dialog = new Dialogs.SendMailDialog(msgDetailFromLabelContent);
            dialog.Show();
        }

        private void InitializeHandler(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

    }
}
