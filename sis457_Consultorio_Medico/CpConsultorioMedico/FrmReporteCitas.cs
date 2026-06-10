using System;
using System.Windows.Forms;
using ClnConsultorioMedico;

namespace CpConsultorioMedico
{
    public partial class FrmReporteCitas : Form
    {
        public FrmReporteCitas()
        {
            InitializeComponent();
        }

        private void FrmReporteCitas_Load(object sender, EventArgs e)
        {
            dtpFechaInicio.Value = DateTime.Today.AddMonths(-1);
            dtpFechaFin.Value = DateTime.Today;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var lista = CitaCln.reporteCitasPorFecha(
                dtpFechaInicio.Value.Date,
                dtpFechaFin.Value.Date);

            dgvReporte.DataSource = lista;
        }

        private void btnBuscar_Click_1(object sender, EventArgs e)
        {
            var lista = CitaCln.reporteCitasPorFecha(
            dtpFechaInicio.Value.Date,
            dtpFechaFin.Value.Date);

            dgvReporte.DataSource = lista;
        }

        private void FrmReporteCitas_Load_1(object sender, EventArgs e)
        {
            dtpFechaInicio.Value = DateTime.Today.AddMonths(-1);
            dtpFechaFin.Value = DateTime.Today;
        }
    }
}