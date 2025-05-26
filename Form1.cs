using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Large_Address_Aware__DotNETFramework_
{
    public partial class MainForm: Form
    {
        const int IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x20;
        const int PE_HEADER_OFFSET_LOCATION = 0x3C;
        const int CHARACTERISTICS_OFFSET = 0x16; // Offset in PE header
        const int MACHINE_TYPE_OFFSET = 0x4; // Offset in PE header where machine type is stored
        string initialDirectory = "";
        string backupDirectory = "";

        public MainForm()
        {
            InitializeComponent();

            filePathBox.AllowDrop = true;
            filePathBox.Enabled = false;
            restoreButton.Enabled = false;
        }

        private String searchForBackup(String path)
        {
            List<String> files = new List<String>(Directory.GetFileSystemEntries(path));
            String returnPath = null;

            foreach (var item in files)
            {
                if (Path.GetExtension(Path.GetFileName(item)) == ".BACK")
                {
                    returnPath = item;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return returnPath;
        }

        private void createBackup(String filePath)
        {
            String newFilePath = Path.ChangeExtension(filePath, ".BACK");
            if (!File.Exists(newFilePath))
            {
                File.Copy(filePath, newFilePath);
            }
            backupDirectory = newFilePath;
        }

        private void deleteBackup(String filePath)
        {
            File.Delete(filePath);
        }

        private void restoreFromBackup(String filePath)
        {
            String path = searchForBackup(Path.GetDirectoryName(filePath));
            String newPath = Path.ChangeExtension(path, ".exe");
            
            if (!String.IsNullOrEmpty(path))
            {
                File.Copy(path, newPath, overwrite: true);
                if (File.Exists(newPath))
                {
                    SystemSounds.Asterisk.Play();
                    MessageBox.Show(Path.GetFileNameWithoutExtension(path) + " restored successfully!", "Success,", MessageBoxButtons.OK);
                }
                else
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show(Path.GetFileNameWithoutExtension(path) + " could not be restored.", "Error,", MessageBoxButtons.OK);
                }
            }
        }

        private bool Is32Bit(ushort machineType)
        {
            bool is32 = false;

            if (machineType == 0x14C)
            {
                is32 = true;
            }

            return is32;
        }

        private bool IsLAA(string path, BinaryReader reader, FileStream stream)
        {
            ushort characteristics = 0;

            if (stream.CanSeek == true)
            {
                // Read PE header location
                stream.Seek(PE_HEADER_OFFSET_LOCATION, SeekOrigin.Begin);
                int PEHeaderOffset = reader.ReadInt32();

                // Seek to the characteristics field
                stream.Seek(PEHeaderOffset + CHARACTERISTICS_OFFSET, SeekOrigin.Begin);
                characteristics = reader.ReadUInt16();
            }

            return (characteristics & IMAGE_FILE_LARGE_ADDRESS_AWARE) != 0;
        }

        private void patchFile(String filePath)
        {
            try
            {
                if (backupCheckbox.Checked)
                {
                    createBackup(filePath);
                }

                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                using (var rdr = new BinaryReader(fs))
                using (var wrtr = new BinaryWriter(fs))

                {

                    if (fs.CanSeek == true)
                    {
                        // Read PE header location
                        fs.Seek(PE_HEADER_OFFSET_LOCATION, SeekOrigin.Begin);
                        int PEHeaderOffset = rdr.ReadInt32();

                        // Seek to the characteristics field
                        fs.Seek(PEHeaderOffset + CHARACTERISTICS_OFFSET, SeekOrigin.Begin);
                        ushort characteristics = rdr.ReadUInt16();

                        // Seek to the Machine Type field
                        fs.Seek(PEHeaderOffset + MACHINE_TYPE_OFFSET, SeekOrigin.Begin);
                        ushort machineType = rdr.ReadUInt16();

                        if (Is32Bit(machineType))
                        {
                            // Modify the Large Address Aware flag
                            characteristics |= IMAGE_FILE_LARGE_ADDRESS_AWARE;

                            // Write the modified flag back
                            fs.Seek(PEHeaderOffset + CHARACTERISTICS_OFFSET, SeekOrigin.Begin);
                            wrtr.Write(characteristics);
                            SystemSounds.Asterisk.Play();
                            String directoryPath = Path.GetDirectoryName(filePath);
                            String folder = Path.GetFileName(directoryPath);
                            FileVersionInfo info = FileVersionInfo.GetVersionInfo(filePath);
                            String description = null;
                            if (!String.IsNullOrEmpty(info.FileDescription))
                            {
                                description = info.FileDescription;
                            }
                            else if (!String.IsNullOrEmpty(info.FileName))
                            {
                                description = info.FileName;
                            }

                            if (IsLAA(filePath, rdr, fs))
                            {
                                SystemSounds.Asterisk.Play();
                                MessageBox.Show(Path.GetFileName(filePathBox.Text) + " should now be LAA.", "Woo-Hoo!", MessageBoxButtons.OK);
                                if (!String.IsNullOrEmpty(backupDirectory))
                                {
                                    restoreButton.Enabled = true;
                                    deleteBackupButton.Enabled = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Something went wrong. Be sure to run this program as administrator. I promise this isn't a virus. JK it's a virus.");
                            }
                            if (restoreButton.Enabled == true)
                            {
                                deleteBackupButton.Enabled = true;
                            }
                            else
                            {
                                deleteBackupButton.Enabled = false;
                            }
                        }
                        else
                        {
                            SystemSounds.Beep.Play();
                            MessageBox.Show("File is not 32 bit. Exiting...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        SystemSounds.Exclamation.Play();
                        MessageBox.Show("Something went wrong. Exiting...");
                        return;
                    }
                }
            }
            catch (System.IO.EndOfStreamException ex)
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show(ex.ToString() + "!\n" + " this probably means you are attempting to use this tool on a 16-bit/DOS exe. Will only work on 32-bit Windows applications.", "Error", MessageBoxButtons.OK);
            }
            //backupDirectory = null;
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (String.IsNullOrEmpty(this.initialDirectory))
            {
                fd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                fd.InitialDirectory = this.initialDirectory;
            }
                fd.Filter = "Executable Files (*.exe)|*.exe";
            fd.Title = "Select an exe";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                filePathBox.Text = fd.FileName;
                this.initialDirectory = filePathBox.Text;
                return;
            }
        }

        private void patchButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(filePathBox.Text.Trim()))
            {
                patchFile(filePathBox.Text.Trim());
            }
            if (restoreButton.Enabled == true)
            {
                deleteBackupButton.Enabled = true;
            }
            else
            {
                deleteBackupButton.Enabled = false;
            }
        }

        private void filePathBox_TextChanged(object sender, EventArgs e)
        {
            string backup = searchForBackup(Path.GetDirectoryName(filePathBox.Text));
            string extension = Path.GetExtension(backup);
            if (String.IsNullOrEmpty(backup))
            {
                return;
            }
            string backupPath = Path.GetFullPath(backup);
            string extension2 = Path.GetExtension(filePathBox.Text);

            if (extension == ".BACK" && extension2 == ".EXE".ToLower())
            {
                restoreButton.Enabled = true;
            }
            else
            {
                restoreButton.Enabled = false;
            }

            if (restoreButton.Enabled == true)
            {
                deleteBackupButton.Enabled = true;
            }
            else
            {
                deleteBackupButton.Enabled = false;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            patchButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            openButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            filePathBox.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;

            if (String.IsNullOrEmpty(filePathBox.Text)) {
                deleteBackupButton.Enabled = false;
            }
            else
            {
                deleteBackupButton.Enabled = true;
            }
        }

        private void filePathBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                filePathBox.Text = e.Data.GetData(DataFormats.Text).ToString();
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                filePathBox.Text = string.Join(Environment.NewLine, files);
            }
        }

        private void restoreButton_Click(object sender, EventArgs e)
        {
            try
            {
                restoreFromBackup(filePathBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("uh oh.\n" + ex.ToString());
            }
        }

        private void deleteBackupButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(backupDirectory))
                {
                    deleteBackup(backupDirectory);
                    SystemSounds.Beep.Play();
                    MessageBox.Show("BACKUP DELETED.");
                }
                if (!File.Exists(backupDirectory)) {
                    restoreButton.Enabled = false;
                }
                else
                {
                    restoreButton.Enabled = true;
                }
                if (restoreButton.Enabled == true)
                {
                    deleteBackupButton.Enabled = true;
                }
                else
                {
                    deleteBackupButton.Enabled = false;
                }
            }
            catch (Exception)
            {
                if (restoreButton.Enabled == true)
                {
                    deleteBackupButton.Enabled = true;
                }
                else
                {
                    deleteBackupButton.Enabled = false;
                }
                return;
            }
        }
    }
}
