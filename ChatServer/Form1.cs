using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Windows.Forms;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        private ServerControl server;

        public Form1()
        {
            InitializeComponent();
            UpdateStatusLabel("未実行");
            ButtonInitEnabled();

            this.server = new ServerControl(this);
        }

        private void ButtonInitEnabled()
        {
            this.button1.Enabled = true;
            this.button2.Enabled = false;
        }

        public void UpdateStatusLabel(string text)
        {
            this.StatusLabel.Text = text;
        }

        public void AppendText(string mes)
        {
            textBox1.Text += mes;
        }

        public void SysWait()
        {
            Thread.Sleep(100);
            Application.DoEvents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            ChangeButtonEnabler();
            server.ListenerWait();
        }

        private void ChangeButtonEnabler()
        {
            button1.Enabled = !button1.Enabled;
            button2.Enabled = !button2.Enabled;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            server.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChangeButtonEnabler();
            server.Close();
            UpdateStatusLabel("未実行");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }
    }
}
