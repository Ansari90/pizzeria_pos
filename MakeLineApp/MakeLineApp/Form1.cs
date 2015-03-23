using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DataHolders;
using System.IO;

namespace MakeLineApp
{
    public partial class Form1 : Form
    {
        OrderInfo orderInfo;
        IFormatter formatter;

        public Form1()
        {
            InitializeComponent();
            orderInfo = new OrderInfo();
            backgroundWorker1.RunWorkerAsync();
            formatter = new BinaryFormatter();
        }

        public void updateMakeLine()
        {
            makeLine_productBox.Items.Clear();
            for (int i = 0; i < orderInfo.productNames.Count; i++)
            {
                makeLine_productBox.Items.Add(orderInfo.productNames[i] + "\t" + orderInfo.productQuantities[i] + "\t" + DateTime.Now.Hour + ":" + DateTime.Now.Minute);
            }
        }

        private void makeLine_completedBtn_Click(object sender, EventArgs e)
        {
            if (makeLine_productBox.SelectedIndex >= 0)
            {
                if (orderInfo.productQuantities[makeLine_productBox.SelectedIndex] > 1)
                {
                    orderInfo.productQuantities[makeLine_productBox.SelectedIndex]--;
                }
                else
                {
                    orderInfo.removeAtIndex(makeLine_productBox.SelectedIndex, false);
                }
            }
            updateMakeLine();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            StreamReader reader = new StreamReader("networkData.txt");
            string targetCompName = reader.ReadLine();
            int portNum = Int32.Parse(reader.ReadLine());

            BackgroundWorker worker = (BackgroundWorker)sender;
            TcpListener listener;

            System.Net.IPAddress ipAddress = System.Net.Dns.GetHostEntry(targetCompName).AddressList[0];

            listener = new TcpListener(ipAddress, portNum);
            listener.Start();

            while (true)
            {
                System.Threading.Thread.Sleep(10);
                TcpClient client = listener.AcceptTcpClient();
                orderInfo = (OrderInfo)formatter.Deserialize(client.GetStream());
                worker.ReportProgress(0);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            updateMakeLine();
        }
    }
}
