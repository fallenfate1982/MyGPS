namespace AlertService
{
  partial class ServiceStop
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.btnStopService = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnStopService
      // 
      this.btnStopService.Location = new System.Drawing.Point(106, 46);
      this.btnStopService.Name = "btnStopService";
      this.btnStopService.Size = new System.Drawing.Size(285, 47);
      this.btnStopService.TabIndex = 0;
      this.btnStopService.Text = "Stop Service";
      this.btnStopService.UseVisualStyleBackColor = true;
      this.btnStopService.Click += new System.EventHandler(this.btnStopService_Click);
      // 
      // ServiceStop
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(464, 128);
      this.Controls.Add(this.btnStopService);
      this.Name = "ServiceStop";
      this.Text = "ServiceStop";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnStopService;
  }
}