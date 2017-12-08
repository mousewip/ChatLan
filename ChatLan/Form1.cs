using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ChatLan
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videoDevice;
        private FilterInfoCollection audioDevice;
        private VideoCaptureDevice videoSource;

        MJPEGStream stream;

        private IPEndPoint iep, remoteEp;
        private Socket server;

        private UdpClient uServer;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoSource.IsRunning)
                videoSource.Stop();
            if (stream.IsRunning)
                stream.Stop();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            iep = new IPEndPoint(IPAddress.Any, 1234);
            uServer = new UdpClient(1234);

            //remoteEp = new IPEndPoint(IPAddress.Any, 0);

            //while (true)
            //{
            //    byte[] data = uServer.Receive(ref remoteEp);
            //    try
            //    {
            //        Bitmap img = (Bitmap)Helper.Helper.Instance.Deserialize(data);
            //        ptbImage.Image = img;
            //    }
            //    catch (Exception exception)
            //    {
            //        //
            //    }
            //}


            Thread server_thread = new Thread(new ThreadStart(server_start));
            server_thread.Start();
        }

        private void server_start()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] data = uServer.Receive(ref ipep);
                    if (data.Length == 0) continue;
                    else
                    {
                        MemoryStream ms = new MemoryStream(data);
                        Image tmp = Image.FromStream(ms);
                        this.ptbImage.Invoke(new Action(() => { this.ptbImage.Image = tmp; }));
                    }
                }
                catch { }

            }
        }
    }
}
