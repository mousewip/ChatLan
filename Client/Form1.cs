using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Client
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videoDevice;
        private FilterInfoCollection audioDevice;
        private VideoCaptureDevice videoSource;

        MJPEGStream stream;

        private IPEndPoint iep;
        private Socket server;

        private UdpClient uClient;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            videoDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach (FilterInfo item in videoDevice)
            {
                cbbVideo.Items.Add(item.Name);
            }

            videoSource = new VideoCaptureDevice();
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            uClient = new UdpClient("127.0.0.1", 1234);

            Thread client_thread = new Thread(client_start);
            client_thread.Start();
        }

        private void client_start()
        {
            while (true)
            {
                Image tmp = null;
                MemoryStream ms = new MemoryStream();
                try
                {
                    this.ptbImage.Invoke(new Action(() => { tmp = (Image)ptbImage.Image.Clone(); }));
                    tmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] msg = ms.ToArray();
                    uClient.Send(msg, msg.Length);
                }
                catch
                {
                }
            }

        }

        private void videSource_newFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap img = (Bitmap)eventArgs.Frame.Clone();
                ptbImage.Image = img;
            }
            catch (Exception e)
            {
                //
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            if (videoSource.IsRunning)
            {
                videoSource.Stop();
                ptbImage.Image = null;
                ptbImage.Invalidate();
            }
            else
            {
                videoSource = new VideoCaptureDevice(videoDevice[cbbVideo.SelectedIndex].MonikerString);
                videoSource.NewFrame += videSource_newFrame;
                videoSource.Start();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoSource.IsRunning)
                videoSource.Stop();
            if (stream.IsRunning)
                stream.Stop();
        }
    }
}
