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
using Win32Mapi;

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
            string evoid = "MainWindow:SettingsCommand_Executed";
            try
            {
                Settings ssettings = new Settings();
                ssettings.Show();
            }
            catch (Exception ex)
            {
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\PhotoEdit_Error.log", "(" + DateTime.Now.ToString() + ") " + evoid + ": " + ex.Message.ToString() + Environment.NewLine);
                MessageBox.Show("An error has occurred. Please see error log for details.", "Error", MessageBoxButton.OK);
            }
        }

        private void cmdBrowseCurrentFolder_Click(object sender, RoutedEventArgs e)
        {
            string evoid = "MainWindow:cmdBrowseCurrentFolder_Click";
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
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\PhotoEdit_Error.log", "(" + DateTime.Now.ToString() + ") " + evoid + ": " + ex.Message.ToString() + Environment.NewLine);
                MessageBox.Show("An error has occurred. Please see error log for details.", "Error", MessageBoxButton.OK);
            }
        }

        private void cmdBrowseDestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            string evoid = "MainWindow:cmdBrowseDestinationFolder_Click";

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
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\PhotoEdit_Error.log", "(" + DateTime.Now.ToString() + ") " + evoid + ": " + ex.Message.ToString() + Environment.NewLine);
                MessageBox.Show("An error has occurred. Please see error log for details.", "Error", MessageBoxButton.OK);
            }
        }
        private void SelectionChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            string evoid = "MainWindow:SelectionChanged";
            try
            {
                bool anyselected = false;
                foreach (TreeViewItem t in tvPictures.Items)
                {
                    if (t.IsSelected == true)
                    {
                        anyselected = true;
                        break;
                    }
                }

                if (anyselected == true)
                {
                    TreeViewItem titem = tvPictures.SelectedItem as TreeViewItem;
                    string filepath = titem.Tag.ToString();
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.UriSource = new Uri(filepath);
                    bi.EndInit();
                    ImageSource imageSource = bi;
                    imViewer.Source = imageSource;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\PhotoEdit_Error.log", "(" + DateTime.Now.ToString() + ") " + evoid + ": " + ex.Message.ToString() + Environment.NewLine);
                MessageBox.Show("An error has occurred. Please see error log for details.", "Error", MessageBoxButton.OK);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string evoid = "MainWindow:Window_Loaded";

            try
            {
                dtpDateTaken.SelectedDate = DateTime.Today;

                string defaultPath = Properties.Settings.Default.RetrieveFolder.ToString();
                txtCurrentFolder.Text = defaultPath;
                ListDirectory(tvPictures, defaultPath);
                defaultPath = Properties.Settings.Default.DestinationFolder.ToString();
                txtDestinationFolder.Text = defaultPath;
                txtSaveFileFormat.Text = dtpDateTaken.SelectedDate.Value.ToString("yyyyMMdd");

                if (File.Exists(Properties.Settings.Default.FileNameFile.ToString()))
                {
                    string line = "";
                    StreamReader sr = new StreamReader(Properties.Settings.Default.FileNameFile.ToString());
                    while ((line=sr.ReadLine()) != null)
                    {
                        cboFilename.Items.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\PhotoEdit_Error.log", "(" + DateTime.Now.ToString() + ") " + evoid + ": " + ex.Message.ToString() + Environment.NewLine);
                MessageBox.Show("An error has occurred. Please see error log for details.", "Error", MessageBoxButton.OK);
            }
        }

        private void dtpDateTaken_Changed(object sender, RoutedEventArgs e)
        {
            string evoid = "MainWindow:dtpDateTaken_Changed";

            try
            {
                txtSaveFileFormat.Text = dtpDateTaken.SelectedDate.Value.ToString("yyyyMMdd");
            }
            catch (Exception ex)
            {
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\PhotoEdit_Error.log", "(" + DateTime.Now.ToString() + ") " + evoid + ": " + ex.Message.ToString() + Environment.NewLine);
                MessageBox.Show("An error has occurred. Please see error log for details.", "Error", MessageBoxButton.OK);
            }
        }

        private void cmdOpenEmail_Click(object sender, RoutedEventArgs e)
        {
            string evoid = "MainWindow:OpenEmail_Click";

            try
            {
                string toaddress = Properties.Settings.Default.EmailTo.ToString();
                string fromaddress = Properties.Settings.Default.EmailFrom.ToString();
                string subject = Properties.Settings.Default.EmailSubject.ToString();
                if (txtDestinationFolder.Text.Substring(txtDestinationFolder.Text.Length - 1, 1) == "\\") { txtDestinationFolder.Text = txtDestinationFolder.Text.Substring(0, txtDestinationFolder.Text.Length - 1); }
                string foldername = txtDestinationFolder.Text;
                if (txtSaveFileFormat.Text != null && txtSaveFileFormat.Text != "")
                {
                    foldername += "\\" + txtSaveFileFormat.Text;
                }

                var mapi = new SimpleMapi();
                if (toaddress != "") { mapi.AddRecipient(name: toaddress, addr: null, cc: false); }
                if (fromaddress != "") { mapi.SetSender(fromaddress, senderAddress: null); }
                foreach (string s in Directory.GetFiles(foldername))
                {
                    mapi.Attach(s);
                }
                mapi.Send(subject, "Please see the attached photos.", true);
            }
            catch (Exception ex)
            {
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\PhotoEdit_Error.log", "(" + DateTime.Now.ToString() + ") " + evoid + ": " + ex.Message.ToString() + Environment.NewLine);
                MessageBox.Show("An error has occurred. Please see error log for details.", "Error", MessageBoxButton.OK);
            }
        }

        private void cmdSaveFile_Click(object sender, RoutedEventArgs e)
        {
            string evoid = "MainWindow:cmdSaveFile_Click";

            try
            {
                TreeViewItem titem = tvPictures.SelectedItem as TreeViewItem;
                
                if (titem == null) { MessageBox.Show("You must choose a file to rename.", "Missing File", MessageBoxButton.OK); }
                else
                {
                    if ((txtSaveFileFormat.Text == null || txtSaveFileFormat.Text == "") && (cboFilename.Text == null || cboFilename.Text == ""))
                    {
                        MessageBox.Show("You must have a file name.", "Missing File Name", MessageBoxButton.OK);
                    }
                    else
                    {
                        if (txtDestinationFolder.Text.Substring(txtDestinationFolder.Text.Length - 1, 1) == "\\") { txtDestinationFolder.Text = txtDestinationFolder.Text.Substring(0, txtDestinationFolder.Text.Length - 1); }

                        string foldername = txtDestinationFolder.Text;
                        string filename = "";
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
                            filename += "_" + cboFilename.SelectedValue.ToString();
                        }
                        if (txtExtraFileName.Text != "") { filename += "_" + txtExtraFileName.Text; }
                        filename += filepath.Substring(filepath.IndexOf('.')).ToLower();

                        File.Copy(filepath, foldername + "\\" + filename);

                        if (chkKeepSourceFile.IsChecked == false)
                        {
                            imViewer.Source = null;
                            titem.IsSelected = false;
                            tvPictures.Items.Remove(titem);
                            File.Delete(filepath);
                        }

                        MessageBox.Show("File successfully saved.", "Success", MessageBoxButton.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\PhotoEdit_Error.log", "(" + DateTime.Now.ToString() + ") " + evoid + ": " + ex.Message.ToString() + Environment.NewLine);
                MessageBox.Show("An error has occurred. Please see error log for details.", "Error", MessageBoxButton.OK);
            }
        }

        private static void ListDirectory(TreeView treeView, string path)
        {
            string evoid = "MainWindow:ListDirectory";

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
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\PhotoEdit_Error.log", "(" + DateTime.Now.ToString() + ") " + evoid + ": " + ex.Message.ToString() + Environment.NewLine);
                MessageBox.Show("An error has occurred. Please see error log for details.", "Error", MessageBoxButton.OK);
            }
        }

        public static List<string> GetFileTypes()
        {
            string evoid = "MainWindow:GetFileTypes";
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
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\PhotoEdit_Error.log", "(" + DateTime.Now.ToString() + ") " + evoid + ": " + ex.Message.ToString() + Environment.NewLine);
                MessageBox.Show("An error has occurred. Please see error log for details.", "Error", MessageBoxButton.OK);
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
