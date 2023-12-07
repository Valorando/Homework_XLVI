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

namespace Homework_06_12
{
    public partial class Form1 : Form
    {
        public IPAddress ip = IPAddress.Parse("127.0.0.1");
        public int port = 8888;
        Socket ls = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public Form1()
        {
            InitializeComponent();
            Main();
        }

        public async Task Main()
        {
            ls.Bind(new IPEndPoint(ip, port));
            ls.Listen(10);
            listBox1.Items.Add("Сервер запущен... Ожидание подключений.");

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
            DateTime dt = DateTime.Now;
            TimeSpan ts = dt.TimeOfDay;
            listBox1.Items.Add($"В {ts} от [{((IPEndPoint)client.RemoteEndPoint).Address}] получена строка: {message}");
            
            byte[] response = Encoding.UTF8.GetBytes("Привет, клиент!");
            await client.SendAsync(new ArraySegment<byte>(response), SocketFlags.None);

            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}
