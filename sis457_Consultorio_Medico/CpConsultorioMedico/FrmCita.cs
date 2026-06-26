using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C1.Framework;
using CadConsultorioMedico;
using ClnConsultorioMedico;
using CpMinerva;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CpConsultorioMedico
{
    public partial class FrmCita : Form
    {
        private bool esNuevo = false;
        private bool formularioCargado = false;
        private string parametroValido = "";
        public FrmCita()
        {
            InitializeComponent();
        }

        // Busca columna por nombre (case-insensitive)
        private DataGridViewColumn GetColumn(string name)
        {
            if (dgvLista?.Columns == null) return null;
            return dgvLista.Columns
                .Cast<DataGridViewColumn>()
                .FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)
                                  || string.Equals(c.HeaderText, name, StringComparison.OrdinalIgnoreCase));
        }

        // Obtiene de forma segura el valor de una celda por nombre de columna
        private object GetCellValue(DataGridViewRow row, string columnName)
        {
            var col = GetColumn(columnName);
            if (col == null) return null;
            var cell = row.Cells[col.Index];
            if (cell == null || Convert.IsDBNull(cell.Value)) return null;
            return cell.Value;
        }

        // Intenta determinar si la fila está pagada. Devuelve true/false en out y true si se pudo determinar.
        private bool TryIsPagada(DataGridViewRow row, out bool isPagada)
        {
            isPagada = false;
            if (row == null) return false;

            // Nombres de columna candidatas que pueden expresar pago
            var candidatos = new[] { "Pagada", "pagada", "Pagado", "pagado", "IsPaid", "isPaid", "estadoPago", "estado_pago" };

            foreach (var nombre in candidatos)
            {
                var val = GetCellValue(row, nombre);
                if (val == null) continue;

                // Boolean
                if (val is bool b) { isPagada = b; return true; }

                // Entero (0/1)
                if (val is int i) { isPagada = i != 0; return true; }
                if (val is long l) { isPagada = l != 0; return true; }

                // String: aceptar "sí", "si", "yes", "true", "1", "pagado"
                var s = val.ToString().Trim();
                if (string.IsNullOrEmpty(s)) continue;
                var sNorm = s.ToLowerInvariant();
                if (sNorm == "sí" || sNorm == "si" || sNorm == "yes" || sNorm == "true" || sNorm == "pagado" || sNorm == "1")
                {
                    isPagada = true;
                    return true;
                }
                if (sNorm == "no" || sNorm == "false" || sNorm == "0" || sNorm == "pendiente")
                {
                    isPagada = false;
                    return true;
                }

                // Otros formatos: intentar parseo booleano
                if (bool.TryParse(s, out bool parsedBool)) { isPagada = parsedBool; return true; }
                if (int.TryParse(s, out int parsedInt)) { isPagada = parsedInt != 0; return true; }
            }

            // No se pudo determinar
            return false;
        }
        // metodo Listar citas por paciente
        public void listar()
        {
            var lista = CitaCln.listarPa(txtParametro.Text.Trim());
            dgvLista.DataSource = lista;

            // Proteger accesos a columnas (las oculta)
            var col = GetColumn("id"); if (col != null) col.Visible = false;
            col = GetColumn("estado"); if (col != null) col.Visible = false;
            col = GetColumn("idEspecialidad"); if (col != null) col.Visible = false;

            col = GetColumn("fecha"); if (col != null) col.HeaderText = "Fecha Programada";
            col = GetColumn("Hora"); if (col != null) col.HeaderText = "Hora Programada";
            col = GetColumn("cedulaIdentidad"); if (col != null) col.HeaderText = "Cédula de Identidad";
            col = GetColumn("nombreCompletoPaciente"); if (col != null) col.HeaderText = "Paciente";
            col = GetColumn("nombre"); if (col != null) col.HeaderText = "Especialidad";
            col = GetColumn("nombreCompletoDoctor"); if (col != null) col.HeaderText = "Doctor";
            col = GetColumn("fecha1"); if (col != null) col.HeaderText = "Fecha de Pago";
            col = GetColumn("usuarioRegistro"); if (col != null) col.HeaderText = "Usuario Registro";
            col = GetColumn("fechaRegistro"); if (col != null) col.HeaderText = "Fecha Registro";

            // verifica si hay registros en la lista 
            if (lista.Count > 0)
            {
                var cFecha = GetColumn("fecha");
                if (cFecha != null && dgvLista.Rows[0].Cells.Count > cFecha.Index)
                    dgvLista.CurrentCell = dgvLista.Rows[0].Cells[cFecha.Index];
                else if (dgvLista.Rows[0].Cells.Count > 0)
                    dgvLista.CurrentCell = dgvLista.Rows[0].Cells[0];
            }
            btnEditar.Enabled = lista.Count > 0;
            btnEliminar.Enabled = lista.Count > 0;
        }
        // metodo limpiar controles
        private void limpiar()
        {
            txtPaciente.Clear();
            cbxEspecialidad.SelectedIndex = -1;
            cbxDoctor.SelectedIndex = -1;
            dtpFecha.Value = DateTime.Now;
            cbxHora.SelectedIndex = -1;
        }
        // apertura del formulario citas
        private void FrmCita_Load(object sender, EventArgs e)
        {
            Size = new Size(862, 539);

            // evita seleccionar fechas pasadas

            dtpFecha.MinDate = DateTime.Today;

            listar();
            cargarEspecialidades();
            cargarHoras();
            txtFPaciente.Enabled = false;
            txtPaciente.Enabled = false;
            formularioCargado = true;

            dtpFFecha.Visible = false;
            lblFFecha.Visible = false;

            dgvLista_SelectionChanged(dgvLista, EventArgs.Empty);

            // Inicialmente deshabilitar los controles de registro de cita
            habilitarControles(false);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
        // optener paciente por cedula de identidad (BUSCAR)
        public void obtenerPaciente()
        {
            // Verificar si el campo de búsqueda está vacío
            if (string.IsNullOrWhiteSpace(txtParametro.Text.Trim()))
            {
                txtFPaciente.Text = string.Empty;
                return;
            }

            string nombrePaciente = PacienteCln.obtenerNombrePaciente(txtParametro.Text.Trim());

            if (string.IsNullOrEmpty(nombrePaciente))
            {
                txtFPaciente.Text = string.Empty;
                MessageBox.Show("El paciente no existe", "::: Consultorio Médico - Mensaje :::",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                txtFPaciente.Text = nombrePaciente;

                // desabilita los controles para registrar una nueva cita
                habilitarControles(false);

               // MessageBox.Show(
                    //"Paciente encontrado. Presione 'Nueva Cita' para registrar una cita.",
                   // "Consultorio Médico",
                  //  MessageBoxButtons.OK,
                  //  MessageBoxIcon.Information);
            }
        }
        // evento click del boton buscar
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            listar();
            obtenerPaciente();
        }
        private void txtParametro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                listar();
                obtenerPaciente();
            }
        }
        // metodo listar citas por fecha (filtrar por fecha)
        public void listarFecha()
        {
            var lista = CitaCln.listarFecha(dtpFFecha.Value);
            dgvLista.DataSource = lista;

            var col = GetColumn("id"); if (col != null) col.Visible = false;
            col = GetColumn("estado"); if (col != null) col.Visible = false;
            col = GetColumn("idEspecialidad"); if (col != null) col.Visible = false;

            col = GetColumn("fecha"); if (col != null) col.HeaderText = "Fecha Programada";
            col = GetColumn("Hora"); if (col != null) col.HeaderText = "Hora Programada";
            col = GetColumn("cedulaIdentidad"); if (col != null) col.HeaderText = "Cédula de Identidad";
            col = GetColumn("nombreCompletoPaciente"); if (col != null) col.HeaderText = "Paciente";
            col = GetColumn("nombre"); if (col != null) col.HeaderText = "Especialidad";
            col = GetColumn("nombreCompletoDoctor"); if (col != null) col.HeaderText = "Doctor";
            col = GetColumn("usuarioRegistro"); if (col != null) col.HeaderText = "Usuario Registro";
            col = GetColumn("fechaRegistro"); if (col != null) col.HeaderText = "Fecha Registro";

            if (lista.Count > 0)
            {
                var cFecha = GetColumn("fecha");
                if (cFecha != null && dgvLista.Rows[0].Cells.Count > cFecha.Index)
                    dgvLista.CurrentCell = dgvLista.Rows[0].Cells[cFecha.Index];
                else if (dgvLista.Rows[0].Cells.Count > 0)
                    dgvLista.CurrentCell = dgvLista.Rows[0].Cells[0];
            }
            btnEditar.Enabled = lista.Count > 0;
            btnEliminar.Enabled = lista.Count > 0;
        }
        // evento al cambiar la fecha de busqueda (filtrar por fecha)
        private void dtpFFecha_ValueChanged(object sender, EventArgs e)
        {
            listarFecha();
            if (dtpFFecha.Value != DateTime.Now)
            {
                txtParametro.Clear();
                txtFPaciente.Clear();
            }
        }
        // evento click del boton nueva cita
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFPaciente.Text))
            {
                MessageBox.Show(
                    "Primero debe buscar un paciente.",
                    "Consultorio Médico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            esNuevo = true;

            // habilita controles

            habilitarControles(true);

            Size = new Size(862, 713);

            cbxEspecialidad.Focus();

            cbxDoctor.DataSource = null;
            cbxDoctor.Items.Clear();
            // deseleccionar especialidad y doctor al iniciar nueva cita
            cbxDoctor.SelectedIndex = -1;
        }

        // metodo para cargar especialidades y doctores
        private void cargarEspecialidades()
        {
            var especialidades = EspecialidadCln.listar();
            cbxEspecialidad.DataSource = especialidades;
            cbxEspecialidad.DisplayMember = "nombre";
            cbxEspecialidad.ValueMember = "id";
            cbxEspecialidad.SelectedIndex = -1;
        }
        // metodo para cargar doctores por especialidad
        private void cargarDoctores(int idEspecialidad)
        {
            var doctores = DoctorCln.listarPorEspecialidad(idEspecialidad);
            cbxDoctor.DataSource = null;
            cbxDoctor.Items.Clear();
            cbxDoctor.DataSource = doctores;
            cbxDoctor.DisplayMember = "nombreCompletoDoctor";
            cbxDoctor.ValueMember = "id";
            cbxDoctor.SelectedIndex = -1;
        }
        // EVENTO al cambiar la especialidad, se cargan los doctores disponibles para esa especialidad
        private void cbxEspecialidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (formularioCargado)
            {
                int idEspecialidad = Convert.ToInt32(cbxEspecialidad.SelectedValue);
                cargarDoctores(idEspecialidad);
            }
        }
        // metodo para cargar horas de atencion del doctor
        private void cargarHoras()
        {
            cbxHora.Items.Clear();

            for (int h = 9; h <= 17; h++)
            {
                cbxHora.Items.Add(new TimeSpan(h, 0, 0));
                if (h != 17)
                    cbxHora.Items.Add(new TimeSpan(h, 30, 0));
            }

            cbxHora.SelectedIndex = -1;
        }

        // metodo cargar hora disponible
       
        private void cargarHorasDisponibles()
        {
            cbxHora.Items.Clear();

            int idDoctor;
            // Validar que se haya seleccionado un doctor y que el valor sea un entero válido
            if (cbxDoctor.SelectedValue == null ||
                !int.TryParse(cbxDoctor.SelectedValue.ToString(), out idDoctor))
                return;

            DateTime fecha = dtpFecha.Value.Date;

            for (int h = 9; h <= 17; h++)
            {
                agregarHora(new TimeSpan(h, 0, 0), idDoctor, fecha);

                if (h != 17)
                    agregarHora(new TimeSpan(h, 30, 0), idDoctor, fecha);
            }

            cbxHora.SelectedIndex = -1;
        }

        // metodo auxiliar agregarHora

        private void agregarHora(
        TimeSpan hora,
        int idDoctor,
        DateTime fecha)
        {
            bool ocupado =
                CitaCln.existeHorarioOcupado(
                    idDoctor,
                    fecha,
                    hora);

            string texto =
                ocupado
                ? "🔴 " + hora.ToString(@"hh\:mm") + " Ocupado"
                : "🟢 " + hora.ToString(@"hh\:mm") + " Libre";

            cbxHora.Items.Add(texto);
        }

        // EVENTO PARA EDITAR CITA
        private void btnEditar_Click(object sender, EventArgs e)
        {
            esNuevo = false;

            habilitarControles(true);

            Size = new Size(862, 713);
            int index = dgvLista.CurrentCell.RowIndex;
            int id = Convert.ToInt32(dgvLista.Rows[index].Cells["id"].Value);
            var cita = CitaCln.obtenerUno(id);
            var paciente = PacienteCln.obtenerUno(cita.idPaciente);
            txtPaciente.Text = paciente.nombreCompletoPaciente;
            cbxEspecialidad.SelectedValue = cita.idEspecialidad;
            cbxDoctor.SelectedValue = cita.idDoctor;
            dtpFecha.Value = cita.fecha;
            cbxHora.SelectedItem = cita.hora;
            cbxEspecialidad.Focus();
        }
        // evento click del boton cancelar
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            esNuevo = false;

            txtFPaciente.Clear();
            txtParametro.Clear();

            habilitarControles(false);

            Size = new Size(862, 539);

            limpiar();
        }
        // evento para sincronizar el nombre del paciente con el campo oculto
        private void txtFPaciente_TextChanged(object sender, EventArgs e)
        {
            txtPaciente.Text = txtFPaciente.Text;
        }
        // metodo para validar campos obligatorios
        private bool validar()
        {
            bool esValido = true;
            erpEspecialidad.SetError(cbxEspecialidad, "");
            erpDoctor.SetError(cbxDoctor, "");
            erpFecha.SetError(dtpFecha, "");
            erpHora.SetError(cbxHora, "");
            if (string.IsNullOrEmpty(cbxEspecialidad.Text))
            {
                erpEspecialidad.SetError(cbxEspecialidad, "El campo Especilalidad es obligatorio");
                esValido = false;
            }
            if (string.IsNullOrEmpty(cbxDoctor.Text))
            {
                erpDoctor.SetError(cbxDoctor, "El campo Doctor es obligatorio");
                esValido = false;
            }
            if (dtpFecha.Value.Date < DateTime.Today)
            {
                erpFecha.SetError(dtpFecha, "El campo Fecha no puede ser una fecha pasada");
                esValido = false;
            }
            if (string.IsNullOrEmpty(cbxHora.Text))
            {
                erpHora.SetError(cbxHora, "El campo Hora es obligatorio");
                esValido = false;
            }

            return esValido;
        }
        // metodo para guardar cita
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (validar())
            {
                // objeto donde se almacena los datos optenidos
                var cita = new Cita();

                // declara dos variables
                int idEspecialidadSelected = 0;
                int idDoctorSelected = 0;
                if (cbxEspecialidad?.SelectedValue != null) int.TryParse(cbxEspecialidad.SelectedValue.ToString(), out idEspecialidadSelected);
                if (cbxDoctor?.SelectedValue != null) int.TryParse(cbxDoctor.SelectedValue.ToString(), out idDoctorSelected);

                cita.idEspecialidad = idEspecialidadSelected;
                cita.idDoctor = idDoctorSelected;
                cita.fecha = dtpFecha.Value;

                /////////// VERIFICACION DE HORA OCUPADA ////////////////////////

                //metodo  para identificar una hora ocupada
                string textoHora = cbxHora.Text;

                if (textoHora.Contains("🔴"))
                {
                    MessageBox.Show(
                        "El horario seleccionado está ocupado.",
                        "Consultorio Médico",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // extrae solamente la hora, y guarda al usuario
                string horaTexto = textoHora.Substring(2, 5);

                cita.hora = TimeSpan.Parse(horaTexto);
                
                cita.usuarioRegistro = Util.usuario?.usuario ?? string.Empty;

                ////////// REGISTRO DE UNA NUEVA CITA /////////////////

                if (esNuevo)
                {
                    txtPaciente.Text = txtFPaciente.Text;
                    var cedulaIdentidad = txtParametro.Text.Trim();
                    var paciente = PacienteCln.buscarPorCedula(cedulaIdentidad);

                    if (paciente == null)
                    {
                        MessageBox.Show("No se encontró el paciente con la cédula proporcionada.", "::: Consultorio Médico - Mensaje :::",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    // paciente ya cuenta con una cita registrada para esa especialidad en la misma fecha

                    if (CitaCln.existeCita(paciente.id, cita.idEspecialidad, dtpFecha.Value))
                    {
                        MessageBox.Show("Este paciente ya tiene una cita registrada para esta especialidad en la misma fecha.",
                            "::: Consultorio Médico - BUENA SALUD :::",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    // validacion para que el doctor ya tiene una citaen esa fecha y horario 

                    if (CitaCln.existeHorarioOcupado(cita.idDoctor, dtpFecha.Value, cita.hora))
                    {
                        MessageBox.Show(
                            "El doctor ya tiene una cita programada en esa fecha y hora.",
                            "::: Consultorio Médico - BUENA SALUD :::",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                    cita.idPaciente = paciente.id;
                    cita.fechaRegistro = DateTime.Now;
                    cita.estado = 1;
                    CitaCln.insertar(cita);
                }

                        //////// ACTUALIZACION (EDITAR UNA CITA) ///////
                else
                {
                    // validacion de fila seleccionada
                    if (dgvLista == null || dgvLista.CurrentRow == null)
                    {
                        MessageBox.Show("No hay una cita seleccionada para editar.", "::: Consultorio Médico - Mensaje :::",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Obtener id de cita seleccionada
                    var idObj = GetCellValue(dgvLista.CurrentRow, "id");
                    if (idObj == null || !int.TryParse(idObj.ToString(), out int id))
                    {
                        MessageBox.Show("No se pudo obtener el identificador de la cita seleccionada.", "::: Consultorio Médico - Mensaje :::",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    cita.id = id;

                    var paciente = PacienteCln.buscar(txtPaciente.Text.Trim());

                    if (paciente == null)
                    {
                        MessageBox.Show("No se encontró el paciente (edición). Verifique el nombre.", "::: Consultorio Médico - Mensaje :::",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    cita.idPaciente = paciente.id;
                    CitaCln.actualizar(cita);
                }

                listar();

                btnCancelar.PerformClick();

                MessageBox.Show("Cita guardada correctamente", "::: Consultorio Médico - Mensaje :::",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtFPaciente.Clear();
                txtParametro.Clear();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int index = dgvLista.CurrentCell.RowIndex;
            int id = Convert.ToInt32(dgvLista.Rows[index].Cells["id"].Value);
            string paciente = dgvLista.Rows[index].Cells["Paciente"].Value.ToString();
            DateTime fecha = Convert.ToDateTime(dgvLista.Rows[index].Cells["fecha"].Value);
            string fechaSolo = fecha.ToString("yyyy/MM/dd");
            string hora = dgvLista.Rows[index].Cells["hora"].Value.ToString();
            DialogResult dialog = MessageBox.Show($"¿Está seguro que desea anular la cita del paciente {paciente} para la fecha {fechaSolo} a horas {hora}?",
                "::: Consultorio Médico - BUENA SALUD ::: ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                CitaCln.eliminar(id, Util.usuario.usuario);
                listar();
                MessageBox.Show("Cita anulada correctamente", "::: Consultorio Médico - BUENA SALUD ::: ",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            txtFPaciente.Clear();
            txtParametro.Clear();
        }


        private void btnPagar_Click(object sender, EventArgs e)
        {
            // 1. Limpiar campos
            txtFPaciente.Clear();
            txtParametro.Clear();

            // 2. Verificar selección
            if (dgvLista.CurrentRow == null)
            {
                MessageBox.Show("No hay una cita seleccionada.", "::: Consultorio Médico - Mensaje :::",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dgvLista.CurrentRow;

            // 4. Obtener id de cita de forma segura
            var idObj = GetCellValue(row, "id");
            if (idObj == null || !int.TryParse(idObj.ToString(), out int id))
            {
                MessageBox.Show("No se pudo obtener el identificador de la cita seleccionada.", "::: Consultorio Médico - Mensaje :::",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Obtener nombre del paciente (intenta por nombre de propiedad o por header)
            var pacienteObj = GetCellValue(row, "nombreCompletoPaciente") ?? GetCellValue(row, "Paciente") ?? GetCellValue(row, "nombreCompletoPaciente");
            string paciente = pacienteObj?.ToString() ?? string.Empty;

            // Obtener especialidad (intenta por propiedad 'nombre' o por header 'Especialidad')
            var especialidadObj = GetCellValue(row, "nombre") ?? GetCellValue(row, "Especialidad");
            string especialidad = especialidadObj?.ToString() ?? string.Empty;

            // declara una variable para almacenar ID
            int idEspecialidad = 0;
            var idEspObj = GetCellValue(row, "idEspecialidad");
            if (idEspObj != null)
            {
                int.TryParse(idEspObj.ToString(), out idEspecialidad);
            }

            // 6. Abrir formulario de pago
            new FrmPago(this, id, paciente, especialidad, idEspecialidad).ShowDialog();
        }
         // evento donde se inserta parametro ID
        private void txtParametro_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtParametro.Text))
            {
                parametroValido = txtParametro.Text.Trim();
            }
        }
        // metodo pera refrescar despues de realizar pago
        public void refrescar()
        {
            var parametro = parametroValido;
            if (dtpFFecha.Value > DateTime.Now)
            {
                var lista = CitaCln.listarFecha(dtpFFecha.Value);
                dgvLista.DataSource = lista;
            }
            else
            {
                var lista = CitaCln.listarPa(parametro);
                dgvLista.DataSource = lista;
            }
        }
             // habilitar y desabilitar botones
        private void dgvLista_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null)
            {
                btnPagar.Enabled = false;
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
                return;
            }

            // Intentar determinar si la cita está pagada
            if (TryIsPagada(dgvLista.CurrentRow, out bool pagada))
            {
                if (pagada)
                {
                    btnPagar.Enabled = false;
                    btnEditar.Enabled = false;
                    btnEliminar.Enabled = false;
                }
                else
                {
                    btnPagar.Enabled = true;
                    btnEditar.Enabled = true;
                    btnEliminar.Enabled = true;
                }
            }
            else
            {
                // No se pudo determinar: comportamiento por defecto seguro (habilitar)
                btnPagar.Enabled = true;
                btnEditar.Enabled = true;
                btnEliminar.Enabled = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void lblFPaciente_Click(object sender, EventArgs e)
        {

        }
        // evento que carga horas disponibles cuando cambian de doctor y fecha
        private void cbxDoctor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!formularioCargado)
                return;

            cargarHorasDisponibles();
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            if (!formularioCargado)
                return;

            cargarHorasDisponibles();
        }
        // Método para habilitar o deshabilitar controles del formulario
        private void habilitarControles(bool habilitar)
        {

            cbxEspecialidad.Enabled = habilitar;
            cbxDoctor.Enabled = habilitar;
            dtpFecha.Enabled = habilitar;
            cbxHora.Enabled = habilitar;
            btnGuardar.Enabled = habilitar;
        }
    }
}