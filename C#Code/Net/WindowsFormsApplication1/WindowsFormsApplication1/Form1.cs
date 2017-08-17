using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZQLib;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            m_TcpClient = new ZQTcpClient();
            m_TcpClient.Initialize();
        }

        private void button1_Click(object sender, EventArgs e)
        {
 
            int port = int.Parse(edt_port.Text);
            m_TcpClient.ConnectToServer(edt_IPAddress.Text,port , 0);

        }

        private ZQTcpClient m_TcpClient = null;
    }
}
