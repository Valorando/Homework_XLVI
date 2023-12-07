using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Homework_06_12_VI
{
    public partial class Form1 : Form
    {
        private Socket c;

        public Form1()
        {
            InitializeComponent();
            InitializeClient();
        }

        private void InitializeClient()
        {
            c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            textBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
        }

        private async Task ConnectToServer()
        {
            try
            {
                IPAddress serverIp = IPAddress.Parse(textBox2.Text);
                int serverPort = Convert.ToInt32(textBox3.Text);

                await c.ConnectAsync(new IPEndPoint(serverIp, serverPort));

                listBox1.Items.Add("[Собеседник подключился]");

                textBox1.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;

                while (true)
                {
                    byte[] data = new byte[256];
                    int bytesRead = await c.ReceiveAsync(new ArraySegment<byte>(data), SocketFlags.None);
                    string message = Encoding.UTF8.GetString(data, 0, bytesRead);

                    if (string.IsNullOrEmpty(message))
                    {
                        listBox1.Items.Add("[Сервер не отвечает]");
                        InitializeClient();
                    }

                    listBox1.Items.Add($"[Собеседник]: {message}");

                    if (message == "Bye")
                    {
                        c.Shutdown(SocketShutdown.Both);
                        c.Close();
                        listBox1.Items.Add("[Соединение разорвано]");
                        InitializeClient();
                    }
                }
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
                InitializeClient();
            }
        }

        private async Task SendMessage()
        {
            try
            {
                string message = textBox1.Text;
                byte[] data = Encoding.UTF8.GetBytes(message);
                await c.SendAsync(new ArraySegment<byte>(data, 0, data.Length), SocketFlags.None);
                listBox1.Items.Add($"[Вы]: {textBox1.Text}");

                if (message == "Bye")
                {
                    c.Shutdown(SocketShutdown.Both);
                    c.Close();
                    listBox1.Items.Add("[Соединение разорвано]");
                    InitializeClient();
                }
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await ConnectToServer();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await SendMessage();
        }
    }

}
