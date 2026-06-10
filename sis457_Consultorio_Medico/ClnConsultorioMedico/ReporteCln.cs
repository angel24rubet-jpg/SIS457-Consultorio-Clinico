using CadConsultorioMedico;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnConsultorioMedico
{
    public class ReporteCln
    {
        public static List<paReporteCitasPorFecha_Result> listar(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            using (var context = new LabConsultorioMedicoEntities())
            {
                return context.paReporteCitasPorFecha(
                    fechaInicio,
                    fechaFin).ToList();
            }
        }
    }
}