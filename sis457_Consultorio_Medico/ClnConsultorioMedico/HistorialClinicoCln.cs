using CadConsultorioMedico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClnConsultorioMedico
{
    public class HistorialClinicoCln
    {
        public static int insertar(HistorialClinico historialclinico)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                context.HistorialClinico.Add(historialclinico);
                context.SaveChanges();
                return historialclinico.id;
            }
        }

        public static int actualizar(HistorialClinico historialclinico)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                var existente = context.HistorialClinico.Find(historialclinico.id);
                existente.diagnostico = historialclinico.diagnostico;
                existente.tratamiento = historialclinico.tratamiento;
                existente.fecha = historialclinico.fecha;
                existente.fechaRegistro = historialclinico.fechaRegistro;
                existente.estado = historialclinico.estado;
                existente.usuarioRegistro = historialclinico.usuarioRegistro;
                return context.SaveChanges();
            }
        }
        // anulacion logica en cascada
        public static int eliminar(int id, string usuarioRegistro)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                // Anular historial
                var existente = context.HistorialClinico.Find(id);

                existente.estado = -1;
                existente.usuarioRegistro = usuarioRegistro;

                // Anular todas las recetas asociadas
                var recetas = context.Receta
                    .Where(r => r.idHistorialClinico == id)
                    .ToList();

                foreach (var receta in recetas)
                {
                    receta.estado = -1;
                    receta.usuarioRegistro = usuarioRegistro;
                }

                return context.SaveChanges();
            }
        }

        public static HistorialClinico obtenerUno(int id)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                return context.HistorialClinico.Find(id);
            }
        }

        public static List<paHistorialClinicoListar_Result> listarPa(string parametro)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                return context.paHistorialClinicoListar()
                    .OrderByDescending(x => x.id)
                    .ToList();
            }
        }
    }
}