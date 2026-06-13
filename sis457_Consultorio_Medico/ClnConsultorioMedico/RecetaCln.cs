using CadConsultorioMedico;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnConsultorioMedico
{
    public class RecetaCln
    {
        public static int insertar(Receta receta)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                context.Receta.Add(receta);
                context.SaveChanges();
                return receta.id;
            }
        }

        public static int actualizar(Receta receta)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                var existente = context.Receta.Find(receta.id);

                existente.idHistorialClinico = receta.idHistorialClinico;
                existente.medicamentos = receta.medicamentos;
                existente.dosis = receta.dosis;
                existente.indicaciones = receta.indicaciones;
                existente.usuarioRegistro = receta.usuarioRegistro;

                return context.SaveChanges();
            }
        }

        public static int eliminar(int id, string usuario)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                var existente = context.Receta.Find(id);

                existente.estado = -1;
                existente.usuarioRegistro = usuario;

                return context.SaveChanges();
            }
        }

        public static Receta obtenerUno(int id)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                return context.Receta.Find(id);
            }
        }

        public static List<paRecetaListar_Result> listarPa()
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                return context.paRecetaListar()
                    .OrderByDescending(x => x.fechaRegistro)
                    .ToList();
            }
        }
    }
}
