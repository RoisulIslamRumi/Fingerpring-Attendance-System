using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net;

namespace Fp_attendance
{
    public partial class Form1 : Form
    {
        private SerialPort myPort;
        private string name, clas, roll, fid;

        public Form1()
        {
            InitializeComponent();
            availablePorts();
        }

        private void availablePorts()
        {
            var ports = SerialPort.GetPortNames();
            comboBox1.DataSource = ports;
        }

        void connectFingerprint()
        {
            try
            {
                myPort = new SerialPort();
                myPort.PortName = comboBox1.Text;
                myPort.BaudRate = Int32.Parse("9600");
                myPort.Open();
                myPort.Parity = Parity.None;
                myPort.StopBits = StopBits.One;
                myPort.DataBits = 8;
                myPort.DataReceived += myPort_DataReceived;
                richTextBox2.Text = "Fingerprint Connected";
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Unauthorised Access");
            }
        }

        public void myPort_DataReceived(object sender, EventArgs e)
        {
            String datum = myPort.ReadLine();
            this.BeginInvoke(new LineReceivedEvent(LineReceived), datum);
        }

        private delegate void LineReceivedEvent(string line);

        
        private void LineReceived(string line)
        {
            richTextBox1.Text += line;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            connectFingerprint();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            disconnectFingerprint();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            disconnectFingerprint();
            connectFingerprint();
            richTextBox2.Text = "Check Mode On";
            myPort.Write("check");
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
            clas = textBox2.Text;
            roll = textBox3.Text;
            fid = textBox4.Text;

            int id = Convert.ToInt32(textBox4.Text);
            myPort.Write("enroll" + id);
            myPort.Write("x");
            richTextBox2.Text = "FingerID Verifying";
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            WebClient wc2 = new WebClient();
            var data = wc2.DownloadString("http://api.bracumongoltori.com/fingerprint.php?action=student&name=" + name + "&class=" + clas + "&roll=" + roll + "&finger=" + fid);
            richTextBox2.Text = data;
            myPort.Write("done");
        }

        public void disconnectFingerprint()
        {
            if (myPort.IsOpen)
            {
                myPort.Close();
                richTextBox2.Text = "Fingerprint Disconnected";
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }
            myPort.Dispose();
        }

    }
}
