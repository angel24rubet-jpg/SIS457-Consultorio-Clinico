namespace CpConsultorioMedico
{
    partial class FrmMenu
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMenu));
            this.panelMenu = new System.Windows.Forms.Panel();
            this.btnAyuda = new System.Windows.Forms.Button();
            this.btnReportes = new System.Windows.Forms.Button();
            this.btnConcepto = new System.Windows.Forms.Button();
            this.btnCitas = new System.Windows.Forms.Button();
            this.btnHistorial = new System.Windows.Forms.Button();
            this.btnEspecialidades = new System.Windows.Forms.Button();
            this.btnDoctores = new System.Windows.Forms.Button();
            this.btnPacientes = new System.Windows.Forms.Button();
            this.panelContenido = new System.Windows.Forms.Panel();
            this.panelMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panelMenu.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelMenu.BackgroundImage")));
            this.panelMenu.Controls.Add(this.btnAyuda);
            this.panelMenu.Controls.Add(this.btnReportes);
            this.panelMenu.Controls.Add(this.btnConcepto);
            this.panelMenu.Controls.Add(this.btnCitas);
            this.panelMenu.Controls.Add(this.btnHistorial);
            this.panelMenu.Controls.Add(this.btnEspecialidades);
            this.panelMenu.Controls.Add(this.btnDoctores);
            this.panelMenu.Controls.Add(this.btnPacientes);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Padding = new System.Windows.Forms.Padding(5);
            this.panelMenu.Size = new System.Drawing.Size(305, 866);
            this.panelMenu.TabIndex = 1;
            // 
            // btnAyuda
            // 
            this.btnAyuda.BackColor = System.Drawing.Color.SkyBlue;
            this.btnAyuda.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAyuda.BackgroundImage")));
            this.btnAyuda.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAyuda.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAyuda.Location = new System.Drawing.Point(5, 682);
            this.btnAyuda.Name = "btnAyuda";
            this.btnAyuda.Size = new System.Drawing.Size(295, 97);
            this.btnAyuda.TabIndex = 0;
            this.btnAyuda.Text = "AYUDA";
            this.btnAyuda.UseVisualStyleBackColor = false;
            this.btnAyuda.Click += new System.EventHandler(this.btnAyuda_Click);
            // 
            // btnReportes
            // 
            this.btnReportes.BackColor = System.Drawing.Color.Azure;
            this.btnReportes.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnReportes.BackgroundImage")));
            this.btnReportes.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnReportes.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReportes.Location = new System.Drawing.Point(5, 593);
            this.btnReportes.Name = "btnReportes";
            this.btnReportes.Size = new System.Drawing.Size(295, 89);
            this.btnReportes.TabIndex = 1;
            this.btnReportes.Text = "REPORTES";
            this.btnReportes.UseVisualStyleBackColor = false;
            this.btnReportes.Click += new System.EventHandler(this.btnReportes_Click);
            // 
            // btnConcepto
            // 
            this.btnConcepto.BackColor = System.Drawing.Color.Azure;
            this.btnConcepto.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConcepto.BackgroundImage")));
            this.btnConcepto.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnConcepto.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConcepto.Location = new System.Drawing.Point(5, 495);
            this.btnConcepto.Name = "btnConcepto";
            this.btnConcepto.Size = new System.Drawing.Size(295, 98);
            this.btnConcepto.TabIndex = 2;
            this.btnConcepto.Text = "CONCEPTOS";
            this.btnConcepto.UseVisualStyleBackColor = false;
            this.btnConcepto.Click += new System.EventHandler(this.btnConcepto_Click);
            // 
            // btnCitas
            // 
            this.btnCitas.BackColor = System.Drawing.Color.Azure;
            this.btnCitas.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCitas.BackgroundImage")));
            this.btnCitas.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCitas.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCitas.Location = new System.Drawing.Point(5, 391);
            this.btnCitas.Name = "btnCitas";
            this.btnCitas.Size = new System.Drawing.Size(295, 104);
            this.btnCitas.TabIndex = 3;
            this.btnCitas.Text = "CITAS";
            this.btnCitas.UseVisualStyleBackColor = false;
            this.btnCitas.Click += new System.EventHandler(this.btnCitas_Click);
            // 
            // btnHistorial
            // 
            this.btnHistorial.BackColor = System.Drawing.Color.Azure;
            this.btnHistorial.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnHistorial.BackgroundImage")));
            this.btnHistorial.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnHistorial.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHistorial.Location = new System.Drawing.Point(5, 291);
            this.btnHistorial.Name = "btnHistorial";
            this.btnHistorial.Size = new System.Drawing.Size(295, 100);
            this.btnHistorial.TabIndex = 4;
            this.btnHistorial.Text = "HISTORIAL CLINICO";
            this.btnHistorial.UseVisualStyleBackColor = false;
            this.btnHistorial.Click += new System.EventHandler(this.btnHistorial_Click);
            // 
            // btnEspecialidades
            // 
            this.btnEspecialidades.BackColor = System.Drawing.Color.Azure;
            this.btnEspecialidades.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnEspecialidades.BackgroundImage")));
            this.btnEspecialidades.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEspecialidades.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEspecialidades.Location = new System.Drawing.Point(5, 190);
            this.btnEspecialidades.Name = "btnEspecialidades";
            this.btnEspecialidades.Size = new System.Drawing.Size(295, 101);
            this.btnEspecialidades.TabIndex = 5;
            this.btnEspecialidades.Text = "ESPECIALIDADES";
            this.btnEspecialidades.UseVisualStyleBackColor = false;
            this.btnEspecialidades.Click += new System.EventHandler(this.btnEspecialidades_Click);
            // 
            // btnDoctores
            // 
            this.btnDoctores.BackColor = System.Drawing.Color.Azure;
            this.btnDoctores.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDoctores.BackgroundImage")));
            this.btnDoctores.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDoctores.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDoctores.Location = new System.Drawing.Point(5, 93);
            this.btnDoctores.Name = "btnDoctores";
            this.btnDoctores.Size = new System.Drawing.Size(295, 97);
            this.btnDoctores.TabIndex = 6;
            this.btnDoctores.Text = "DOCTORES";
            this.btnDoctores.UseVisualStyleBackColor = false;
            this.btnDoctores.Click += new System.EventHandler(this.btnDoctores_Click);
            // 
            // btnPacientes
            // 
            this.btnPacientes.BackColor = System.Drawing.Color.Azure;
            this.btnPacientes.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPacientes.BackgroundImage")));
            this.btnPacientes.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPacientes.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPacientes.Location = new System.Drawing.Point(5, 5);
            this.btnPacientes.Name = "btnPacientes";
            this.btnPacientes.Size = new System.Drawing.Size(295, 88);
            this.btnPacientes.TabIndex = 7;
            this.btnPacientes.Text = "PACIENTE";
            this.btnPacientes.UseVisualStyleBackColor = false;
            this.btnPacientes.Click += new System.EventHandler(this.btnPacientes_Click);
            // 
            // panelContenido
            // 
            this.panelContenido.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelContenido.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelContenido.BackgroundImage")));
            this.panelContenido.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenido.Location = new System.Drawing.Point(305, 0);
            this.panelContenido.Name = "panelContenido";
            this.panelContenido.Size = new System.Drawing.Size(958, 866);
            this.panelContenido.TabIndex = 0;
            this.panelContenido.Paint += new System.Windows.Forms.PaintEventHandler(this.panelContenido_Paint);
            // 
            // FrmMenu
            // 
            this.ClientSize = new System.Drawing.Size(1263, 866);
            this.Controls.Add(this.panelContenido);
            this.Controls.Add(this.panelMenu);
            this.Name = "FrmMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "::: Menú Principal - Consultorio Médico :::";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPrincipal_FormClosing);
            this.panelMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panelContenido;
        private System.Windows.Forms.Button btnPacientes;
        private System.Windows.Forms.Button btnDoctores;
        private System.Windows.Forms.Button btnEspecialidades;
        private System.Windows.Forms.Button btnHistorial;
        private System.Windows.Forms.Button btnCitas;
        private System.Windows.Forms.Button btnConcepto;
        private System.Windows.Forms.Button btnReportes;
        private System.Windows.Forms.Button btnAyuda;
    }
}