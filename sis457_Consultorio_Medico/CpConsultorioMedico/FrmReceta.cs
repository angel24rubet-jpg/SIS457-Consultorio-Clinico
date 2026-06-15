using CadConsultorioMedico;
using ClnConsultorioMedico;
using CpMinerva;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;


namespace CpConsultorioMedico
{
    public partial class FrmReceta : Form
    {
        private bool esNuevo = true;
        private int idReceta = 0;

        private string recetaPaciente = "";
        private string recetaMedicamentos = "";
        private string recetaDosis = "";
        private string recetaIndicaciones = "";
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
                    .Select(x => new
                    {
                        x.id,
                        Descripcion = x.id + " - " + x.Paciente.nombreCompletoPaciente
                    })
                    .ToList();

                cbxHistorial.DataSource = lista;
                cbxHistorial.DisplayMember = "Descripcion";
                cbxHistorial.ValueMember = "id";
                cbxHistorial.SelectedIndex = -1;
            }
        }

        // metodo para listar recetas
        private void listar()
        {
            var lista = RecetaCln.listarPa();

            dgvLista.DataSource = lista;

            dgvLista.Columns["id"].Visible = false;
            dgvLista.Columns["idPaciente"].Visible = false;

            dgvLista.Columns["Paciente"].HeaderText = "Paciente";
            dgvLista.Columns["medicamentos"].HeaderText = "Medicamentos";
            dgvLista.Columns["dosis"].HeaderText = "Dosis";
            dgvLista.Columns["indicaciones"].HeaderText = "Indicaciones";
            dgvLista.Columns["fechaRegistro"].HeaderText = "Fecha";
            dgvLista.Columns["usuarioRegistro"].HeaderText = "Usuario";

            if (dgvLista.Columns.Contains("estado"))
                dgvLista.Columns["estado"].Visible = false;

            btnEditar.Enabled = lista.Count > 0;
            btnEliminar.Enabled = lista.Count > 0;
        }
        // metodo limpiar campos

        private void limpiar()
        {
            txtCedulaIdentidad.Clear();

            cbxHistorial.DataSource = null;

            txtPaciente.Clear();
            txtDiagnostico.Clear();
            txtTratamiento.Clear();

            txtMedicamentos.Clear();
            txtDosis.Clear();
            txtIndicaciones.Clear();

            idPacienteSeleccionado = 0;

            txtCedulaIdentidad.Focus();
        }

        private void FrmReceta_Load(object sender, EventArgs e)
        {
            cargarHistoriales();
            listar();
        }
        // metodo buscar paciente por C.I.

        private int idPacienteSeleccionado = 0;

        private void buscarPaciente()
        {
            string ci = txtCedulaIdentidad.Text.Trim();

            if (string.IsNullOrEmpty(ci))
            {
                MessageBox.Show(
                    "Ingrese una cédula de identidad.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCedulaIdentidad.Focus();
                return;
            }

            using (var context = new LabConsultorioMedicoEntities())
            {
                var paciente = context.Paciente
                    .FirstOrDefault(x =>
                        x.cedulaIdentidad == ci &&
                        x.estado != -1);

                if (paciente == null)
                {
                    MessageBox.Show(
                        "No existe un paciente con esa cédula.",
                        "Búsqueda",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    txtPaciente.Clear();
                    idPacienteSeleccionado = 0;
                    return;
                }

                idPacienteSeleccionado = paciente.id;
                txtPaciente.Text = paciente.nombreCompletoPaciente;

                cargarHistorialesPaciente();
            }
        }

        private void cbxHistorial_SelectedIndexChanged(object sender, EventArgs e)
        {
            // metodo para seleccionar una historial y aparezca su nombre

            if (cbxHistorial.SelectedIndex == -1)
            {
                txtDiagnostico.Clear();
                txtTratamiento.Clear();
                return;
            }

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
                    txtDiagnostico.Text = historial.diagnostico;
                    txtTratamiento.Text = historial.tratamiento;
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cbxHistorial.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione un historial clínico.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cbxHistorial.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMedicamentos.Text))
            {
                MessageBox.Show(
                    "Ingrese los medicamentos.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtMedicamentos.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDosis.Text))
            {
                MessageBox.Show(
                    "Ingrese la dosis.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtDosis.Focus();
                return;
            }
            //objeto guardar

            var receta = new Receta();

            receta.idHistorialClinico =
                Convert.ToInt32(cbxHistorial.SelectedValue);

            receta.medicamentos =
                txtMedicamentos.Text.Trim();

            receta.dosis =
                txtDosis.Text.Trim();

            receta.indicaciones =
                txtIndicaciones.Text.Trim();

            receta.fechaRegistro =
                DateTime.Now;

            receta.usuarioRegistro =
                Util.usuario.usuario;

            // validacion de receta nueva o actualizacion

            receta.estado = 1;

            if (esNuevo)
            {
                RecetaCln.insertar(receta);

                MessageBox.Show(
                    "Receta registrada correctamente",
                    "::: Consultorio Médico :::",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                receta.id = idReceta;

                RecetaCln.actualizar(receta);

                MessageBox.Show(
                    "Receta actualizada correctamente",
                    "::: Consultorio Médico :::",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                esNuevo = true;
                idReceta = 0;
            }

            listar();
            limpiar();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null)
                return;

            esNuevo = false;

            idReceta = Convert.ToInt32(
                dgvLista.CurrentRow.Cells["id"].Value);

            var receta = RecetaCln.obtenerUno(idReceta);

            if (receta != null)
            {
                cbxHistorial.SelectedValue =
                    receta.idHistorialClinico;

                txtMedicamentos.Text =
                    receta.medicamentos;

                txtDosis.Text =
                    receta.dosis;

                txtIndicaciones.Text =
                    receta.indicaciones;
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccione una receta.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(
                dgvLista.CurrentRow.Cells["id"].Value);

            DialogResult resultado = MessageBox.Show(
                "¿Está seguro de eliminar la receta?",
                "::: Consultorio Médico :::",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                RecetaCln.eliminar(
                    id,
                    Util.usuario.usuario);

                listar();
                limpiar();

                MessageBox.Show(
                    "Receta eliminada correctamente",
                    "::: Consultorio Médico :::",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void btnBuscarPaciente_Click(object sender, EventArgs e)
        {
            buscarPaciente();

        }
        // metodo para cargar historial d paciente

        private void cargarHistorialesPaciente()
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                var lista = context.HistorialClinico
                    .Where(x =>
                        x.idPaciente == idPacienteSeleccionado &&
                        x.estado != -1)
                    .OrderByDescending(x => x.fecha)
                    .ToList()
                    .Select(x => new
                    {
                        x.id,
                        Descripcion = x.fecha.HasValue
                            ? x.fecha.Value.ToString("dd/MM/yyyy")
                            : "Sin fecha"
                    })
                    .ToList();

                cbxHistorial.DataSource = lista;
                cbxHistorial.DisplayMember = "Descripcion";
                cbxHistorial.ValueMember = "id";
                cbxHistorial.SelectedIndex = -1;

                txtDiagnostico.Clear();
                txtTratamiento.Clear();
            }
        }
        // evento imprimir receta
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccione una receta.",
                    "Información",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            recetaPaciente =
                dgvLista.CurrentRow.Cells["Paciente"].Value.ToString();

            recetaMedicamentos =
                dgvLista.CurrentRow.Cells["medicamentos"].Value.ToString();

            recetaDosis =
                dgvLista.CurrentRow.Cells["dosis"].Value.ToString();

            recetaIndicaciones =
                dgvLista.CurrentRow.Cells["indicaciones"].Value.ToString();

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Pd_PrintPage;

            PrintPreviewDialog vista = new PrintPreviewDialog();
            vista.Document = pd;
            vista.ShowDialog();
        }

        // FORMULARIO
        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font titulo = new Font("Arial", 16, FontStyle.Bold);
            Font texto = new Font("Arial", 11);

            int y = 50;

            e.Graphics.DrawString(
             "CONSULTORIO CLINICO BUENA SALUD ",
             titulo,
             Brushes.Black,
             250,
             y);

            y += 50;

            e.Graphics.DrawString(
                "RECETA MÉDICA ",
                titulo,
                Brushes.Black,
                250,
                y);

            y += 50;

            e.Graphics.DrawString(
                "Paciente: " + recetaPaciente,
                texto,
                Brushes.Black,
                50,
                y);

            y += 40;

            e.Graphics.DrawString(
                "Medicamentos:",
                texto,
                Brushes.Black,
                50,
                y);

            y += 30;

            e.Graphics.DrawString(
                recetaMedicamentos,
                texto,
                Brushes.Black,
                70,
                y);

            y += 60;

            e.Graphics.DrawString(
                "Dosis:",
                texto,
                Brushes.Black,
                50,
                y);

            y += 30;

            e.Graphics.DrawString(
                recetaDosis,
                texto,
                Brushes.Black,
                70,
                y);

            y += 60;

            e.Graphics.DrawString(
                "Indicaciones:",
                texto,
                Brushes.Black,
                50,
                y);

            y += 30;

            e.Graphics.DrawString(
                recetaIndicaciones,
                texto,
                Brushes.Black,
                70,
                y);

            y += 120;

            e.Graphics.DrawString(
                "________________________",
                texto,
                Brushes.Black,
                200,
                y);

            y += 25;

            e.Graphics.DrawString(
                "Firma y Sello Médico",
                texto,
                Brushes.Black,
                215,
                y);
        }

        private void txtPaciente_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
