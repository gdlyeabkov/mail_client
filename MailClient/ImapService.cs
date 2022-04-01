using ImapX;
using ImapX.Collections;
using ImapX.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MailClient
{
    internal class ImapService
    {
        public static ImapClient client
        {
            get;
            set;
        }
        public static BackgroundWorker worker;

        public static void Initialize()
        {
            client = new ImapClient("imap.gmail.com", true);
            if (!client.Connect())
            {
                throw new Exception("Error connecting to the client.");
            }
            worker = new BackgroundWorker();
            worker.DoWork += DownloadFolderHandler;
        }

        /*public static bool Login(string u, string p)
        {
            returnclient.Login(u, p);
        }*/
        public static void Logout()
        {
            // Remove the login value from the client.  
            if (client.IsAuthenticated)
            {
                client.Logout();
            }
            // Clear the logged in value.  
            // MainWindow.LoggedIn = false;
        }
        
        public static List<Folder> GetFolders(SpeechSynthesizer debugger)
        {
            var folders = new List<Folder>();
            foreach (Folder folder in client.Folders)
            {
                folders.Add(folder);
            }
            // Before returning start the idling  

            Login("glebdyakov2000@gmail.com", "ttolpqpdzbigrkhz");
            if (client.IsAuthenticated)
            {
                // client.Folders.Inbox.StartIdling(); // And continue to listen for more.  
                client.Folders.Inbox.OnNewMessagesArrived += Inbox_OnNewMessagesArrived;
            }
            return folders;
        }

        private static void Inbox_OnNewMessagesArrived(object sender, IdleEventArgs e)
        {
            // Show a dialog  
            MessageBox.Show("A new message was downloaded in {e.Folder.Name} folder.");
        }

        public static bool Login(string u, string p)
        {
            return client.Login(u, p);
        }


        public static Message[] MessageCollectionGetMessagesForFolder(string name, int subFolderIndex = 1)
        {
            var clientFolders = client.Folders;
            var messages = clientFolders["INBOX"].Search("ALL", MessageFetchMode.ClientDefault, 5);
            bool isGmailFolder = name == "[Gmail]";
            if (isGmailFolder)
            {
                var gmailFolder = clientFolders["[Gmail]"];
                var gmailSubFolders = gmailFolder.SubFolders;
                Folder subFolder = gmailSubFolders[subFolderIndex];
                messages = subFolder.Search("ALL", MessageFetchMode.ClientDefault, 5);
            }
            return messages;
        }

        public static void DownloadFolderHandler(object sender, DoWorkEventArgs e)
        {
            if (true)
            {
                client.Folders["INBOX"].Messages.Download();
            }
        }



    }

    class EmailFolder
    {

        public string title = "";
        public List<Folder> subfolders;

        public EmailFolder(string title, FolderCollection subfolders)
        {
            this.title = title;
            this.subfolders = subfolders.ToList();
        }

    }

}
