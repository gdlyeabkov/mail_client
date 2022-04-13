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
using System.Web.Script.Serialization;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Effects;
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
        public int currentMsgIndex = 0;
        public bool isPagesSelectorInit = false;

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
                        bool isNecessarilySubFolder = subFolderTitle == "Важное";
                        bool isBucketSubFolder = subFolderTitle == "Корзина";
                        bool isSendSubFolder = subFolderTitle == "Отправленные";
                        bool isBookmarkSubFolder = subFolderTitle == "Помеченные";
                        bool isSpamSubFolder = subFolderTitle == "Спам";
                        bool isNoteSubFolder = subFolderTitle == "Черновики";
                        if (isNecessarilySubFolder)
                        {
                            subFoldersItemIcon.Kind = PackIconKind.Warning;
                        }
                        else if (isBucketSubFolder)
                        {
                            subFoldersItemIcon.Kind = PackIconKind.Bucket;
                        }
                        else if (isSendSubFolder)
                        {
                            subFoldersItemIcon.Kind = PackIconKind.Send;
                        }
                        else if (isBookmarkSubFolder)
                        {
                            subFoldersItemIcon.Kind = PackIconKind.Bookmark;
                        }
                        else if (isSpamSubFolder)
                        {
                            subFoldersItemIcon.Kind = PackIconKind.Ads;
                        }
                        else if (isNoteSubFolder)
                        {
                            subFoldersItemIcon.Kind = PackIconKind.Note;
                        }
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
            messagesWrap.Visibility = Visibility.Collapsed;
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

                // pages.Children.Clear();
                // pagesSelector.Items.Clear();
                bool isPagesSelectorNotInit = false;

                foreach (Message message in messageCollection.ToList())
                {
                    string subject = message.Subject;
                    Attachment[] attachments = message.Attachments;
                    DateTime? possibleDate = message.Date;
                    DateTime date = ((DateTime)(possibleDate));
                    List<MailAddress> to = message.To.ToList();

                    List<Message> messageListCollection = messageCollection.ToList();
                    int msgIndex = messageListCollection.IndexOf(message);
                    int startMsgIndex = currentMsgIndex - 4;
                    int endMsgIndex = currentMsgIndex;
                    bool isStartIndexMatch = msgIndex >= startMsgIndex;
                    bool isEndIndexMatch = msgIndex <= endMsgIndex;
                    bool isMsgMatch = isStartIndexMatch && isEndIndexMatch;
                    if (isMsgMatch)
                    {
                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height = new GridLength(35);
                        messages.RowDefinitions.Add(rowDefinition);
                    }

                    int countMessages = messages.RowDefinitions.Count;
                    PackIcon favoriteBtn = new PackIcon();
                    
                    MessageBody messageBody = message.Body;
                    string messageBodyContent = messageBody.Text;

                    Environment.SpecialFolder localApplicationDataFolder = Environment.SpecialFolder.LocalApplicationData;
                    string localApplicationDataFolderPath = Environment.GetFolderPath(localApplicationDataFolder);
                    string saveDataFilePath = localApplicationDataFolderPath + @"\OfficeWare\MailClient\save-data.txt";
                    string cachePath = localApplicationDataFolderPath + @"\OfficeWare\MailClient";
                    bool isCacheFolderExists = Directory.Exists(cachePath);
                    bool isCacheFolderNotExists = !isCacheFolderExists;
                    if (isCacheFolderNotExists)
                    {
                        Directory.CreateDirectory(cachePath);
                        using (Stream s = File.Open(saveDataFilePath, FileMode.OpenOrCreate))
                        {
                            using (StreamWriter sw = new StreamWriter(s))
                            {
                                sw.Write("");
                            }
                        };
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    string rawLoadedContent = File.ReadAllText(saveDataFilePath);
                    SavedContent loadedContent = js.Deserialize<SavedContent>(rawLoadedContent);
                    bool isLoadedContentExists = loadedContent != null;
                    if (isLoadedContentExists)
                    {
                        bool isMarkAsFavorite = loadedContent.favorites.Contains(messageBodyContent);
                        if (isMarkAsFavorite)
                        {
                            favoriteBtn.Foreground = System.Windows.Media.Brushes.Orange;
                        }
                    }
                    favoriteBtn.Kind = PackIconKind.Star;
                    favoriteBtn.DataContext = messageBodyContent;
                    favoriteBtn.MouseLeftButtonUp += MarkMessageAsFavoriteHandler;
                    bool isSearchNotExists = search == "";
                    bool isSetSearch = search != "" && subject.Contains(search);
                    bool isSearchMatch = isSearchNotExists || isSetSearch;
                    if (isSearchMatch)
                    {
                        if (messageCollection.ToList().IndexOf(message) >= currentMsgIndex - 4 && messageCollection.ToList().IndexOf(message) <= currentMsgIndex)
                        {
                            messages.Children.Add(favoriteBtn);
                            Grid.SetRow(favoriteBtn, countMessages - 1);
                            Grid.SetColumn(favoriteBtn, 0);
                        }
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
                        if (isMsgMatch)
                        {
                            messages.Children.Add(attachmentBtn);
                            Grid.SetRow(attachmentBtn, countMessages - 1);
                            Grid.SetColumn(attachmentBtn, 1);
                        }
                    }
                    TextBlock subjectLabel = new TextBlock();
                    subjectLabel.Text = subject;
                    isSearchMatch = search == "" || (search != "" && subject.Contains(search));
                    if (isSearchMatch)
                    {
                        if (isMsgMatch)
                        {
                            messages.Children.Add(subjectLabel);
                            Grid.SetRow(subjectLabel, countMessages - 1);
                            Grid.SetColumn(subjectLabel, 2);
                            subjectLabel.DataContext = message;
                            subjectLabel.MouseLeftButtonUp += OpenMessageDialogHandler;
                        }
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
                        if (isMsgMatch)
                        {
                            messages.Children.Add(toLabel);
                            Grid.SetRow(toLabel, countMessages - 1);
                            Grid.SetColumn(toLabel, 3);
                            TextBlock dateLabel = new TextBlock();
                            dateLabel.Text = date.ToLongDateString();
                            dateLabel.TextAlignment = TextAlignment.Center;
                            messages.Children.Add(dateLabel);
                            Grid.SetRow(dateLabel, countMessages - 1);
                            Grid.SetColumn(dateLabel, 4);
                        }
                    }

                    CheckBox checkBox = new CheckBox();
                    checkBox.Click += MessageToggledHandler;
                    isSearchNotExists = search == "";
                    isSetSearch = search != "" && subject.Contains(search);
                    isSearchMatch = isSearchNotExists || isSetSearch;
                    if (isSearchMatch)
                    {
                        if (isMsgMatch)
                        {
                            messages.Children.Add(checkBox);
                            Grid.SetRow(checkBox, countMessages - 1);
                            Grid.SetColumn(checkBox, 5);
                        }
                    }

                    int msgNum = msgIndex + 1;
                    double remainder = msgNum % 5;
                    bool isLastMsgOfPage = remainder == 0;
                    if (isLastMsgOfPage)
                    {
                        isPagesSelectorNotInit = !isPagesSelectorInit;
                        if (isPagesSelectorNotInit)
                        {
                            ComboBoxItem pagesSelectorItem = new ComboBoxItem();
                            pagesSelectorItem.Content = (messageCollection.ToList().IndexOf(message) - 3).ToString() + "-" + (messageCollection.ToList().IndexOf(message) + 1).ToString();
                            pagesSelectorItem.DataContext = ((int)(messageCollection.ToList().IndexOf(message)));
                            pagesSelector.Items.Add(pagesSelectorItem);
                        }
                    }

                }
                loader.Visibility = Visibility.Collapsed;
                messagesWrap.Visibility = Visibility.Visible;
                this.Cursor = Cursors.Arrow;
                isPagesSelectorNotInit = !isPagesSelectorInit;
                ItemCollection pagesSelectorItems = pagesSelector.Items;
                int pagesSelectorItemsCount = pagesSelectorItems.Count;
                bool isPagesExists = pagesSelectorItemsCount >= 1;
                bool isNeedPagesSelectorInit = isPagesExists && isPagesSelectorNotInit;
                if (isNeedPagesSelectorInit) 
                {
                    pagesSelector.SelectedIndex = 0;
                    isPagesSelectorInit = true;
                }
                actionBar.DataContext = ((Message[])(messageCollection));
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
            messagesWrap.Visibility = Visibility.Collapsed;
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

        public void MarkMessageAsFavoriteHandler (object sender, RoutedEventArgs e)
        {
            PackIcon favoriteBtn = ((PackIcon)(sender));
            object favoriteBtnData = favoriteBtn.DataContext;
            string msgContent = ((string)(favoriteBtnData));
            MarkMessageAsFavorite(msgContent, favoriteBtn);
        }

        public void MarkMessageAsFavorite (string msgContent, PackIcon favoriteBtn)
        {
            Environment.SpecialFolder localApplicationDataFolder = Environment.SpecialFolder.LocalApplicationData;
            string localApplicationDataFolderPath = Environment.GetFolderPath(localApplicationDataFolder);
            string saveDataFilePath = localApplicationDataFolderPath + @"\OfficeWare\MailClient\save-data.txt";

            JavaScriptSerializer js = new JavaScriptSerializer();
            string rawLoadedContent = File.ReadAllText(saveDataFilePath);
            SavedContent loadedContent = js.Deserialize<SavedContent>(rawLoadedContent);
            List<string> loadedContentFavorites;
            SavedContent savedContent = new SavedContent() {
                favorites = new List<string>()
            };
            if (loadedContent == null)
            {
                loadedContentFavorites = new List<string>();
                savedContent.favorites = loadedContentFavorites;
            }
            else
            {
                loadedContentFavorites = loadedContent.favorites;
                savedContent.favorites = loadedContentFavorites;
                bool isMarkAsFavorite = loadedContent.favorites.Contains(msgContent);
                if (isMarkAsFavorite)
                {
                    favoriteBtn.Foreground = System.Windows.Media.Brushes.Black;
                    // savedContent.favorites.Remove(msgContent);
                    savedContent.favorites = savedContent.favorites.Where<string>((string content) =>
                    {
                        return content != msgContent;
                    }).ToList<string>();
                }
                else
                {
                    favoriteBtn.Foreground = System.Windows.Media.Brushes.Orange; 
                    savedContent.favorites.Add(msgContent);
                }
            }
            string rawSavedContent = js.Serialize(savedContent);
            File.WriteAllText(saveDataFilePath, rawSavedContent);
        }

        private void UpdatePageHandler(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem pagesSelectorItem = ((ComboBoxItem)(pagesSelector.Items[pagesSelector.SelectedIndex]));
            object pagesSelectorItemData = pagesSelectorItem.DataContext;
            int msgIndex = ((int)(pagesSelectorItemData));
            UpdatePage(msgIndex);
        }

        public void UpdatePage(int msgIndex)
        {
            currentMsgIndex = msgIndex;
            OutputMessages(currentFolder, currentSubFolder);
        }

        public void MessageToggledHandler(object sender, RoutedEventArgs e)
        {
            MessageToggled();
        }

        public void MessageToggled ()
        {
            bool isSelectedCheckBoxFound = false;
            foreach (UIElement messagesItem in messages.Children)
            {
                bool isCheckBox = messagesItem is CheckBox;
                if (isCheckBox)
                {

                    CheckBox checkBox = messagesItem as CheckBox;
                    object checkedState = checkBox.IsChecked;
                    bool isCheckBoxChecked = ((bool)(checkedState));
                    if (isCheckBoxChecked)
                    {
                        isSelectedCheckBoxFound = true;
                        break;
                    }
                }
            }
            if (isSelectedCheckBoxFound)
            {
                actionBar.Visibility = Visibility.Visible;
            }
            else
            {
                actionBar.Visibility = Visibility.Collapsed;
            }
        }

        private void RemoveMessagesHandler(object sender, RoutedEventArgs e)
        {
            RemoveMessages();
        }

        public void RemoveMessages ()
        {
            foreach (UIElement messagesItem in messages.Children)
            {
                bool isCheckBox = messagesItem is CheckBox;
                if (isCheckBox)
                {

                    CheckBox checkBox = messagesItem as CheckBox;
                    object checkedState = checkBox.IsChecked;
                    bool isCheckBoxChecked = ((bool)(checkedState));
                    if (isCheckBoxChecked)
                    {
                        int rowIndex = Grid.GetRow(messagesItem);
                        int messageIndex = rowIndex - 1;
                        object actionBarData = actionBar.DataContext;
                        Message[] messageCollection = ((Message[])(actionBarData));
                        ImapClient mailClient = ImapService.client;
                        CommonFolderCollection mailClientFolders = mailClient.Folders;
                        Folder trashFolder = mailClientFolders.Trash;
                        bool isRemoved = messageCollection[messageIndex].MoveTo(trashFolder);
                        bool isNotRemoved = !isRemoved;
                        if (isNotRemoved)
                        {
                            MessageBox.Show("Не удалось удалить сообщение", "Ошибка");
                        }
                    }
                }
            }
            OutputMessages(currentFolder, currentSubFolder);
        }

        private void RefreshMessagesHandler(object sender, RoutedEventArgs e)
        {
            RefreshMessages();
        }

        public void RefreshMessages()
        {
            OutputMessages(currentFolder, currentSubFolder);
        }

        private void MoveToSpamMessagesHandler(object sender, RoutedEventArgs e)
        {
            MoveToSpamMessages();
        }

        public void MoveToSpamMessages()
        {
            foreach (UIElement messagesItem in messages.Children)
            {
                bool isCheckBox = messagesItem is CheckBox;
                if (isCheckBox)
                {

                    CheckBox checkBox = messagesItem as CheckBox;
                    object checkedState = checkBox.IsChecked;
                    bool isCheckBoxChecked = ((bool)(checkedState));
                    if (isCheckBoxChecked)
                    {
                        int rowIndex = Grid.GetRow(messagesItem);
                        int messageIndex = rowIndex - 1;
                        object actionBarData = actionBar.DataContext;
                        Message[] messageCollection = ((Message[])(actionBarData));
                        ImapClient mailClient = ImapService.client;
                        CommonFolderCollection mailClientFolders = mailClient.Folders;
                        Folder spamFolder = mailClientFolders.Junk;
                        bool isRemoved = messageCollection[messageIndex].MoveTo(spamFolder);
                        bool isNotRemoved = !isRemoved;
                        if (isNotRemoved)
                        {
                            MessageBox.Show("Не удалось переместить сообщение в спам", "Ошибка");
                        }
                    }
                }
            }
            OutputMessages(currentFolder, currentSubFolder);
        }

        private void ArchiveMessagesHandler(object sender, RoutedEventArgs e)
        {
            ArchiveMessages();
        }

        public void ArchiveMessages ()
        {
            foreach (UIElement messagesItem in messages.Children)
            {
                bool isCheckBox = messagesItem is CheckBox;
                if (isCheckBox)
                {

                    CheckBox checkBox = messagesItem as CheckBox;
                    object checkedState = checkBox.IsChecked;
                    bool isCheckBoxChecked = ((bool)(checkedState));
                    if (isCheckBoxChecked)
                    {
                        int rowIndex = Grid.GetRow(messagesItem);
                        int messageIndex = rowIndex - 1;
                        object actionBarData = actionBar.DataContext;
                        Message[] messageCollection = ((Message[])(actionBarData));
                        ImapClient mailClient = ImapService.client;
                        CommonFolderCollection mailClientFolders = mailClient.Folders;
                        Folder archiveFolder = mailClientFolders.Drafts;
                        bool isMoved = messageCollection[messageIndex].MoveTo(archiveFolder);
                        bool isNotMoved = !isMoved;
                        if (isNotMoved)
                        {
                            MessageBox.Show("Не удалось архивировать сообщение", "Ошибка");
                        }
                    }
                }
            }
            OutputMessages(currentFolder, currentSubFolder);
        }

        private void MoveToInboxMessagesHandler(object sender, RoutedEventArgs e)
        {
            MoveToInboxMessages();
        }

        public void MoveToInboxMessages()
        {
            foreach (UIElement messagesItem in messages.Children)
            {
                bool isCheckBox = messagesItem is CheckBox;
                if (isCheckBox)
                {

                    CheckBox checkBox = messagesItem as CheckBox;
                    object checkedState = checkBox.IsChecked;
                    bool isCheckBoxChecked = ((bool)(checkedState));
                    if (isCheckBoxChecked)
                    {
                        int rowIndex = Grid.GetRow(messagesItem);
                        int messageIndex = rowIndex - 1;
                        object actionBarData = actionBar.DataContext;
                        Message[] messageCollection = ((Message[])(actionBarData));
                        ImapClient mailClient = ImapService.client;
                        CommonFolderCollection mailClientFolders = mailClient.Folders;
                        Folder inboxFolder = mailClientFolders.Inbox;
                        bool isMoved = messageCollection[messageIndex].MoveTo(inboxFolder);
                        bool isNotMoved = !isMoved;
                        if (isNotMoved)
                        {
                            MessageBox.Show("Не удалось переместить сообщение во входящие", "Ошибка");
                        }
                    }
                }
            }
            OutputMessages(currentFolder, currentSubFolder);
        }

        private void MoveToImportantMessagesHandler(object sender, RoutedEventArgs e)
        {
            MoveToImportantMessages();
        }

        public void MoveToImportantMessages()
        {
            foreach (UIElement messagesItem in messages.Children)
            {
                bool isCheckBox = messagesItem is CheckBox;
                if (isCheckBox)
                {

                    CheckBox checkBox = messagesItem as CheckBox;
                    object checkedState = checkBox.IsChecked;
                    bool isCheckBoxChecked = ((bool)(checkedState));
                    if (isCheckBoxChecked)
                    {
                        int rowIndex = Grid.GetRow(messagesItem);
                        int messageIndex = rowIndex - 1;
                        object actionBarData = actionBar.DataContext;
                        Message[] messageCollection = ((Message[])(actionBarData));
                        ImapClient mailClient = ImapService.client;
                        CommonFolderCollection mailClientFolders = mailClient.Folders;
                        Folder importantFolder = mailClientFolders.Important;
                        bool isMoved = messageCollection[messageIndex].MoveTo(importantFolder);
                        bool isNotMoved = !isMoved;
                        if (isNotMoved)
                        {
                            MessageBox.Show("Не удалось переместить сообщение в важные", "Ошибка");
                        }
                    }
                }
            }
            OutputMessages(currentFolder, currentSubFolder);
        }

        private void SmartSelectMessagesHandler(object sender, SelectionChangedEventArgs e)
        {
            ComboBox smartSelector = ((ComboBox)(sender));
            int selectedIndex = smartSelector.SelectedIndex;
            SmartSelectMessages(selectedIndex);
        }

        public void SmartSelectMessages (int selectedIndex)
        {
            bool isAllMessages = selectedIndex == 0;
            bool isNoOneMessages = selectedIndex == 1;
            if (isAllMessages)
            {
                foreach (UIElement messagesItem in messages.Children)
                {
                    bool isCheckBox = messagesItem is CheckBox;
                    if (isCheckBox)
                    {
                        CheckBox checkBox = messagesItem as CheckBox;
                        checkBox.IsChecked = true;
                    }
                }
            }
            else if (isNoOneMessages)
            {
                foreach (UIElement messagesItem in messages.Children)
                {
                    bool isCheckBox = messagesItem is CheckBox;
                    if (isCheckBox)
                    {
                        CheckBox checkBox = messagesItem as CheckBox;
                        checkBox.IsChecked = false;
                    }
                }
            }
            MessageToggled();
        }

        private void OpenMoveToMessagesHandler(object sender, RoutedEventArgs e)
        {
            Button btn = ((Button)(sender));
            OpenMoveToMessages(btn);
        }

        public void OpenMoveToMessages(Button btn)
        {
            moveToMessagesPopup.IsOpen = true;
        }

        public void HideMoveToMessagesPopup ()
        {
            moveToMessagesPopup.IsOpen = false;
        }

        private void MoveToSpamMessagesFromMenuHandler(object sender, MouseButtonEventArgs e)
        {
            MoveToSpamMessages();
            HideMoveToMessagesPopup();
        }

        private void MoveToTrashMessagesFromMenuHandler(object sender, MouseButtonEventArgs e)
        {
            RemoveMessages();
            HideMoveToMessagesPopup();
        }

        private void FilterMoveToMessagesItemsHandler (object sender, TextChangedEventArgs e)
        {
            FilterMoveToMessagesItems();
        }

        private void FilterMoveToMessagesItems ()
        {
            string moveToMessagesPopupFilterBoxTextContent = moveToMessagesPopupFilterBox.Text;
            string moveToMessagesPopupFilterBoxIgnoreCaseTextContent = moveToMessagesPopupFilterBoxTextContent.ToLower();
            string spamLabelContent = "Спам";
            string spamLabelIgnoreCaseContent = spamLabelContent.ToLower();
            bool isSpamMatch = spamLabelIgnoreCaseContent.Contains(moveToMessagesPopupFilterBoxIgnoreCaseTextContent);
            if (isSpamMatch)
            {
                moveToMessagesPopupSpamItem.Visibility = Visibility.Visible;
            }
            else
            {
                moveToMessagesPopupSpamItem.Visibility = Visibility.Collapsed;
            }
            string trashLabelContent = "Корзина";
            string trashLabelIgnoreCaseContent = trashLabelContent.ToLower();
            bool isTrashMatch = trashLabelIgnoreCaseContent.Contains(moveToMessagesPopupFilterBoxIgnoreCaseTextContent);
            if (isTrashMatch)
            {
                moveToMessagesPopupTrashItem.Visibility = Visibility.Visible;
            }
            else
            {
                moveToMessagesPopupTrashItem.Visibility = Visibility.Collapsed;
            }
        }

    }

    public class SavedContent
    {
        public List<string> favorites;
    }

}
