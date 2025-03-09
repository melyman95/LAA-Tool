using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
//using System.Linq;
using System.Media;
using System.Text;
//using System.Threading.Tasks;
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
        String initialDirectory = "";

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

        private bool IsLAA()
        {
            bool laa = false;

            return laa;
        }

        private void patchFile(String filePath)
        {
            try
            {
                if (backupCheckbox.Checked)
                {
                    createBackup(filePath);
                }

                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                BinaryReader rdr = new BinaryReader(fs);
                BinaryWriter wrtr = new BinaryWriter(fs);

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
                        MessageBox.Show(Path.GetFileName(filePathBox.Text) + " (" + folder + ")" + " should now be LAA! :) smileyface", "Congraturlations", MessageBoxButtons.OK);
                    }
                    else
                    {
                        SystemSounds.Exclamation.Play();
                        MessageBox.Show("File is not 32 bit. Exiting...", "Error", MessageBoxButtons.OK);
                        return;
                    }
                }
                else
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("Something went wrong. Exiting...");
                    return;
                }
            }
            catch (System.IO.EndOfStreamException ex)
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show(ex.ToString() + "!\n" + " this probably means you are attempting to use this tool on a 16-bit/DOS exe. Will only work on 32-bit Windows applications.", "Error", MessageBoxButtons.OK);
            }
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
        }

        private void filePathBox_TextChanged(object sender, EventArgs e)
        {
            String backup = searchForBackup(Path.GetDirectoryName(filePathBox.Text));
            String extension = Path.GetExtension(backup);
            if (extension == ".BACK")
            {
                restoreButton.Enabled = true;
                return;
            }
            else
            {
                restoreButton.Enabled = false;
                return;
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
    }
}
