namespace TicketingManager {
    partial class dlgPrintReport {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.cmbEvents = new System.Windows.Forms.ComboBox();
            this.cmbSessions = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.prgLoading = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // cmbEvents
            // 
            this.cmbEvents.FormattingEnabled = true;
            this.cmbEvents.Location = new System.Drawing.Point(13, 13);
            this.cmbEvents.Name = "cmbEvents";
            this.cmbEvents.Size = new System.Drawing.Size(446, 21);
            this.cmbEvents.TabIndex = 0;
            this.cmbEvents.SelectedIndexChanged += new System.EventHandler(this.cmbEvents_SelectedIndexChanged);
            // 
            // cmbSessions
            // 
            this.cmbSessions.Enabled = false;
            this.cmbSessions.FormattingEnabled = true;
            this.cmbSessions.Location = new System.Drawing.Point(13, 41);
            this.cmbSessions.Name = "cmbSessions";
            this.cmbSessions.Size = new System.Drawing.Size(446, 21);
            this.cmbSessions.TabIndex = 1;
            this.cmbSessions.SelectedIndexChanged += new System.EventHandler(this.cmbSessions_SelectedIndexChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(383, 69);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Enabled = false;
            this.btnPrint.Location = new System.Drawing.Point(302, 69);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // prgLoading
            // 
            this.prgLoading.Location = new System.Drawing.Point(13, 98);
            this.prgLoading.Name = "prgLoading";
            this.prgLoading.Size = new System.Drawing.Size(446, 23);
            this.prgLoading.TabIndex = 4;
            // 
            // dlgPrintReport
            // 
            this.AcceptButton = this.btnPrint;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(468, 131);
            this.Controls.Add(this.prgLoading);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cmbSessions);
            this.Controls.Add(this.cmbEvents);
            this.Name = "dlgPrintReport";
            this.Text = "Print Door Reports";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbEvents;
        private System.Windows.Forms.ComboBox cmbSessions;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.ProgressBar prgLoading;
    }
}

