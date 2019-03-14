using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlertService
{
  public partial class ServiceStop : Form
  {
    public ServiceStop()
    {
      InitializeComponent();
    }

    private void btnStopService_Click(object sender, EventArgs e)
    {
      Program.StopService();
    }
  }
}
