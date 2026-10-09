using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class DEntradaSalidaRegistroControlResponse
    {
        public int id_entrada_salida { get; set; }

        public DateTime? fecha_ingreso_principal { get; set; }
        public string hora_ingreso_principal { get; set; }
        public string hora_egreso_principal { get; set; }

        public DateTime? fecha_ingreso_control_interno { get; set; }
        public string hora_ingreso_control_interno { get; set; }
        public string hora_egreso_control_interno { get; set; }

        public DateTime? fecha_ingreso_mesa_control { get; set; }
        public string hora_ingreso_mesa_control { get; set; }
        public string hora_egreso_mesa_control { get; set; }

        public DateTime? fecha_ingreso_acceso_4 { get; set; }
        public string hora_ingreso_acceso_4 { get; set; }
        public string hora_egreso_acceso_4 { get; set; }
    }
}
