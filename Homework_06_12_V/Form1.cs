using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Homework_06_12_V
{
    public partial class Form1 : Form
    {
        private Socket ls;
        private Socket c;
        private IPAddress ip;
        private int port;
        private List<Socket> clients;

        public Form1()
        {
            InitializeComponent();
            UpdateUIOnConnection(false);
            ip = IPAddress.Parse("127.0.0.1");
            port = 8888;

            ls = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ls.Bind(new IPEndPoint(IPAddress.Any, 8888));
            ls.Listen(10);

            clients = new List<Socket>();
            InitializeServer();
        }

        private void InitializeServer()
        {
            ls = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ls.Bind(new IPEndPoint(ip, port));
            ls.Listen(10);
            _ = AcceptConnectionsAsync();
        }

        private async Task AcceptConnectionsAsync()
        {
            while (true)
            {
                try
                {
                    c = await ls.AcceptAsync();
                    listBox1.Items.Add("[Собеседник подключился]");

                    UpdateUIOnConnection(true);

                    _ = HandleClientAsync(c);
                }
                catch (Exception ex)
                {
                    listBox1.Items.Add($"{ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(Socket client)
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[256];
                    int bytesRead = await client.ReceiveAsync(new ArraySegment<byte>(data), SocketFlags.None);
                    string message = Encoding.UTF8.GetString(data, 0, bytesRead);

                    if (string.IsNullOrEmpty(message))
                    {
                        listBox1.Items.Add("[Собеседник отключился]");
                        textBox1.Enabled = false;
                        button1.Enabled = false;
                        clients.Remove(client);
                        break;
                    }

                    listBox1.Items.Add($"[Собеседник]: {message}");

                    foreach (var otherClient in clients.Where(c => c != client))
                    {
                        await otherClient.SendAsync(new ArraySegment<byte>(data, 0, bytesRead), SocketFlags.None);
                    }
                }
                catch (Exception ex)
                {
                    listBox1.Items.Add($"{ex.Message}");
                }
            }
        }

        private void UpdateUIOnConnection(bool isConnected)
        {
            button1.Enabled = textBox1.Enabled = isConnected;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string message = textBox1.Text;
                byte[] data = Encoding.UTF8.GetBytes(message);

                if (c.Connected)
                {
                    await c.SendAsync(new ArraySegment<byte>(data, 0, data.Length), SocketFlags.None);
                    listBox1.Items.Add($"[Вы]: {message}");
                }
                else
                {
                    listBox1.Items.Add("[Клиент не отвечает]");
                }
            }
            catch(Exception ex)
            {
                listBox1.Items.Add($"{ex.Message}");
            }
        }
    }
}
