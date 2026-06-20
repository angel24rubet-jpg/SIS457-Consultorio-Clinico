using CadConsultorioMedico;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClnConsultorioMedico
{
    public class PacienteCln
    {
        public static int insertar(Paciente paciente)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                context.Paciente.Add(paciente);
                context.SaveChanges();
                return paciente.id;
            }
        }

        public static int actualizar(Paciente paciente)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                var existente = context.Paciente.Find(paciente.id);
                existente.cedulaIdentidad = paciente.cedulaIdentidad;
                existente.nombreCompletoPaciente = paciente.nombreCompletoPaciente;
                existente.fechaNacimiento = paciente.fechaNacimiento;
                existente.direccion = paciente.direccion;
                existente.celular = paciente.celular;
                existente.usuarioRegistro = paciente.usuarioRegistro;
                existente.estado = paciente.estado;
                return context.SaveChanges();
            }
        }
        //  ANULACION LOGICA EN CASCADA : Al eliminar un paciente, se anulan sus citas, historiales clínicos y recetas asociadas
        public static int eliminar(int id, string usuarioRegistro)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                // Paciente
                var paciente = context.Paciente.Find(id);

                if (paciente == null)
                    return 0;

                paciente.estado = -1;
                paciente.usuarioRegistro = usuarioRegistro;

                // Citas del paciente
                var citas = context.Cita
                    .Where(c => c.idPaciente == id)
                    .ToList();

                foreach (var cita in citas)
                {
                    cita.estado = -1;
                    cita.usuarioRegistro = usuarioRegistro;
                }

                // Historiales del paciente
                var historiales = context.HistorialClinico
                    .Where(h => h.idPaciente == id)
                    .ToList();

                foreach (var historial in historiales)
                {
                    historial.estado = -1;
                    historial.usuarioRegistro = usuarioRegistro;

                    // Recetas del historial
                    var recetas = context.Receta
                        .Where(r => r.idHistorialClinico == historial.id)
                        .ToList();

                    foreach (var receta in recetas)
                    {
                        receta.estado = -1;
                        receta.usuarioRegistro = usuarioRegistro;
                    }
                }
                // INTRUCCION QUE GUARDA Y ACTUALIZA LOS CAMBIOS REALIZADOS 
                return context.SaveChanges();
            }
        }

        public static Paciente obtenerUno(int id)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                return context.Paciente.Find(id);
            }
        }

        public static List<paPacienteListar_Result> listarPa(string parametro)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                // orden de listado ultimo que entra primero en la fila
                var lista = context.paPacienteListar()
                  .OrderByDescending(x => x.id)
                  .ToList();

                if (string.IsNullOrWhiteSpace(parametro))
                {
                    return lista;
                }

                return lista
                    .Where(x => !string.IsNullOrEmpty(x.nombreCompletoPaciente) &&
                                x.nombreCompletoPaciente.IndexOf(parametro, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    .ToList();
            }
        }
        public static Paciente buscar(string nombrePaciente)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                return context.Paciente.FirstOrDefault(x => x.nombreCompletoPaciente == nombrePaciente);
            }
        }
        public static Paciente buscarPorCedula(string cedulaIdentidad)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                return context.Paciente.AsNoTracking().FirstOrDefault(x => x.cedulaIdentidad == cedulaIdentidad);
            }
        }
        public static string obtenerNombrePaciente(string cedulaIdentidad)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                var paciente = context.Paciente.FirstOrDefault(x => x.cedulaIdentidad == cedulaIdentidad && x.estado != -1);
                return paciente != null ? paciente.nombreCompletoPaciente : null;
            }
        }
        // metodo para validar si el paciente existe por cedula

        public static bool existeCedula(string cedulaIdentidad)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                return context.Paciente.Any(x =>
                    x.cedulaIdentidad == cedulaIdentidad &&
                    x.estado != -1);
            }
        }
    }
}