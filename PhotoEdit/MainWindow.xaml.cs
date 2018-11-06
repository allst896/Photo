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
using System.IO;
using System.Net.Mail;
using System.Reflection;

namespace PhotoEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SettingsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Settings ssettings = new Settings();
                ssettings.Show();
            }
            catch (Exception ex)
            {

            }
        }

        private void cmdBrowseCurrentFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    txtCurrentFolder.Text = dialog.SelectedPath;
                    ListDirectory(tvPictures, dialog.SelectedPath);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void cmdBrowseDestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    txtDestinationFolder.Text = dialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void SelectionChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            try
            {
                TreeViewItem titem = tvPictures.SelectedItem as TreeViewItem;
                string filepath = titem.Tag.ToString();
                ImageSource imageSource = new BitmapImage(new Uri(filepath));
                imViewer.Source = imageSource;
            }
            catch (Exception ex)
            {

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dtpDateTaken.SelectedDate = DateTime.Today;

                string defaultPath = Properties.Settings.Default.RetrieveFolder.ToString();
                txtCurrentFolder.Text = defaultPath;
                ListDirectory(tvPictures, defaultPath);
                defaultPath = Properties.Settings.Default.DestinationFolder.ToString();
                txtDestinationFolder.Text = defaultPath;
                txtSaveFileFormat.Text = dtpDateTaken.SelectedDate.Value.ToString("yyyyMMdd");
            }
            catch (Exception ex)
            {

            }
        }

        private void dtpDateTaken_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                txtSaveFileFormat.Text = dtpDateTaken.SelectedDate.Value.ToString("yyyyMMdd");
            }
            catch (Exception ex)
            {

            }
        }

        private void cmdOpenEmail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //open outlook with to, from, subject and attachment
                //create an email
                MailMessage message = new MailMessage();
                message.From = new MailAddress("your@email.com", "MyCompany");
                message.Subject = "Email subject goes here";
                message.IsBodyHtml = true;
                message.Body = "<html><head></head><body><font size=\"3\" color=\"#ff0000\">HTML formatted email body.</body></html>";

                //add the attachments
                message.Attachments.Add(new Attachment("PhotoEdit.pdb"));

                //get the message as byte array
                byte[] bin = getEmailAsEML(message);

                //save byte array to file
                string path = "C://Temp/mymessage.eml";
                File.WriteAllBytes(path, bin);

                //send the email to the client as eml
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception ex)
            {

            }
        }

        public byte[] getEmailAsEML(MailMessage message)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var binaryWriter = new BinaryWriter(ms);
                //Write the Unsent header to the file so the mail client knows this mail must be presented in "New message" mode
                binaryWriter.Write(Encoding.UTF8.GetBytes("X-Unsent: 1" + Environment.NewLine));

                var assembly = typeof(SmtpClient).Assembly;
                var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

                // Get reflection info for MailWriter contructor
                var mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);

                // Construct MailWriter object with our FileStream
                var mailWriter = mailWriterContructor.Invoke(new object[] { ms });

                // Get reflection info for Send() method on MailMessage
                var sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

                sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { mailWriter, true, true }, null);

                // Finally get reflection info for Close() method on our MailWriter
                var closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

                // Call close method
                closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);

                return ms.ToArray();
            }
        }

        private void cmdSaveFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string foldername = txtDestinationFolder.Text;
                string filename = "";
                TreeViewItem titem = tvPictures.SelectedItem as TreeViewItem;
                string filepath = titem.Tag.ToString();

                if (txtSaveFileFormat.Text != null && txtSaveFileFormat.Text != "")
                {
                    if (!(Directory.Exists(txtDestinationFolder.Text + "\\" + txtSaveFileFormat.Text)))
                    {
                        Directory.CreateDirectory(txtDestinationFolder.Text + "\\" + txtSaveFileFormat.Text);
                    }
                    foldername += "\\" + txtSaveFileFormat.Text;
                    filename = txtSaveFileFormat.Text;
                }

                if (cboFilename.SelectedValue != null)
                {
                    filename += cboFilename.SelectedValue.ToString();
                }
                filename += "." + filepath.Substring(filepath.IndexOf('.')).ToLower();
            }
            catch (Exception ex)
            {

            }
        }

        private static void ListDirectory(TreeView treeView, string path)
        {
            try
            {
                List<string> picExt = new List<string>();
                picExt = GetFileTypes();

                treeView.Items.Clear();

                foreach (string s in Directory.GetFiles(path))
                {
                    if (picExt.Contains(s.Substring(s.IndexOf('.')).ToLower()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        treeView.Items.Add(subitem);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static List<string> GetFileTypes()
        {
            List<string> returnList = new List<string>();

            try
            {
                string[] defaulttypes = Properties.Settings.Default.FileTypes.Split(',');
                foreach (string s in defaulttypes)
                {
                    returnList.Add(s);
                }
            }
            catch (Exception ex)
            {

            }
            return returnList;
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand Settings = new RoutedUICommand
            (
                "Settings",
                "Settings",
                typeof(CustomCommands)
            );
    }
}
