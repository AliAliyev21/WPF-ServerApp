using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ServerApp
{
    public partial class MainWindow : Window
    {
        private IPAddress ipAddress = IPAddress.Parse("192.168.56.1");
        private int port = 27001;

        public MainWindow()
        {
            InitializeComponent();
            StartServer();
        }

        private async void StartServer()
        {
            try
            {
                TcpListener listener = new TcpListener(ipAddress, port);
                listener.Start();

                Console.WriteLine($"Server started. Listening on {ipAddress}:{port}");

                while (true)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    await HandleClientConnectionAsync(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }




        private async Task HandleClientConnectionAsync(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int imageSize = reader.ReadInt32();
                    byte[] imageData = reader.ReadBytes(imageSize);

                    await AddImageToListBoxAsync(imageData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client connection: {ex.Message}");
            }
            finally
            {
                client.Dispose();
            }
        }

        private async Task AddImageToListBoxAsync(byte[] imageData)
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageData);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                Image image = new Image();
                image.Source = bitmapImage;

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    imageListBox.Items.Add(image);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding image to ListBox: " + ex.Message);
            }
        }
    }
}
