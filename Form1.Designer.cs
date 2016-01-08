﻿namespace diNo
{
	partial class Form1
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
      this.btnReadLehrer = new System.Windows.Forms.Button();
      this.btnReadExcelFile = new System.Windows.Forms.Button();
      this.btnCreateExcel = new System.Windows.Forms.Button();
      this.btnReadExcelKurse = new System.Windows.Forms.Button();
      this.btnImportSchueler = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.button3 = new System.Windows.Forms.Button();
      this.btnFixstand = new System.Windows.Forms.Button();
      this.comboBoxZeitpunkt = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.textBoxStatusMessage = new System.Windows.Forms.TextBox();
      this.btnReport = new System.Windows.Forms.Button();
      this.btnSendMail = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.userControlVorkommnisse1 = new diNo.UserControlVorkommnisse();
      this.button4 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnReadLehrer
      // 
      this.btnReadLehrer.Location = new System.Drawing.Point(12, 12);
      this.btnReadLehrer.Name = "btnReadLehrer";
      this.btnReadLehrer.Size = new System.Drawing.Size(132, 23);
      this.btnReadLehrer.TabIndex = 0;
      this.btnReadLehrer.Text = "Lehrer einlesen";
      this.btnReadLehrer.UseVisualStyleBackColor = true;
      this.btnReadLehrer.Click += new System.EventHandler(this.btnReadLehrer_Click);
      // 
      // btnReadExcelFile
      // 
      this.btnReadExcelFile.Location = new System.Drawing.Point(209, 12);
      this.btnReadExcelFile.Name = "btnReadExcelFile";
      this.btnReadExcelFile.Size = new System.Drawing.Size(97, 23);
      this.btnReadExcelFile.TabIndex = 2;
      this.btnReadExcelFile.Text = "Notendatei lesen";
      this.btnReadExcelFile.UseVisualStyleBackColor = true;
      this.btnReadExcelFile.Click += new System.EventHandler(this.btnReadExcelFile_Click);
      // 
      // btnCreateExcel
      // 
      this.btnCreateExcel.Location = new System.Drawing.Point(12, 114);
      this.btnCreateExcel.Name = "btnCreateExcel";
      this.btnCreateExcel.Size = new System.Drawing.Size(132, 23);
      this.btnCreateExcel.TabIndex = 3;
      this.btnCreateExcel.Text = "Excel-Dateien erzeugen";
      this.btnCreateExcel.UseVisualStyleBackColor = true;
      this.btnCreateExcel.Click += new System.EventHandler(this.btnCreateExcels_Click);
      // 
      // btnReadExcelKurse
      // 
      this.btnReadExcelKurse.Location = new System.Drawing.Point(12, 70);
      this.btnReadExcelKurse.Name = "btnReadExcelKurse";
      this.btnReadExcelKurse.Size = new System.Drawing.Size(132, 38);
      this.btnReadExcelKurse.TabIndex = 4;
      this.btnReadExcelKurse.Text = "Kursplan Excel einlesen und Schueler zuordnen";
      this.btnReadExcelKurse.UseVisualStyleBackColor = true;
      this.btnReadExcelKurse.Click += new System.EventHandler(this.btnReadExcelKurse_Click);
      // 
      // btnImportSchueler
      // 
      this.btnImportSchueler.Location = new System.Drawing.Point(12, 41);
      this.btnImportSchueler.Name = "btnImportSchueler";
      this.btnImportSchueler.Size = new System.Drawing.Size(132, 23);
      this.btnImportSchueler.TabIndex = 5;
      this.btnImportSchueler.Text = "WinSV Schülerdaten importieren";
      this.btnImportSchueler.UseVisualStyleBackColor = true;
      this.btnImportSchueler.Click += new System.EventHandler(this.btnImportSchueler_Click);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(440, 12);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(96, 23);
      this.button1.TabIndex = 6;
      this.button1.Text = "Klassenansicht";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button3
      // 
      this.button3.Location = new System.Drawing.Point(227, 169);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(96, 23);
      this.button3.TabIndex = 8;
      this.button3.Text = "Noten prüfen";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // btnFixstand
      // 
      this.btnFixstand.Location = new System.Drawing.Point(227, 198);
      this.btnFixstand.Name = "btnFixstand";
      this.btnFixstand.Size = new System.Drawing.Size(96, 38);
      this.btnFixstand.TabIndex = 9;
      this.btnFixstand.Text = "Fixstand speichern";
      this.btnFixstand.UseVisualStyleBackColor = true;
      this.btnFixstand.Click += new System.EventHandler(this.btnFixstand_Click);
      // 
      // comboBoxZeitpunkt
      // 
      this.comboBoxZeitpunkt.FormattingEnabled = true;
      this.comboBoxZeitpunkt.Items.AddRange(new object[] {
            "Probezeit BOS",
            "Halbjahr",
            "1. PA",
            "2. PA",
            "Jahresende"});
      this.comboBoxZeitpunkt.Location = new System.Drawing.Point(100, 171);
      this.comboBoxZeitpunkt.Name = "comboBoxZeitpunkt";
      this.comboBoxZeitpunkt.Size = new System.Drawing.Size(121, 21);
      this.comboBoxZeitpunkt.TabIndex = 10;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 254);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(37, 13);
      this.label1.TabIndex = 11;
      this.label1.Text = "Status";
      // 
      // textBoxStatusMessage
      // 
      this.textBoxStatusMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxStatusMessage.BackColor = System.Drawing.SystemColors.Control;
      this.textBoxStatusMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.textBoxStatusMessage.CausesValidation = false;
      this.textBoxStatusMessage.Enabled = false;
      this.textBoxStatusMessage.Location = new System.Drawing.Point(55, 533);
      this.textBoxStatusMessage.Name = "textBoxStatusMessage";
      this.textBoxStatusMessage.Size = new System.Drawing.Size(1220, 20);
      this.textBoxStatusMessage.TabIndex = 12;
      this.textBoxStatusMessage.TabStop = false;
      // 
      // btnReport
      // 
      this.btnReport.Location = new System.Drawing.Point(440, 114);
      this.btnReport.Name = "btnReport";
      this.btnReport.Size = new System.Drawing.Size(96, 23);
      this.btnReport.TabIndex = 13;
      this.btnReport.Text = "Bericht öffnen";
      this.btnReport.UseVisualStyleBackColor = true;
      this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
      // 
      // btnSendMail
      // 
      this.btnSendMail.Location = new System.Drawing.Point(12, 143);
      this.btnSendMail.Name = "btnSendMail";
      this.btnSendMail.Size = new System.Drawing.Size(132, 23);
      this.btnSendMail.TabIndex = 14;
      this.btnSendMail.Text = "Mails schicken";
      this.btnSendMail.UseVisualStyleBackColor = true;
      this.btnSendMail.Click += new System.EventHandler(this.btnSendMail_Click);
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(440, 41);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(109, 23);
      this.button2.TabIndex = 15;
      this.button2.Text = "Vorkommnisansicht";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // userControlVorkommnisse1
      // 
      this.userControlVorkommnisse1.Location = new System.Drawing.Point(564, 12);
      this.userControlVorkommnisse1.Name = "userControlVorkommnisse1";
      this.userControlVorkommnisse1.Schueler = null;
      this.userControlVorkommnisse1.Size = new System.Drawing.Size(710, 493);
      this.userControlVorkommnisse1.TabIndex = 16;
      this.userControlVorkommnisse1.Visible = false;
      // 
      // button4
      // 
      this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.button4.Location = new System.Drawing.Point(215, 314);
      this.button4.Name = "button4";
      this.button4.Size = new System.Drawing.Size(124, 55);
      this.button4.TabIndex = 18;
      this.button4.Text = "Test";
      this.button4.UseVisualStyleBackColor = true;
      this.button4.Click += new System.EventHandler(this.button4_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1287, 558);
      this.Controls.Add(this.button4);
      this.Controls.Add(this.userControlVorkommnisse1);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.btnSendMail);
      this.Controls.Add(this.btnReport);
      this.Controls.Add(this.textBoxStatusMessage);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.comboBoxZeitpunkt);
      this.Controls.Add(this.btnFixstand);
      this.Controls.Add(this.button3);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.btnImportSchueler);
      this.Controls.Add(this.btnReadExcelKurse);
      this.Controls.Add(this.btnCreateExcel);
      this.Controls.Add(this.btnReadExcelFile);
      this.Controls.Add(this.btnReadLehrer);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

		}

		#endregion

    private System.Windows.Forms.Button btnReadLehrer;
    private System.Windows.Forms.Button btnReadExcelFile;
    private System.Windows.Forms.Button btnCreateExcel;
    private System.Windows.Forms.Button btnReadExcelKurse;
    private System.Windows.Forms.Button btnImportSchueler;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button3;
    private System.Windows.Forms.Button btnFixstand;
    private System.Windows.Forms.ComboBox comboBoxZeitpunkt;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox textBoxStatusMessage;
        private System.Windows.Forms.Button btnReport;
    private System.Windows.Forms.Button btnSendMail;
    private System.Windows.Forms.Button button2;
    private UserControlVorkommnisse userControlVorkommnisse1;
        private System.Windows.Forms.Button button4;
    }
}

