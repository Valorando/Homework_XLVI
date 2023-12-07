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

namespace Homework_06_12_III
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

            Socket ls = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ls.Bind(new IPEndPoint(ip, port));
            ls.Listen(10);
            listBox1.Items.Add("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                Socket client = await ls.AcceptAsync();
                _ = Task.Run(() => ProcessClient(client));
            }
        }

        public async Task ProcessClient(Socket client)
        {
            byte[] data = new byte[256];
            int bytes = await client.ReceiveAsync(new ArraySegment<byte>(data), SocketFlags.None);
            string message = Encoding.UTF8.GetString(data, 0, bytes);

            if (message == "/time")
            {
                DateTime dt = DateTime.Now;
                TimeSpan ts = dt.TimeOfDay;
                byte[] response = Encoding.UTF8.GetBytes($"Текущее время: {ts}");
                await client.SendAsync(new ArraySegment<byte>(response), SocketFlags.None);
            }

            else if (message == "/date")
            {
                byte[] response = Encoding.UTF8.GetBytes($"Текущая дата: {DateTime.Today}");
                await client.SendAsync(new ArraySegment<byte>(response), SocketFlags.None);
            }

            else
            {
                byte[] response = Encoding.UTF8.GetBytes("Команда не распознана.");
                await client.SendAsync(new ArraySegment<byte>(response), SocketFlags.None);
            }

            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}
