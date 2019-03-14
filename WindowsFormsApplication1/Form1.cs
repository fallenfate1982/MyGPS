using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GTSDataStorage;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GTSDataStorage.GPSTrackerEntities1 context = new GTSDataStorage.GPSTrackerEntities1();
            context.AddToSimCards(GTSDataStorage.SimCard.CreateSimCard(0,"123"));
            context.SaveChanges();
            
            
        }
    }
}
