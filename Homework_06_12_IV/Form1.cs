using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Homework_06_12_IV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Main();
        }

        public async Task Main()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 8888;
            Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await c.ConnectAsync(new IPEndPoint(ip, port));
                listBox1.Items.Add("Подключено к серверу");

                button1.Click += async (sender, e) => await HandleButtonClick(c);

            }
            catch (Exception ex)
            {
                listBox1.Items.Add($"Ошибка: {ex.Message}");
            }
        }

        private async Task HandleButtonClick(Socket c)
        {
            try
            {
                string command = textBox1.Text;

                byte[] data = Encoding.UTF8.GetBytes(command);
                await c.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);

                button1.Enabled = false;
                textBox1.Enabled = false;

                data = new byte[256];
                int bytesReceived = await c.ReceiveAsync(new ArraySegment<byte>(data), SocketFlags.None);
                string response = Encoding.UTF8.GetString(data, 0, bytesReceived);
                listBox1.Items.Add(response);
            }
            catch(Exception ex)
            {
                listBox1.Items.Add($"Ошибка: {ex.Message}");
            }
            finally
            {
                c.Shutdown(SocketShutdown.Both);
                c.Close();
            }
        }

    }
}
