using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms; 

namespace TruMobility.Network.Services 
{
    public partial class Form1 : Form
    {
        private MAFHandler m_Handler = new MAFHandler();

        public Form1()
        {
            InitializeComponent();
        }

        private void StartProcessor_Click(object sender, EventArgs e)
        {
            m_Handler.StartProcessing();

        }

        private void StopProcessor_Click(object sender, EventArgs e)
        {
            m_Handler.StopProcessing();
        }

        private void GetCallDirection_Click(object sender, EventArgs e)
        {
            MAFDbMgr db = new MAFDbMgr();
            string cc = "8";
            string c = db.GetCallDirection( cc );
            Console.WriteLine("TheCallDirectionIs: " + c);

        }



    }
}
