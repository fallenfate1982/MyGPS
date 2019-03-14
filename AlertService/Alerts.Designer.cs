namespace AlertService
{
  partial class Alerts
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.dataMigrationTimer = new System.Windows.Forms.Timer(this.components);
      // 
      // dataMigrationTimer
      // 
      this.dataMigrationTimer.Enabled = true;
      this.dataMigrationTimer.Tick += new System.EventHandler(this.dataMigrationTimer_Tick);
      // 
      // Alerts
      // 
      this.ServiceName = "Service1";

    }

    #endregion

    private System.Windows.Forms.Timer dataMigrationTimer;
  }
}
