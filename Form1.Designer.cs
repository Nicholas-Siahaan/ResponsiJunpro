namespace ResponsiJunpro
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblAppTitle = new Label();
            lblApk = new Label();
            lblJudulDev = new Label();
            lblNama = new Label();
            txtNama = new TextBox();
            cbProyek = new ComboBox();
            lblProyek = new Label();
            lblStatus = new Label();
            cbStatus = new ComboBox();
            lblKinerja = new Label();
            lblFitur = new Label();
            lblBug = new Label();
            lblFiturDikerjakan = new Label();
            lblBugDitemukan = new Label();
            txtFitur = new TextBox();
            txtBug = new TextBox();
            btnInsert = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            lblPerforma = new Label();
            dgvPerforma = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvPerforma).BeginInit();
            SuspendLayout();
            // 
            // lblAppTitle
            // 
            lblAppTitle.AutoSize = true;
            lblAppTitle.Location = new Point(170, 19);
            lblAppTitle.Name = "lblAppTitle";
            lblAppTitle.Size = new Size(63, 15);
            lblAppTitle.TabIndex = 0;
            lblAppTitle.Text = "[App Title]";
            // 
            // lblApk
            // 
            lblApk.AutoSize = true;
            lblApk.Location = new Point(116, 34);
            lblApk.Name = "lblApk";
            lblApk.Size = new Size(204, 15);
            lblApk.TabIndex = 1;
            lblApk.Text = "Developer Team Performance Tracker";
            // 
            // lblJudulDev
            // 
            lblJudulDev.AutoSize = true;
            lblJudulDev.Location = new Point(18, 73);
            lblJudulDev.Name = "lblJudulDev";
            lblJudulDev.Size = new Size(101, 15);
            lblJudulDev.TabIndex = 2;
            lblJudulDev.Text = "DATA DEVELOPER";
            // 
            // lblNama
            // 
            lblNama.AutoSize = true;
            lblNama.Location = new Point(18, 104);
            lblNama.Name = "lblNama";
            lblNama.Size = new Size(95, 15);
            lblNama.TabIndex = 3;
            lblNama.Text = "Nama Developer";
            // 
            // txtNama
            // 
            txtNama.Location = new Point(149, 96);
            txtNama.Name = "txtNama";
            txtNama.Size = new Size(218, 23);
            txtNama.TabIndex = 4;
            txtNama.TextChanged += txtNama_TextChanged;
            // 
            // cbProyek
            // 
            cbProyek.FormattingEnabled = true;
            cbProyek.Location = new Point(149, 131);
            cbProyek.Name = "cbProyek";
            cbProyek.Size = new Size(218, 23);
            cbProyek.TabIndex = 5;
            cbProyek.Text = "Dropdown";
            cbProyek.SelectedIndexChanged += cbProyek_SelectedIndexChanged;
            // 
            // lblProyek
            // 
            lblProyek.AutoSize = true;
            lblProyek.Location = new Point(18, 134);
            lblProyek.Name = "lblProyek";
            lblProyek.Size = new Size(69, 15);
            lblProyek.TabIndex = 6;
            lblProyek.Text = "Pilih Proyek";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(18, 163);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(83, 15);
            lblStatus.TabIndex = 7;
            lblStatus.Text = "Status Kontrak";
            // 
            // cbStatus
            // 
            cbStatus.FormattingEnabled = true;
            cbStatus.Location = new Point(149, 160);
            cbStatus.Name = "cbStatus";
            cbStatus.Size = new Size(218, 23);
            cbStatus.TabIndex = 8;
            cbStatus.Text = "Dropdown";
            cbStatus.SelectedIndexChanged += cbStatus_SelectedIndexChanged;
            // 
            // lblKinerja
            // 
            lblKinerja.AutoSize = true;
            lblKinerja.Location = new Point(18, 213);
            lblKinerja.Name = "lblKinerja";
            lblKinerja.Size = new Size(83, 15);
            lblKinerja.TabIndex = 9;
            lblKinerja.Text = "DATA KINERJA";
            lblKinerja.Click += lblKinerja_Click;
            // 
            // lblFitur
            // 
            lblFitur.AutoSize = true;
            lblFitur.Location = new Point(18, 241);
            lblFitur.Name = "lblFitur";
            lblFitur.Size = new Size(69, 15);
            lblFitur.TabIndex = 10;
            lblFitur.Text = "Fitur Selesai";
            // 
            // lblBug
            // 
            lblBug.AutoSize = true;
            lblBug.Location = new Point(18, 265);
            lblBug.Name = "lblBug";
            lblBug.Size = new Size(69, 15);
            lblBug.TabIndex = 11;
            lblBug.Text = "Jumlah Bug";
            // 
            // lblFiturDikerjakan
            // 
            lblFiturDikerjakan.AutoSize = true;
            lblFiturDikerjakan.Location = new Point(254, 241);
            lblFiturDikerjakan.Name = "lblFiturDikerjakan";
            lblFiturDikerjakan.Size = new Size(164, 15);
            lblFiturDikerjakan.TabIndex = 12;
            lblFiturDikerjakan.Text = "(Jumlah fitur yang dikerjakan)";
            // 
            // lblBugDitemukan
            // 
            lblBugDitemukan.AutoSize = true;
            lblBugDitemukan.Location = new Point(256, 266);
            lblBugDitemukan.Name = "lblBugDitemukan";
            lblBugDitemukan.Size = new Size(166, 15);
            lblBugDitemukan.TabIndex = 13;
            lblBugDitemukan.Text = "(Jumlah bug yang ditemukan)";
            // 
            // txtFitur
            // 
            txtFitur.Location = new Point(149, 238);
            txtFitur.Name = "txtFitur";
            txtFitur.Size = new Size(100, 23);
            txtFitur.TabIndex = 14;
            txtFitur.TextChanged += txtFitur_TextChanged;
            // 
            // txtBug
            // 
            txtBug.Location = new Point(149, 263);
            txtBug.Name = "txtBug";
            txtBug.Size = new Size(100, 23);
            txtBug.TabIndex = 15;
            txtBug.TextChanged += txtBug_TextChanged;
            // 
            // btnInsert
            // 
            btnInsert.Location = new Point(12, 307);
            btnInsert.Name = "btnInsert";
            btnInsert.Size = new Size(75, 23);
            btnInsert.TabIndex = 16;
            btnInsert.Text = "INSERT";
            btnInsert.UseVisualStyleBackColor = true;
            btnInsert.Click += btnInsert_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(149, 307);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(75, 23);
            btnUpdate.TabIndex = 17;
            btnUpdate.Text = "UPDATE";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(304, 307);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 23);
            btnDelete.TabIndex = 18;
            btnDelete.Text = "DELETE";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // lblPerforma
            // 
            lblPerforma.AutoSize = true;
            lblPerforma.Location = new Point(18, 361);
            lblPerforma.Name = "lblPerforma";
            lblPerforma.Size = new Size(138, 15);
            lblPerforma.TabIndex = 19;
            lblPerforma.Text = "DAFTAR PERFORMA TIM";
            // 
            // dgvPerforma
            // 
            dgvPerforma.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPerforma.Location = new Point(12, 379);
            dgvPerforma.Name = "dgvPerforma";
            dgvPerforma.Size = new Size(432, 111);
            dgvPerforma.TabIndex = 20;
            dgvPerforma.CellContentClick += dgvPerforma_CellContentClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(464, 502);
            Controls.Add(dgvPerforma);
            Controls.Add(lblPerforma);
            Controls.Add(btnDelete);
            Controls.Add(btnUpdate);
            Controls.Add(btnInsert);
            Controls.Add(txtBug);
            Controls.Add(txtFitur);
            Controls.Add(lblBugDitemukan);
            Controls.Add(lblFiturDikerjakan);
            Controls.Add(lblBug);
            Controls.Add(lblFitur);
            Controls.Add(lblKinerja);
            Controls.Add(cbStatus);
            Controls.Add(lblStatus);
            Controls.Add(lblProyek);
            Controls.Add(cbProyek);
            Controls.Add(txtNama);
            Controls.Add(lblNama);
            Controls.Add(lblJudulDev);
            Controls.Add(lblApk);
            Controls.Add(lblAppTitle);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgvPerforma).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAppTitle;
        private Label lblApk;
        private Label lblJudulDev;
        private Label lblNama;
        private TextBox txtNama;
        private ComboBox cbProyek;
        private Label lblProyek;
        private Label lblStatus;
        private ComboBox cbStatus;
        private Label lblKinerja;
        private Label lblFitur;
        private Label lblBug;
        private Label lblFiturDikerjakan;
        private Label lblBugDitemukan;
        private TextBox txtFitur;
        private TextBox txtBug;
        private Button btnInsert;
        private Button btnUpdate;
        private Button btnDelete;
        private Label lblPerforma;
        private DataGridView dgvPerforma;
    }
}
