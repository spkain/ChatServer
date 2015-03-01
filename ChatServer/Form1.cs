﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Windows.Forms;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        private TcpListener listner;
        private TcpClient client;
        private OptionLoader loader;
        private bool isActive = false;
        public Form1()
        {
            InitializeComponent();
            UpdateStatusLabel("未実行");
            ButtonInitEnabled();
            this.loader = new OptionLoader();
            this.listner = new TcpListener(IPAddress.Any, Int32.Parse(this.loader["localport"]));
        }

        private void ButtonInitEnabled()
        {
            this.button1.Enabled = true;
            this.button2.Enabled = false;
        }

        private void UpdateStatusLabel(string text)
        {
            this.StatusLabel.Text = text;
        }

        private void ListenerWait()
        {
            isActive = true;
            UpdateStatusLabel("待機中");
            this.listner.Start();
            while (isActive)
            {
                if (this.listner.Pending())
                {
                    this.client = this.listner.AcceptTcpClient();
                    UpdateStatusLabel("接続中");
                    //this.isExistsClient = true;
                    RecvAction();
                }
                SysWait();
            }
        }

        private void RecvAction()
        {
            //while (this.isExistsClient)
            //{
            //}

        }

        private void SysWait()
        {
            Thread.Sleep(300);
            Application.DoEvents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeButtonEnabler();
            ListenerWait();

        }

        private void ChangeButtonEnabler()
        {
            button1.Enabled = !button1.Enabled;
            button2.Enabled = !button2.Enabled;
        }
        private void ActiveStatusChange()
        {
            this.isActive = !this.isActive;
            if (this.listner != null)
                this.listner.Stop();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ActiveStatusChange();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChangeButtonEnabler();
            ActiveStatusChange();
            UpdateStatusLabel("未実行");
        }
    }
}