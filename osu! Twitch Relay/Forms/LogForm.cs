using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace osu_Twitch_Relay
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }
        public void Write(string contents, int errorLevel = 0)
        {
            Font fnt = new Font("Segoe UI", 8, FontStyle.Regular);
            if (this.richTextBox1.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    richTextBox1.SelectionFont = fnt;
                    switch (errorLevel)
                    {
                        case 0:
                            richTextBox1.SelectionColor = Color.White;
                            break;
                        case 1:
                            richTextBox1.SelectionColor = Color.Green;
                            break;
                        case 2:
                            richTextBox1.SelectionColor = Color.Yellow;
                            break;
                        case 3:
                            richTextBox1.SelectionColor = Color.Red;
                            break;
                    }
                    richTextBox1.SelectedText = System.DateTime.Now.ToString() + " - " + contents + "\n";
                });
            }
            else
            {
                richTextBox1.SelectionFont = fnt;
                switch (errorLevel)
                {
                    case 0:
                        richTextBox1.SelectionColor = Color.White;
                        break;
                    case 1:
                        richTextBox1.SelectionColor = Color.Green;
                        break;
                    case 2:
                        richTextBox1.SelectionColor = Color.Yellow;
                        break;
                    case 3:
                        richTextBox1.SelectionColor = Color.Red;
                        break;
                }
                richTextBox1.SelectedText = System.DateTime.Now.ToString() + " - " + contents + "\n";
            }
           
        }

        private void LogForm_LocationChanged(object sender, EventArgs e)
        {
            if (this.Owner !=null)
                this.Owner.Location = new Point(this.Location.X - this.Owner.Size.Width, this.Location.Y);  
        }

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        bool moving = false;
        Point offset = Point.Empty;
        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            moving = true;
            offset = new Point(Cursor.Position.X - this.Location.X,Cursor.Position.Y - this.Location.Y);
        }

        private void richTextBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (moving == true)
            {
                this.Location = new Point(Cursor.Position.X - offset.X, Cursor.Position.Y - offset.Y);
            }
        }

        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
        }

        private void LogForm_Load(object sender, EventArgs e)
        {

        }
    }
    public class FocuslessRTB : RichTextBox
    {
        public FocuslessRTB()
        {
            this.ReadOnly = true;
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg != 0x7)
            {
                base.WndProc(ref m);
            }
        }
    }
}
