using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Windows.Forms;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        // XXX 999 always not focus memo area
        // XXX 999 error message
        // XXX 999 error action
        private readonly string CrLf = "\r\n";
        private readonly int DefaultReceiveTimeoutSec = 100;
        private bool TimeOutOption = false;
        private TcpListener listener;
        private Socket client;
        private OptionLoader loader;
        private bool isActive = false;
        private bool isExistsClient = false;
        private int timeoutcount = 0;

        System.Text.StringBuilder sPrevString = new System.Text.StringBuilder();

        public Form1()
        {
            InitializeComponent();
            UpdateStatusLabel("未実行");
            ButtonInitEnabled();
            this.loader = new OptionLoader();

            this.listener = new TcpListener(IPAddress.Any, Int32.Parse(this.loader["localport"]));
        }

        private void AllFlagReset()
        {
            this.isActive = false;
            this.isExistsClient = false;
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
            this.listener.Start();
            while (isActive)
            {
                if (this.listener.Pending())
                {
                    this.client = this.listener.AcceptSocket();
                    UpdateStatusLabel("接続中");
                    this.isExistsClient = true;
                    RecvAction(0);
                }
                SysWait();
            }
        }

        private void Information(Socket client)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes("welcome easy chat server..." + CrLf + CrLf);
            client.Send(buffer);
        }

        private bool TimeWaitCheck(int timeoutcount)
        {
            // TODO SocketException PEND
            if (timeoutcount == 30)
            {
                textBox1.Text += "一定時間が経過したため強制的にはずします";
                return true;
            }

            return false;
        }

        private bool TimeOut(int timecount)
        {
            if (TimeOutOption && TimeWaitCheck(timecount))
            {
                this.isExistsClient = false;
                this.client.Shutdown(SocketShutdown.Both);
                this.client.Close();
                UpdateStatusLabel("待機中");
                return true;
            }
            return false;
        }

        private bool EndLines(string recvString)
        {
            return (recvString.Contains("\n") == false);
        }

        private byte[] getRecvData(out int recvcount)
        {
            recvcount = 0;

            // DONE release byte 
            byte[] recvdata = new byte[512];
            try
            {
                recvcount = this.client.Receive(recvdata);
            }
            catch (SocketException e)
            {
                timeoutcount += 1;
                if (TimeOut(timeoutcount))
                    return null;
            }
            return recvdata;
        }

        private string RecvDataToString(byte[] recvdata)
        {
            string recvString = "";
            // DONE RecvDataToString
            recvString = System.Text.Encoding.UTF8.GetString(recvdata);
            recvString = recvString.Substring(0, recvString.IndexOf('\0'));
            return recvString;
        }

        private void SetOptionTimeout(int timeoutsec)
        {
            if (timeoutsec < 0) timeoutsec = 0;

            // DONE setoption
            if (timeoutsec == 0)
                TimeOutOption = false;
            else
                TimeOutOption = true;
        }

        private void RecvAction(int timeoutsec)
        {
            this.client.ReceiveTimeout = DefaultReceiveTimeoutSec;
            byte[] recvdata = null;
            byte[] returnbytes = System.Text.Encoding.ASCII.GetBytes("\n");
            string recvString = "";

            int recvcount = 0;

            SetOptionTimeout(timeoutsec);

            Information(this.client);

            while (this.isExistsClient)
            {
                recvdata = getRecvData(out recvcount);

                if (recvdata == null)
                    break;

                // TODO 未検討
                if (recvcount == 0)
                {
                    timeoutcount += 1;
                    if (TimeOut(timeoutcount))
                        break;

                    SysWait();
                    continue;
                }

                recvString = RecvDataToString(recvdata);

                sPrevString.Append(recvString);
                //prevString += recvString;
                if (EndLines(recvString))
                {
                    SysWait();
                    continue;
                }

                ChatAction();
                ChatClear();

                recvString = "";
                timeoutcount = 0;

                SysWait();
            }
        }

        private void ChatClear()
        {
            // DONE clear 
            sPrevString.Remove(0, sPrevString.Length);
        }

        private void ChatAction()
        {
            // DONE to Action 
            textBox1.Text += sPrevString.ToString();
            // TODO dispatch detail action

        }

        private void SysWait()
        {
            Thread.Sleep(100);
            Application.DoEvents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            ChangeButtonEnabler();
            ListenerWait();
        }

        private void ChangeButtonEnabler()
        {
            button1.Enabled = !button1.Enabled;
            button2.Enabled = !button2.Enabled;
        }

        private void ListnerStop()
        {
            if (this.client != null)
            {
                //TODO サーバー側の停止アナウンス送付

                // XXX
                // 破棄されたソケットを再度切断しようとしたらObjectDisposedException出たらしい ^Д^
                if (this.client.Connected)
                {
                    try
                    {
                        this.client.Shutdown(SocketShutdown.Both);
                        this.client.Disconnect(false);
                        this.client.Close();
                    }
                    catch (Exception e)
                    {
                        // XXX 999 Inner記録
                    }
                }
                this.client = null;
            }

            if (this.listener != null)
                this.listener.Stop();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            AllFlagReset();
            ListnerStop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChangeButtonEnabler();
            AllFlagReset();
            ListnerStop();
            UpdateStatusLabel("未実行");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }
    }
}
