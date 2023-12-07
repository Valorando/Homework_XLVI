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

namespace Homework_06_12_II
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
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                await s.ConnectAsync(new IPEndPoint(ip, port));
                listBox1.Items.Add("Подключено к серверу.");

                string message = "Привет, сервер!";
                byte[] data = Encoding.UTF8.GetBytes(message);
                await s.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);

                data = new byte[256];
                int bytesReceived = await s.ReceiveAsync(new ArraySegment<byte>(data), SocketFlags.None);
                string response = Encoding.UTF8.GetString(data, 0, bytesReceived);
                DateTime dt = DateTime.Now;
                TimeSpan ts = dt.TimeOfDay;
                listBox1.Items.Add($"В {ts} от [{((IPEndPoint)s.RemoteEndPoint).Address}] получена строка: {response}");
            }
            catch(Exception ex)
            {
                listBox1.Items.Add($"Ошибка: {ex.Message}");
            }
            finally
            {
                s.Shutdown(SocketShutdown.Both);
                s.Close();
            }
        }
    }
}
