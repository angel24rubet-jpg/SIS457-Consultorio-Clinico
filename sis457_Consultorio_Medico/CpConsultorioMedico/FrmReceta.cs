using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CadConsultorioMedico;

namespace CpConsultorioMedico
{
    public partial class FrmReceta : Form
    {
        public FrmReceta()
        {
            InitializeComponent();
        }
        // metodo para listar historial
        private void cargarHistoriales()
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                var lista = context.HistorialClinico
                    .Where(x => x.estado != -1)
                    .ToList();

                cbxHistorial.DataSource = lista;
                cbxHistorial.DisplayMember = "id";
                cbxHistorial.ValueMember = "id";
                cbxHistorial.SelectedIndex = -1;
            }
        }

        private void FrmReceta_Load(object sender, EventArgs e)
        {
            cargarHistoriales();
        }

        private void cbxHistorial_SelectedIndexChanged(object sender, EventArgs e)
        {
           // metodo para seleccionar una historial y aparezca su nombre

           if (cbxHistorial.SelectedValue == null)
                return;

            int idHistorial;

            if (!int.TryParse(cbxHistorial.SelectedValue.ToString(), out idHistorial))
                return;

            using (var context = new LabConsultorioMedicoEntities())
            {
                var historial = context.HistorialClinico
                    .FirstOrDefault(x => x.id == idHistorial);

                if (historial != null)
                {
                    var paciente = context.Paciente
                        .FirstOrDefault(p => p.id == historial.idPaciente);

                    if (paciente != null)
                    {
                        txtPaciente.Text = paciente.nombreCompletoPaciente;
                    }
                }
            }
        }
    }
}
