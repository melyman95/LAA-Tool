using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace Large_Address_Aware__DotNETFramework_
{
    public class CustomMessageBox : Form
    {
        private Label messageLabel;
        private Button okButton;
        private int _width;
        private int _height;

        public CustomMessageBox(string message, string title, int height, int width)
        {
            this._width = width;
            this._height = height;
            ClientSize = new System.Drawing.Size(_width, _height);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;

            messageLabel = new Label()
            {
                AutoSize = true,
                Text = message,
                Location = new Point(80, 20),
                Size = new Size(250, 50)
            };

            okButton = new Button()
            {
                Text = "Continue",
                DialogResult = DialogResult.OK,
                Location = new Point((ClientSize.Width - 80) / 2, 90),
                Size = new Size(80, 30)
            };

            Controls.Add(okButton);
            Controls.Add(messageLabel);
        }

        public void setMessageLabel(string msg)
        {
            this.messageLabel.Text = msg;
        }

        public new DialogResult Show(string message, string title, int _width, int _height)
        {
            using (var box = new CustomMessageBox(message, title, _width, _height))
            {
                return box.ShowDialog();
            }
        }
    };
}
