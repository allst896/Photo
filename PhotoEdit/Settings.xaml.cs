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
using System.IO;

namespace PhotoEdit
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string evoid = "Settings:Window_Loaded";

            try
            {
                this.txtEmailToAddress.Text = Properties.Settings.Default.EmailTo;
                this.txtEmailFromAddress.Text = Properties.Settings.Default.EmailFrom;
                this.txtEmailSubject.Text = Properties.Settings.Default.EmailSubject;

                this.txtToFolder.Text = Properties.Settings.Default.RetrieveFolder;
                this.txtFromFolder.Text = Properties.Settings.Default.DestinationFolder;
                this.txtFolderName.Text = Properties.Settings.Default.FolderNameFormat;
            }
            catch (Exception ex)
            {
                
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            string evoid = "Settings:cmdSave_Click";

            try
            {
                Properties.Settings.Default.EmailTo = txtEmailToAddress.Text;
                Properties.Settings.Default.EmailFrom = txtEmailFromAddress.Text;
                Properties.Settings.Default.EmailSubject = txtEmailSubject.Text;

                Properties.Settings.Default.DestinationFolder = txtToFolder.Text;
                Properties.Settings.Default.RetrieveFolder = txtFromFolder.Text;
                Properties.Settings.Default.FolderNameFormat = txtFolderName.Text;

                Properties.Settings.Default.Save();

                MessageBox.Show("Settings have been saved.", "Success", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
